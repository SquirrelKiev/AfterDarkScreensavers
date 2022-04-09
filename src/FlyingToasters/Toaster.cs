using System;
using System.Numerics;

namespace AfterDarkScreensavers.FlyingToasters
{
    internal class Toaster
    {
        private static Random random = new Random();

        public const int minSpeed = 30;
        public const int maxSpeed = 90;

        public Vector2 position;
        public int speed;

        public Toaster(Vector2 startingPosition)
        {
            speed = random.Next(minSpeed, maxSpeed);
            position = startingPosition;
        }
    }
}
