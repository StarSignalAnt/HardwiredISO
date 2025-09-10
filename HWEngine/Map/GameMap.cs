using HWEngine.App;
using HWEngine.Draw;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using HWEngine.Texture;

namespace HWEngine.Map
{
    public class GameMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public int TileWidth = 64;
        public int TileHeight = 32;
        public Draw.Drawer drawer;

        private GameTile[,,] _tiles;

        public float CamX { get; set; }
        public float CamY { get; set; }

        public GameMap(int width, int height, int depth, int tileWidth = 64, int tileHeight = 32)
        {
            drawer = new Drawer(HWApp.ScreenWidth, HWApp.ScreenHeight);
            if (width <= 0 || height <= 0 || depth <= 0)
            {
                throw new ArgumentException("Map dimensions must be greater than zero.");
            }

            Width = width;
            Height = height;
            Depth = depth;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            CamX = 0;
            CamY = 0;

            _tiles = new GameTile[Width, Height, Depth];
        }

        public void SetTile(int x, int y, int z, GameTile tile)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            {
                throw new IndexOutOfRangeException("Coordinates are outside the map boundaries.");
            }
            _tiles[x, y, z] = tile;
        }

        public GameTile GetTile(int x, int y, int z)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height || z < 0 || z >= Depth)
            {
                throw new IndexOutOfRangeException("Coordinates are outside the map boundaries.");
            }
            return _tiles[x, y, z];
        }

        public void RenderMap()
        {
            // The correct rendering order: iterate y, then x, then z. This ensures back-to-front rendering.
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    for (int z = 0; z < Depth; z++)
                    {
                        GameTile tile = GetTile(x, y, z);
                        if (tile != null && tile.Texture != null)
                        {
                            // Logical grid position of the tile's base
                            float gridX = (x - y) * (TileWidth / 2f);
                            float gridY = (x + y) * (TileHeight / 2f);

                            // The texture is 64x64. The floor part is in the bottom 32 pixels.
                            // The draw position needs to be offset to correctly align the visible part of the texture.
                            float drawX = gridX;
                            float drawY = gridY - (TileHeight / 2f);

                            // Apply Z-axis stacking offset
                            drawY -= z * TileHeight;

                            // Apply camera offset
                            drawX += CamX;
                            drawY += CamY;

                            // Draw the full 64x64 texture at the adjusted position
                            drawer.Image(tile.Texture, new Vector2(drawX, drawY), new Vector2(TileWidth,TileHeight*2), BlendMode.Alpha, tile.Color);
                        }
                    }
                }
            }
            drawer.Flush();
        }
    }
}