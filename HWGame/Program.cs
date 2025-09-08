using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

using HWGame.Game;

namespace HWGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to HardWired. Booting...");

            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings
            {
                Size = new Vector2i(800, 600),
                Title = "OpenTK 5 Preview - My 2D Game"
            };

            using (var game = new HardWired(gws, nws))
                game.Run();
        }

    }



}

