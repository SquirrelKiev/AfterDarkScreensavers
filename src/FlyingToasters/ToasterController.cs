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

        private const float maxToasters = 20f;
        private const float timeBetweenToasters = .5f;

        private const int toasterCollisionSize = 48;

        private static double lastToasterDepartureTime = 0;
        private static HashSet<Toaster> toasters = new HashSet<Toaster>();

        private static Sound explosionSound;

        public static Texture2D explosionTexture;

        public static Dictionary<Toaster.ToastType, Texture2D[]> toasterTextures = new Dictionary<Toaster.ToastType, Texture2D[]>();

        private static List<AnimationTracker> explosionAnimators = new List<AnimationTracker>();

        [Start]
        private static void Start()
        {
            explosionTexture = Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\explosion.png");

            explosionSound = Raylib.LoadSound($"{ReflectionUtility.AssemblyDirectory}\\Sounds\\snd_badexplosion.wav");

            toasterTextures[Toaster.ToastType.Toaster] = new Texture2D[]
            {
                Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\toaster.png")
            };
            toasterTextures[Toaster.ToastType.Toast] = new Texture2D[]
            {
                Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\toastlight.gif"),
                Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\toastwell.gif"),
                Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\toastverywell.gif"),
                Raylib.LoadTexture($"{ReflectionUtility.AssemblyDirectory}\\Sprites\\toastburnt.gif")
            };
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
            if (toasters.Count < maxToasters && lastToasterDepartureTime + timeBetweenToasters <= Raylib.GetTime())
            {
                int toasterX = 0;
                int toasterY = 0;

                bool unsafeSpawn = true;

                while (unsafeSpawn)
                {
                    unsafeSpawn = false;

                    double toastPosDecider = random.NextDouble() * 2;

                    if (toastPosDecider <= 1)
                    {
                        toasterX = (int)(Raylib.GetScreenWidth() * toastPosDecider);
                        toasterY = -80;
                    }
                    else
                    {
                        toasterX = Raylib.GetScreenWidth() + 10;
                        toasterY = (int)(Raylib.GetScreenHeight() * (toastPosDecider - 1));
                    }

                    int spawnCheckSize = toasterCollisionSize * 2;

                    foreach (Toaster toaster in toasters)
                    {
                        if (Raylib.CheckCollisionRecs(
                            new Rectangle(toasterX, toasterY, spawnCheckSize, spawnCheckSize),
                            new Rectangle(toaster.position.X, toaster.position.Y, spawnCheckSize, spawnCheckSize)
                            ))
                        {
                            Console.WriteLine($"failed against toaster at X {toaster.position.X} Y {toaster.position.Y}. My pos is X {toasterX} Y {toasterY}");
                            unsafeSpawn = true;
                        }
                    }
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

                if (toaster.position.X <= -64 || toaster.position.Y >= Raylib.GetScreenHeight())
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
                        new Rectangle(toaster.position.X, toaster.position.Y, toasterCollisionSize, toasterCollisionSize),
                        new Rectangle(toaster1.position.X, toaster1.position.Y, toasterCollisionSize, toasterCollisionSize)
                        ))
                    {
                        if (toastersToRemove == null)
                            toastersToRemove = new HashSet<Toaster>();

                        if (!toastersToRemove.Contains(toaster))
                            toastersToRemove.Add(toaster);

                        if (!toastersToRemove.Contains(toaster1))
                            toastersToRemove.Add(toaster1);

                        Raylib.PlaySound(explosionSound);
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
                toaster.Render();
            }

            List<AnimationTracker> animatorsToRemove = null;

            foreach (AnimationTracker animator in explosionAnimators)
            {
                if (animator.animationCompleted == false)
                {
                    animator.Render();
                }
                else
                {
                    if (animatorsToRemove == null)
                        animatorsToRemove = new List<AnimationTracker>();

                    animatorsToRemove.Add(animator);
                }
            }
        }
    }
}
