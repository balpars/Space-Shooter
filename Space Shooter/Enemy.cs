using SDL2;
using System;

namespace Space_Shooter
{
    public class Enemy : GameObject
    {
        protected string? assetPath;
        protected IntPtr renderer;
        protected IntPtr texture;
        private bool isHit;
        private string hitAssetPath;
        private IntPtr hitTexture;
        private int points;
        private int hitLifetime;
        private int hitLifetimeRemaining;
        private bool isFlashing;
        private int flashCount;
        private uint lastFlashTime;
        private uint flashInterval;
        private bool isVisible;
        protected Game game;

        public int SpeedX { get; protected set; }
        public int SpeedY { get; protected set; }

        public Enemy(int x, int y, int size, IntPtr renderer, int points, int speedX, int speedY, Game game)
            : base(x, y, size, size)
        {
            this.renderer = renderer;
            this.points = points;
            this.game = game;
            isHit = false;
            this.hitLifetime = 500;
            this.hitLifetimeRemaining = hitLifetime;
            isFlashing = false;
            flashCount = 0;
            lastFlashTime = 0;
            flashInterval = 500;
            isVisible = true;



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

        public void SetSpeedY(int speedY)
        {
            SpeedY = speedY;
        }

        public override void Update()
        {
            if (isHit)
            {
                hitLifetimeRemaining -= 16;
                uint currentTime = SDL.SDL_GetTicks();

                if (currentTime > lastFlashTime + flashInterval)
                {
                    isVisible = !isVisible;
                    flashCount++;
                    lastFlashTime = currentTime;
                }

                if (flashCount >= 6)
                {
                    isVisible = false;
                    isFlashing = false;
                    hitLifetimeRemaining = 0;
                }

                if (hitLifetimeRemaining <= 0)
                {
                    this.Cleanup();
                }
            }
            else
            {
                // Ensure enemy stays within screen bounds
                if (rect.x + SpeedX < 0 || rect.x + SpeedX + rect.w > game.windowWidth)
                {
                    SpeedX = -SpeedX;
                }
                Move(SpeedX, SpeedY);
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

        public virtual void OnHit()
        {
            isHit = true;
            isFlashing = true;
            flashCount = 0;
            lastFlashTime = SDL.SDL_GetTicks();
            hitLifetimeRemaining = hitLifetime;
        }

        public bool IsHit()
        {
            return isHit;
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
        public SDL.SDL_Rect GetCollisionRect()
        {
            // Return a smaller rectangle for collision detection
            int collisionWidth = rect.w/3;
            int collisionHeight = rect.h/2;
            return new SDL.SDL_Rect
            {
                x = rect.x + (rect.w - collisionWidth) / 2,
                y = rect.y + (rect.h - collisionHeight) / 2,
                w = collisionWidth,
                h = collisionHeight
            };
        }
    }
   

    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int size, IntPtr renderer, int speedX, int speedY, Game game)
            : base(x, y, size, renderer, 100, speedX, speedY, game)
        {
        }

        public override void OnHit()
        {
            {
                game.IncreaseScore(100);
                base.OnHit();
            }
        }
        
       

    }
}
