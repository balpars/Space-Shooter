// File: Enemy.cs

using SDL2;
using System;

namespace Space_Shooter
{
    public class Enemy : GameObject
    {
        protected string? assetPath; // Path to the texture asset for the enemy
        private IntPtr renderer;
        private IntPtr texture;
        private bool isHit;
        private string hitAssetPath;
        private IntPtr hitTexture;
        private int points;
        private int hitLifetime; // Enemy hit lifetime in ticks
        private int hitLifetimeRemaining;
        private bool isFlashing;
        private int flashCount;
        private uint lastFlashTime;
        private uint flashInterval;
        private bool isVisible;
        private int screenWidth;

        public int SpeedX { get; private set; }
        public int SpeedY { get; private set; }

        public Enemy(int x, int y, int size, IntPtr renderer, int points, int speedX, int speedY, int screenWidth, int hitLifetime = 1000) : base(x, y, size, size)
        {
            this.renderer = renderer;
            this.points = points;
            isHit = false;
            this.hitLifetime = hitLifetime;
            this.hitLifetimeRemaining = hitLifetime;
            isFlashing = false;
            flashCount = 0;
            lastFlashTime = 0;
            flashInterval = 500; // milliseconds
            isVisible = true;
            this.screenWidth = screenWidth;

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"Failed to initialize SDL_image! SDL_image Error: {SDL.SDL_GetError()}");
            }

            assetPath = "Assets/Enemy/enemy.png";
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }

            hitAssetPath = "Assets/Enemy/hit_enemy.png";
            hitTexture = SDL_image.IMG_LoadTexture(renderer, hitAssetPath);
            if (hitTexture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load hit texture {hitAssetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }

            SpeedX = speedX;
            SpeedY = speedY;
        }

        public override void Update()
        {
            if (isHit)
            {
                hitLifetimeRemaining -= 16; // Assuming Update is called every ~16ms (60 FPS)
                uint currentTime = SDL.SDL_GetTicks();

                if (currentTime > lastFlashTime + flashInterval)
                {
                    isVisible = !isVisible;
                    flashCount++;
                    lastFlashTime = currentTime;
                }

                if (flashCount >= 6) // Flash three times (6 visibility changes)
                {
                    isVisible = false; // Finally hide the enemy
                    isFlashing = false;
                    hitLifetimeRemaining = 0;
                }

                if (hitLifetimeRemaining <= 0)
                {
                    rect.x = -1000; // Move it out of screen to ensure it is not rendered
                }
            }
            else
            {
                Move(SpeedX, SpeedY); // Move diagonally

                // Ensure the enemy stays within the screen bounds horizontally
                if (rect.x < 0 || rect.x + rect.w > screenWidth)
                {
                    SpeedX = -SpeedX; // Reverse direction
                }
            }
        }

        public override void Render(IntPtr renderer)
        {
            if (isVisible)
            {
                IntPtr currentTexture = isHit ? hitTexture : texture;
                SDL.SDL_RenderCopy(renderer, currentTexture, IntPtr.Zero, ref rect);
            }
        }

        public void OnHit()
        {
            isHit = true;
            isFlashing = true;
            flashCount = 0;
            lastFlashTime = SDL.SDL_GetTicks();
            hitLifetimeRemaining = hitLifetime; // Reset the lifetime when hit
        }

        public bool IsHit()
        {
            return isHit;
        }

        public bool IsExpired()
        {
            return !isHit && rect.x == -1000;
        }

        public int GetPoints()
        {
            return points;
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
            SDL.SDL_DestroyTexture(hitTexture);
        }
    }

    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int size, IntPtr renderer, int speedX, int speedY, int screenWidth)
            : base(x, y, size, renderer, 100, speedX, speedY, screenWidth)
        {
        }
    }
}
