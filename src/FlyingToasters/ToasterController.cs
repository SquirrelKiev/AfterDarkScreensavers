using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace AfterDarkScreensavers.FlyingToasters
{
    internal static class ToasterController
    {
        private static Random random = new Random();

        private const float timeBetweenToasters = 2f;

        private static double lastToasterDepartureTime = 0;
        private static List<Toaster> toasters = new List<Toaster>();

        [Render]
        private static void Render()
        {
            if (lastToasterDepartureTime + timeBetweenToasters <= Raylib.GetTime())
            {
                double toastPosDecider = random.NextDouble() * 2;

                int toasterX;
                int toasterY;

                if (toastPosDecider <= 1)
                {
                    toasterX = (int)(Raylib.GetScreenWidth() / 2 * toastPosDecider) + Raylib.GetScreenWidth() / 2;
                    toasterY = -10;
                }
                else
                {
                    toasterX = Raylib.GetScreenWidth() + 10;
                    toasterY = (int)(Raylib.GetScreenHeight() / 2 * (toastPosDecider - 1));
                }

                Console.WriteLine($"Toaster departed at {Raylib.GetTime()} X: {toasterX} Y: {toasterY}");

                toasters.Add(new Toaster(new Vector2(toasterX, toasterY)));

                lastToasterDepartureTime = Raylib.GetTime();
            }


            List<Toaster> toastersToRemove = null;

            Raylib.DrawText($"{Math.Round(Raylib.GetTime() - lastToasterDepartureTime, 3)}", 10, 10, 36, Color.GREEN);

            foreach (Toaster toaster in toasters)
            {
                Raylib.DrawRectangle((int)toaster.position.X, (int)toaster.position.Y, 25, 25, Color.RAYWHITE);

                toaster.position.X -= toaster.speed * Raylib.GetFrameTime();
                toaster.position.Y += toaster.speed * 0.5f * Raylib.GetFrameTime();

                if (toaster.position.X <= -25 || toaster.position.Y >= Raylib.GetScreenHeight())
                {
                    Console.WriteLine($"Toaster sent to landfill at {Raylib.GetTime()}");
                    if(toastersToRemove == null)
                        toastersToRemove = new List<Toaster>();

                    toastersToRemove.Add(toaster);
                }
            }

            if(toastersToRemove != null)
            {
                foreach (Toaster toaster in toastersToRemove)
                {
                    toasters.Remove(toaster);
                }
            }
        }
    }
}
