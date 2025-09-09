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
            int b = 5;
        }

        public override void RenderApp()
        {
            
            _Draw.Image(tex1, new Vector2(20, 20), new Vector2(128, 128),BlendMode.Solid, new Vector4(1, 1, 1, 1));
            _Draw.Rect(new Vector2(20, 5), new Vector2(128, 128), BlendMode.Additive, new Vector4(1.0f, 0.0f, 0, 1.0f));

        }



    }
}
