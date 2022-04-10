using System;
using System.Reflection;
using Raylib_cs;

namespace AfterDarkScreensavers
{
    public static class Entrypoint
    {
        /// <summary>
        /// Best to set this on PreRender as the background is cleared/set to this every frame. 
        /// </summary>
        public static Color BackgroundColor { get; set; } = Color.BLACK;

        private static MethodInfo[] startMethods, renderMethods, preRenderMethods, postRenderMethods, beforeCloseMethods;

        private static void Main(string[] args)
        {
            startMethods = ReflectionUtility.GetMethodsWithAttribute<StartAttribute>();
            renderMethods = ReflectionUtility.GetMethodsWithAttribute<RenderAttribute>();
            preRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PreRenderAttribute>();
            postRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PostRenderAttribute>();
            beforeCloseMethods = ReflectionUtility.GetMethodsWithAttribute<BeforeCloseAttribute>();

            Raylib.InitWindow(800, 600, "Screensaver");

            Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));

            Raylib.InitAudioDevice();

            ReflectionUtility.CallMethods(startMethods);

            bool firstFramePassed = false;

            while (!Raylib.WindowShouldClose())
            {
                DrawScreen();

                if (Raylib.GetMouseDelta() != System.Numerics.Vector2.Zero && firstFramePassed == true)
                    break;

                firstFramePassed = true;
            }

            ReflectionUtility.CallMethods(beforeCloseMethods);
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }

        private static void DrawScreen()
        {
            ReflectionUtility.CallMethods(preRenderMethods);
            
            Raylib.BeginDrawing();
            {
                Raylib.ClearBackground(BackgroundColor);

                ReflectionUtility.CallMethods(renderMethods);
            }
            Raylib.EndDrawing();

            ReflectionUtility.CallMethods(postRenderMethods);
        }
    }
}
