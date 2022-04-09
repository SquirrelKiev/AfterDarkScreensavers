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

        private static MethodInfo[] startMethods, renderMethods, preRenderMethods, postRenderMethods;

        private static void Main(string[] args)
        {
            startMethods = ReflectionUtility.GetMethodsWithAttribute<StartAttribute>();
            renderMethods = ReflectionUtility.GetMethodsWithAttribute<RenderAttribute>();
            preRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PreRenderAttribute>();
            postRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PostRenderAttribute>();

            Raylib.InitWindow(800, 600, "Screensaver");

            Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));

            ReflectionUtility.CallMethods(startMethods);

            while (!Raylib.WindowShouldClose())
            {
                DrawScreen();
            }
        }

        private static void DrawScreen()
        {
            ReflectionUtility.CallMethods(preRenderMethods);
            
            Raylib.BeginDrawing();
            {
                Raylib.ClearBackground(Color.BLACK);

                ReflectionUtility.CallMethods(renderMethods);
            }
            Raylib.EndDrawing();

            ReflectionUtility.CallMethods(postRenderMethods);
        }
    }
}
