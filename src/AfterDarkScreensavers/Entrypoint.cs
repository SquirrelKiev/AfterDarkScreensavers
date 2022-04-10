using System;
using System.Reflection;
using Raylib_cs;
using System.Windows.Forms;

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
            // https://sites.harding.edu/fmccown/screensaver/screensaver.html
            if (args.Length > 0)
            {
                string firstArgument = args[0].ToLower().Trim();
                string secondArgument = null;

                // argument separated by colon handling
                if (firstArgument.Length > 2)
                {
                    // the magic number
                    secondArgument = firstArgument.Substring(3).Trim();
                    // /p /c /s
                    firstArgument = firstArgument.Substring(0, 2);
                }
                else if (args.Length > 1)
                    secondArgument = args[1];

                if (firstArgument == "/c") // Config mode
                {
                    ConfigMode();
                }
                else if (firstArgument == "/p") // Preview mode
                {
                    if (secondArgument == null)
                    {
                        Console.WriteLine("need a window handle");
                        return;
                    }
                    IntPtr previewHandle = new IntPtr(long.Parse(secondArgument));
                    PreviewMode(previewHandle);
                }
                else if (firstArgument == "/s") // Screensaver mode/Full screen mode
                {
                    ScreensaverMode();
                }
                else // Stinky argument we dont know of
                {
                    Console.WriteLine($"{firstArgument} is not valid.");
                }
            }
            else // run as screensaver
            {
                ConfigMode();
            }
        }

        private static void ConfigMode()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Config.ConfigForm());
        }

        private static void PreviewMode(IntPtr previewWindowHandle)
        {
            Raylib.InitWindow(0, 0, "\"Preview\"");

            IntPtr windowHandle;

            unsafe
            {
                windowHandle = new IntPtr(Raylib.GetWindowHandle());
            }

            // Set the preview window as parent of this window
            User32Methods.SetParent(windowHandle, previewWindowHandle);

            // Make this a child window so it will close when the parent dialog closes
            // GWL_STYLE = -16, WS_CHILD = 0x40000000
            User32Methods.SetWindowLong(windowHandle, -16, new IntPtr(User32Methods.GetWindowLong(windowHandle, -16) | 0x40000000));

            // Place our window inside the parent
            System.Drawing.Rectangle parentRect;
            User32Methods.GetClientRect(previewWindowHandle, out parentRect);
            Raylib.SetWindowSize(parentRect.Size.Width, parentRect.Size.Height);
            Raylib.SetWindowPosition(0, 0);

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                {
                    Raylib.ClearBackground(Color.BLACK);
                    Raylib.DrawText($"we dont do previews here", 20, parentRect.Size.Height / 2, 6, Color.WHITE);
                }
                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        private static void ScreensaverMode(IntPtr previewWindowHandle = new IntPtr())
        {
            startMethods = ReflectionUtility.GetMethodsWithAttribute<StartAttribute>();
            renderMethods = ReflectionUtility.GetMethodsWithAttribute<RenderAttribute>();
            preRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PreRenderAttribute>();
            postRenderMethods = ReflectionUtility.GetMethodsWithAttribute<PostRenderAttribute>();
            beforeCloseMethods = ReflectionUtility.GetMethodsWithAttribute<BeforeCloseAttribute>();

            Raylib.InitWindow(0, 0, "Screensaver");

            Raylib.ToggleFullscreen();
            Raylib.DisableCursor();

            Raylib.SetTargetFPS(Raylib.GetMonitorRefreshRate(Raylib.GetCurrentMonitor()));

            Raylib.InitAudioDevice();

            ReflectionUtility.CallMethods(startMethods);

            bool firstFramePassed = false;

            while (!Raylib.WindowShouldClose())
            {
                DrawScreen();

                var mouseDelta = Raylib.GetMouseDelta();

                if (firstFramePassed && mouseDelta.X > 5 || mouseDelta.Y > 5)
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
