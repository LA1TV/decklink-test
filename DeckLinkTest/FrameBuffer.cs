using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace DeckLinkTest
{
    class FrameBuffer
    {
        uint _fboHandle;
        Texture _color;

        public FrameBuffer(int width, int height)
        {
            Width = width;
            Height = height;

            // Create Color Tex
            _color = new Texture();
            _color.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            // Create a FBO and attach the textures
            GL.Ext.GenFramebuffers(1, out _fboHandle);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, _color.Handle, 0);
            
            //Reset
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
        }

        public void Begin()
        {
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, _fboHandle);
            GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.PushAttrib(AttribMask.ViewportBit);
            GL.PushAttrib(AttribMask.ColorBufferBit);
            
            //set FrameBuffer ViewPort
            GL.Viewport(0, 0, Width, Height);
        }

        public void End()
        {
            GL.PopAttrib();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            //Draw back to the normal Buffer
            GL.DrawBuffer(DrawBufferMode.Back);
        }

        public Texture ColorTexture
        {
            get { return _color; }
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }
    }
}
