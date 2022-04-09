using System;
using System.Numerics;
using Raylib_cs;

namespace AfterDarkScreensavers
{
    internal class AnimationTracker
    {
        public Vector2 position = Vector2.Zero;
        public Texture2D spritesheet { get; private set; }
        public bool loop { get; private set; } = false;
        public bool animationCompleted { get; private set; } = false;
        public float timeBetweenFrames = .1f;

        private float frameWidth
        {
            get
            {
                return spritesheet.width / totalFrames;
            }
        }

        private float lastFrameRendered = 0f;
        private int frame = 0;
        private int totalFrames = 1;

        public AnimationTracker(Texture2D spritesheet, int totalFrames, Vector2 startingPosition, bool loop = true, float timeBetweenFrames = 0.05f)
        {
            this.spritesheet = spritesheet;
            this.totalFrames = totalFrames;
            this.loop = loop;
            this.timeBetweenFrames = timeBetweenFrames;
            position = startingPosition;
        }

        public void Render()
        {
            //Raylib.DrawText($"{frame}", 20, 20, 36, Color.GREEN);

            if (lastFrameRendered + timeBetweenFrames <= Raylib.GetTime())
            {
                frame++;
                if (loop == false && frame > totalFrames - 1)
                {
                    animationCompleted = true;
                    return;
                }

                lastFrameRendered = (float)Raylib.GetTime();
            }

            Raylib.DrawTextureRec(spritesheet, new Rectangle(frameWidth * frame, 0, frameWidth, spritesheet.height), position, Color.WHITE);
        }
    }
}
