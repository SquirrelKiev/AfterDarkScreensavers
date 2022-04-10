using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Raylib_cs;

namespace AfterDarkScreensavers.FlyingToasters
{
    internal static class ToasterController
    {
        private static Random random = new Random();

        private const float timeBetweenToasters = 2f;

        private static double lastToasterDepartureTime = 0;
        private static HashSet<Toaster> toasters = new HashSet<Toaster>();

        private static Sound explosionSound;

        private static Texture2D explosionTexture;
        private static List<AnimationTracker> explosionAnimators = new List<AnimationTracker>();

        [Start]
        private static void Start()
        {
            explosionTexture = Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\LazyExplosion\\explosion.png");

            explosionSound = Raylib.LoadSound($"{ReflectionUtility.AssemblyDirectory}\\Sounds\\snd_badexplosion.wav");
        }

        [BeforeClose]
        private static void BeforeClose()
        {
            Raylib.UnloadSound(explosionSound);
            Raylib.UnloadTexture(explosionTexture);
        }

        [PreRender]
        private static void PreRender()
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


            HashSet<Toaster> toastersToRemove = null;

            foreach (Toaster toaster in toasters)
            {
                toaster.position.X -= toaster.speed * Raylib.GetFrameTime();
                toaster.position.Y += toaster.speed * 0.5f * Raylib.GetFrameTime();

                if (toaster.position.X <= -25 || toaster.position.Y >= Raylib.GetScreenHeight())
                {
                    Console.WriteLine($"Toaster sent to landfill at {Raylib.GetTime()}");
                    if (toastersToRemove == null)
                        toastersToRemove = new HashSet<Toaster>();

                    toastersToRemove.Add(toaster);
                }

                // Not optimal but shouldn't be an issue
                foreach (Toaster toaster1 in toasters)
                {
                    if (toaster1 == toaster)
                        continue;

                    if (Raylib.CheckCollisionRecs(
                        new Rectangle(toaster.position.X, toaster.position.Y, 25, 25),
                        new Rectangle(toaster1.position.X, toaster1.position.Y, 25, 25)
                        ))
                    {
                        if (toastersToRemove == null)
                            toastersToRemove = new HashSet<Toaster>();

                        if (!toastersToRemove.Contains(toaster))
                            toastersToRemove.Add(toaster);

                        if (!toastersToRemove.Contains(toaster1))
                            toastersToRemove.Add(toaster1);

                        Raylib.PlaySoundMulti(explosionSound);
                        explosionAnimators.Add(new AnimationTracker(explosionTexture, 17, toaster.position, false));
                    }
                }
            }

            if (toastersToRemove != null)
            {
                foreach (Toaster toaster in toastersToRemove)
                {
                    toasters.Remove(toaster);
                }
            }
        }

        [Render]
        private static void Render()
        {
            foreach (Toaster toaster in toasters)
            {
                Raylib.DrawRectangle((int)toaster.position.X, (int)toaster.position.Y, 25, 25, Color.RAYWHITE);
            }

            List<AnimationTracker> animatorsToRemove = null;

            foreach(AnimationTracker animator in explosionAnimators)
            {
                if(animator.animationCompleted == false)
                {
                    animator.Render();
                }
                else
                {
                    if(animatorsToRemove == null)
                        animatorsToRemove = new List<AnimationTracker>();

                    animatorsToRemove.Add(animator);
                }
            }
        }
    }
}
