using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace DeckLinkTest
{
    internal class Texture
    {
        private uint _handle;

        /// <summary>
        /// Creates a blank null texture
        /// </summary>
        public Texture()
        {
            GL.GenTextures(1, out _handle);

            Console.WriteLine("TextureID:" + _handle + "");

            GL.BindTexture(TextureTarget.Texture2D, _handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.ClampToEdge);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Creates a texture from a file
        /// </summary>
        /// <param name="file">relative file name</param>
        public Texture(string file)
            : this()
        {
            if (String.IsNullOrEmpty(file))
                throw new ArgumentException(file);

            //Bind the texture
            GL.BindTexture(TextureTarget.Texture2D, _handle);
            var bitmap = new Bitmap(file);

            BitmapData data =
                bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            Console.WriteLine("Loading file:" + file);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte,
                data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            
            //Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Sets the current texture to active
        /// </summary>
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        /// <summary>
        /// Unbinds the Active texture to null
        /// </summary>
        public static void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Public accessor for texture Handle
        /// </summary>
        public uint Handle
        {
            get { return _handle; }
        }
    }
}
