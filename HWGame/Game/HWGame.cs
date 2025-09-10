using HWEngine.App;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using HWEngine.Draw;
using HWEngine.Texture;
using HWEngine.Map;

namespace HWGame.Game
{
    public class HardWired : HWApp
    {
        Texture2D tex1;
        Texture2D tex2;
        Drawer _Draw;

        GameMap map1;

        List<GameTile> Tiles = new List<GameTile>();

        public HardWired(GameWindowSettings gws, NativeWindowSettings nws)
           : base(gws, nws)
        {
        }

        List<Texture2D> tex;

        public override void InitApp()
        {
            Console.WriteLine("HardWired Game Initialized");
            _Draw = new Drawer(800,600);
            tex1 = new Texture2D("Test/img1.png");
            tex2 = new Texture2D("Test/img2.png");
            int b = 5;

            //HWEngine.Map.TileSetLoader tload = new HWEngine.Map.TileSetLoader();
            
            tex = TileSetLoader.LoadTileSet("test/tiles3.png",64,64);

            foreach(var t in tex)
            {
                Tiles.Add(new GameTile(t, TileType.Ground, new Vector4(1f, 1f, 1f, 1f)));
            }


            map1 = new GameMap(12, 12, 3,64,32);

            for(int y = 0; y < 12; y++)
            {
                for(int x = 0; x < 12; x++)
                {
                    var tile = Tiles[2];
                    map1.SetTile(x, y, 0, tile);
                }
            }
            map1.SetTile(2, 2,0, Tiles[0]);
            map1.SetTile(3, 2, 0, Tiles[0]);
            map1.SetTile(4, 2, 0, Tiles[0]);

            map1.SetTile(2, 4, 0, Tiles[0]);
            map1.SetTile(3, 5, 0, Tiles[0]);


        }

        Random rnd = new Random();

        public override void RenderApp()
        {


            map1.CamX = 450;
            map1.CamY = 100;

            map1.RenderMap();

            return;
            int x = 5;


            bool im = false;


            int dx = 32;
            int dy = 32;
            foreach (var t in tex)
            {


                _Draw.Image(t,new Vector2(dx,dy),new Vector2(128,128),BlendMode.Alpha,new Vector4(1f,1f,1f,1f));

                dx = dx + 128;
                if (dx >= 800 - 128)
                {
                    dx = 32;
                    dy = dy + 128;
                }

            }


            //_Draw.Flush();

        }



    }
}
