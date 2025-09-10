using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using OpenTK.Graphics.OpenGL4;

namespace HWEngine.Texture
{
    public class Texture2D : IDisposable
    {
        public int Handle { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }


        // Existing constructor for loading from file path
        public Texture2D(string path)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            using (var image = Image.Load<Rgba32>(path))
            {
                image.Mutate(x => x.Flip(FlipMode.Vertical));
                var pixels = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(pixels);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            }
            SetParameters();
        }

        // NEW constructor for loading from an ImageSharp object
        public Texture2D(Image<Rgba32> image)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            var pixels = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixels);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);
            SetParameters();
            Width = image.Width;
            Height = image.Height;
        }

        private void SetParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Crop()
        {
            // Bind the current texture to be able to read its data
            Bind();

            // Get texture dimensions from GPU
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out int width);
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out int height);

            // Retrieve pixel data from the GPU
            var pixels = new byte[width * height * 4];
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            // Load pixel data into an ImageSharp object
            using (var image = Image.LoadPixelData<Rgba32>(pixels, width, height))
            {
                // Flip back to normal coordinates for ImageSharp processing
                image.Mutate(x => x.Flip(FlipMode.Vertical));

                // Find the content bounds
                int minX = width;
                int minY = height;
                int maxX = 0;
                int maxY = 0;
                bool foundContent = false;

                // Manually iterate over the pixels to find the bounding box
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        var pixel = image[x, y];
                        if (pixel.A > 0)
                        {
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            if (y > maxY) maxY = y;
                            foundContent = true;
                        }
                    }
                }

                if (!foundContent) return; // Nothing to crop if the texture is fully transparent

                // Calculate the new cropped dimensions
                int croppedWidth = maxX - minX + 1;
                int croppedHeight = maxY - minY + 1;

                // Create a new, cropped image
                using (var croppedImage = image.Clone(
                    ctx => ctx.Crop(new Rectangle(minX, minY, croppedWidth, croppedHeight))))
                {
                    // Update dimensions
                    Width = croppedImage.Width;
                    Height = croppedImage.Height;

                    // Flip back for OpenGL
                    croppedImage.Mutate(x => x.Flip(FlipMode.Vertical));

                    // Generate a new texture handle
                    GL.DeleteTexture(Handle);
                    Handle = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, Handle);

                    // Copy pixel data and upload to GPU
                    var croppedPixels = new byte[croppedImage.Width * croppedImage.Height * 4];
                    croppedImage.CopyPixelDataTo(croppedPixels);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, croppedImage.Width, croppedImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, croppedPixels);
                }
            }

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Generate mipmaps
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Bind(int unit = 0)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + unit));
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Release(int unit = 0)
        {
            GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + unit));
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}