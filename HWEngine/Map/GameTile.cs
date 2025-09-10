using HWEngine.Texture;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;
using HWEngine.Draw;
namespace HWEngine.Map
{
    public enum TileType
    {
        Ground,
        Wall,
        Floor,
        Object
    }

    public class GameTile
    {
        public Texture2D Texture { get; private set; }
        public Vector4 Color { get; private set; }
        public TileType Type { get; private set; }

        public GameTile(Texture2D texture, TileType type, Vector4 color)
        {
            Texture = texture;
            Type = type;
            Color = color;
        }
        public GameTile()
        {

            Texture = null;
            Color = new Vector4(1f, 1f, 1f, 1f);
            Type = TileType.Ground;

        }

    }
}
