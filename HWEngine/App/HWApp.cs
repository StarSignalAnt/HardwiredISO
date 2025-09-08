using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
namespace HWEngine.App
{
    public class HWApp : GameWindow
    {

        public HWApp(GameWindowSettings gws, NativeWindowSettings nws)
           : base(gws, nws)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Set clear color
            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

            // Enable alpha blending for 2D sprites
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // TODO: draw your 2D objects here

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            // Free GL resources (shaders, textures, VAOs, etc.)
        }


    }
}
