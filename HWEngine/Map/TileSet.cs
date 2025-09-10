using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using HWEngine.Texture;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;
namespace HWEngine.Map
{
    public static class TileSetLoader
    {
        public static List<Texture2D> LoadTileSet(string path, int tileWidth, int tileHeight)
        {
            var tiles = new List<Texture2D>();

            // Load the entire tileset image into memory
            using (var tilesetImage = Image.Load<Rgba32>(path))
            {
                // Ensure the tileset dimensions are valid
                if (tilesetImage.Width % tileWidth != 0 || tilesetImage.Height % tileHeight != 0)
                {
                    throw new InvalidOperationException("Tileset dimensions are not a multiple of the tile size.");
                }

                int cols = tilesetImage.Width / tileWidth;
                int rows = tilesetImage.Height / tileHeight;

                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        // Create a new image for the individual tile
                        using (var tileImage = tilesetImage.Clone(
                            ctx => ctx.Crop(new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight))))
                        {
                            // Create a new OpenGL texture from the cropped tile
                            var tileTexture = new Texture2D(tileImage);
                         //   tileTexture.Crop();
                        
                            tiles.Add(tileTexture);
                        }
                    }
                }
            }
            return tiles;
        }
    }
}
