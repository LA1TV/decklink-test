using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using DeckLinkAPI;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System.Threading;


namespace DeckLinkTest
{
    class DecklinkWindow : IDeckLinkVideoOutputCallback
    {
        //Constants
        const string VideoMode = "HD720p50";
        const int PreviewWidth = 800;
        const int PreviewHeight = 600;

        //OpenGL Fields
        INativeWindow _win;
        IGraphicsContext _context;
        FrameBuffer _fbo;
        Texture _texLogo;

        //Decklink Fields
        private DecklinkDevices _devices;
        private IDeckLinkOutput _deckLinkOutput;
        private IDeckLinkMutableVideoFrame _videoFrame;
        private long _frameDuration;
        private long _frameTimescale;
        private long _fps;
        private uint _totalFrames;

        //GL items
        private float squareRotx;
        private float squareRoty;

        public DecklinkWindow()
        {
            //Create the OpenGL window
            _win = new NativeWindow(PreviewWidth, PreviewHeight,
                "Decklink Test", GameWindowFlags.Default,
                GraphicsMode.Default, DisplayDevice.Default);

            //Now create a Graphics context
            //and set it active for this thread
            _context = new GraphicsContext(GraphicsMode.Default, _win.WindowInfo);
            _context.MakeCurrent(_win.WindowInfo);
            _context.LoadAll();

            //Create the frame buffer we will render to
            _fbo = new FrameBuffer(1280, 720);
            _texLogo = new Texture("la1tv.png");
            _devices = new DecklinkDevices();

            InitDecklink(0);

            _deckLinkOutput.SetScheduledFrameCompletionCallback(this);
            _totalFrames = 0;

            ResetFrame();
            SetPreroll();
            InitOpenGL();
            Draw(); //thread blocking!
        }

        #region OpenGL

        void InitOpenGL()
        {
            GL.ClearColor(Color.Black);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1.0);
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Texture2D);

            _context.Update(_win.WindowInfo);
            Console.WriteLine("InitGL Finnished");
        }

        void Draw()
        {
            //Show the window
            _win.Visible = true;

            while (_win.Exists)
            {
                #region FBO Buffer
                _fbo.Begin();
                //Set the projection type
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, _fbo.Width / (float)_fbo.Height, 1.0f, 64.0f);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref projection);

                //Set the 3D view
                Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref modelview);

                GL.Translate(0f, 0.0f, 3.0f);
                GL.Rotate(squareRotx, 1.0f, 0.0f, 0.0f);
                GL.Rotate(squareRoty, 0.0f, 1.0f, 0.0f);
                _texLogo.Bind();

                GL.Begin(BeginMode.Quads);
                #region Draw Sqaure
                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(-0.5f, 0.5f, 0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(-0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(0.5f, -0.5f, -0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(-0.5f, 0.5f, 0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(-0.5f, 0.5f, -0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(-0.5f, -0.5f, -0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(0.5f, -0.5f, 0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(-0.5f, 0.5f, 0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(0.5f, 0.5f, 0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(0.5f, 0.5f, -0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(-0.5f, 0.5f, -0.5f);

                GL.TexCoord2(0.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, 0.5f);
                GL.TexCoord2(1.0f, 1.0f);
                GL.Vertex3(-0.5f, -0.5f, -0.5f);
                GL.TexCoord2(1.0f, 0.0f);
                GL.Vertex3(0.5f, -0.5f, -0.5f);
                GL.TexCoord2(0.0f, 0.0f);
                GL.Vertex3(0.5f, -0.5f, 0.5f);
                #endregion
                GL.End();

                //Create a Buffer and store the frame
                IntPtr buffer;
                _videoFrame.GetBytes(out buffer);
                GL.ReadPixels(0, 0, 1280, 720, PixelFormat.Bgra, PixelType.UnsignedInt8888Reversed, buffer);

                _fbo.End();
                #endregion

                int width = _win.Size.Width;
                int height = _win.Size.Height;

                #region Back Buffer
                GL.Viewport(0, 0, width, height);

                //Set projection to 2D plane
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, width, height, 0, -1, 1);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                //Draw a full screen Quad with the FBO texture
                _fbo.ColorTexture.Bind();

                //Draw the framebuffer as a quad to a screen
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 1); GL.Vertex2(0, 0);
                GL.TexCoord2(0, 0); GL.Vertex2(0, height);
                GL.TexCoord2(1, 0); GL.Vertex2(width, height);
                GL.TexCoord2(1, 1); GL.Vertex2(width, 0);
                GL.End();

                Texture.Unbind();
                #endregion

                Thread.Sleep(5);
                _context.SwapBuffers();
                _win.ProcessEvents();
            }
        }

        #endregion

        #region Decklink Methods

        private void InitDecklink(int CardIndex)
        {
            int width, height;
            IDeckLinkDisplayMode displayMode;
            
            //Set the card display Mode
            displayMode = _devices[0].GetDisplayMode(VideoMode);

            //Get the timing information
            displayMode.GetFrameRate(out _frameDuration, out _frameTimescale);
            _fps = ((_frameTimescale + (_frameDuration - 1)) / _frameDuration);

            //Get the output interface from the device Index
            _deckLinkOutput = _devices[CardIndex].DeckLinkOutput;

            width = displayMode.GetWidth();
            height = displayMode.GetHeight();

            //Set up a pointer to a video frame
            //with the hight and width
            _deckLinkOutput.CreateVideoFrame(width, height, (width * 4),
                _BMDPixelFormat.bmdFormat8BitBGRA,
                _BMDFrameFlags.bmdFrameFlagFlipVertical,
                out _videoFrame);
         
            //Tell the card to enable output
            _deckLinkOutput.EnableVideoOutput(displayMode.GetDisplayMode(),
                _BMDVideoOutputFlags.bmdVideoOutputFlagDefault);

            Console.WriteLine("Using Card index:" + CardIndex);
        }

        /// <summary>
        /// Fills Buffer with black 
        /// </summary>
        protected void ResetFrame()
        {
            IntPtr buffer;
            int width, height;
            int wordsRemaining;
            UInt32 black = 0x00000000;
            int index = 0;

            _videoFrame.GetBytes(out buffer);
            width = _videoFrame.GetWidth();
            height = _videoFrame.GetHeight();

            wordsRemaining = (width * 2 * height) / 4;

            while (wordsRemaining-- > 0)
            {
                Marshal.WriteInt32(buffer, index * 4, (Int32)black);
                index++;
            }
        }

        /// <summary>
        /// Start 3 seconds of blank frames
        /// </summary>
        protected void SetPreroll()
        {
            for (uint i = 0; i < _fps; i++)
            {
                _deckLinkOutput.ScheduleVideoFrame(_videoFrame, (_totalFrames * _frameDuration), _frameDuration,
                    _frameTimescale);

                _totalFrames++;
            }

            _deckLinkOutput.StartScheduledPlayback(0, 100, 1.0);
        }

        #endregion

        #region IDeckLinkVideoOutputCallback Methods
        public void ScheduledFrameCompleted(IDeckLinkVideoFrame completedFrame, _BMDOutputFrameCompletionResult result)
        {

            /*
             * Have the roating cube here as it will
             * Move at a realtive frequency to the updates
             * to the decklink card
             */
            if (squareRotx >= 360)
                squareRotx = 0.0f;

            if (squareRoty >= 360)
                squareRoty = 0.0f;

            squareRotx += 1.5f;
            squareRoty += 0.5f;

            _deckLinkOutput.ScheduleVideoFrame(_videoFrame, (_totalFrames * _frameDuration), _frameDuration, _frameTimescale);
            _totalFrames++;
        }

        public void ScheduledPlaybackHasStopped()
        {
        }

        #endregion;
    }
}
