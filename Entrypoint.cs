using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Raylib_cs;

namespace AfterDarkScreensavers
{
    public static class Entrypoint
    {
        private static string farts;

        private static MethodInfo[] startMethods, renderMethods;

        private static void Main(string[] args)
        {
            startMethods = ReflectionUtility.GetMethodsWithAttribute<StartAttribute>();
            renderMethods = ReflectionUtility.GetMethodsWithAttribute<RenderAttribute>();

            Raylib.InitWindow(640, 480, "Screensaver");

            foreach(var startMethod in startMethods)
            {
                startMethod.Invoke(null, null);
            }

            while (!Raylib.WindowShouldClose())
            {
                DrawScreen();
            }
        }

        private static void DrawScreen()
        {
            Raylib.BeginDrawing();
            {
                Raylib.ClearBackground(Color.BLACK);

                foreach (var updateMethod in renderMethods)
                {
                    updateMethod.Invoke(null, null);
                }
            }
            Raylib.EndDrawing();
        }
    }
}
