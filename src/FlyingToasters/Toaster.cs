using System;
using System.Numerics;

namespace AfterDarkScreensavers.FlyingToasters
{
    internal class Toaster
    {
        private static Random random = new Random();

        public const int minSpeed = 60;
        public const int maxSpeed = 90;

        public Vector2 position;
        public int speed;

        public AnimationTracker tracker { get; private set; }

        public Toaster(Vector2 startingPosition)
        {
            speed = random.Next(minSpeed, maxSpeed);
            position = startingPosition;
            tracker = new AnimationTracker(ToasterController.toasterTextures[0], 4, position, true, 1 / (speed * .1f));
        }

        public void Render()
        {
            tracker.position = position;
            tracker.Render();
        }
    }
}
