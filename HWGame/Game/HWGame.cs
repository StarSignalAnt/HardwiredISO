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

namespace HWGame.Game
{
    public class HardWired : HWApp
    {
        Texture2D tex1;
        Texture2D tex2;
        Drawer _Draw;

        public HardWired(GameWindowSettings gws, NativeWindowSettings nws)
           : base(gws, nws)
        {
        }

        public override void InitApp()
        {
            Console.WriteLine("HardWired Game Initialized");
            _Draw = new Drawer(800,600);
            tex1 = new Texture2D("Test/img1.png");
            tex2 = new Texture2D("Test/img2.png");
            int b = 5;

        }

        Random rnd = new Random();

        public override void RenderApp()
        {


            int x = 5;


            bool im = false;

            for (int i = 0; i < 10; i++)
            {


                //int x = rnd.Next(0, 800);
                //int y = rnd.Next(0, 600);
                int y = 40;

                Texture2D dt = tex1;
                if (im)
                {
                    dt = tex1;
                }
                else
                {
                    dt = tex2;
                }
                    _Draw.Image(dt, new Vector2(x, y), new Vector2(128, 128), BlendMode.Alpha, new Vector4(1, 1, 1, 1));
                //            _Draw.Rect(new Vector2(20, 5), new Vector2(128, 128), BlendMode.Additive, new Vector4(1.0f, 0.0f, 0, 1.0f));
                x = x + 64;
                im = im ? false : true;
            }

            _Draw.Flush();

        }



    }
}
