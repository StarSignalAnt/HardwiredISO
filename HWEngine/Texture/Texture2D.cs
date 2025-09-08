using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System;
using System.Collections.Generic;
using System.Text;


namespace HWEngine.Texture
{
    public class Texture2D : IDisposable
    {

        public int Handle { get; private set; }

        public Texture2D(string path)
        {
            // Generate texture handle
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D,Handle);

            // Load image with ImageSharp
            using (var image = Image.Load<Rgba32>(path))
            {
                // Flip vertically for OpenGL coordinates
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                // Copy pixel data to byte array
                var pixels = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(pixels);

                // Upload to GPU

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            }


            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        }

        public void Bind(int unit=0)
        {
            GL.ActiveTexture((TextureUnit)(((int)TextureUnit.Texture0)+ unit));
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Release(int unit=0)
        {
            GL.ActiveTexture((TextureUnit)(((int)TextureUnit.Texture0) + unit));
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }

    }
}
