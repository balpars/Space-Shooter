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
        public int Speed { get; private set; } = 3;

        public Enemy(int x, int y, int size, IntPtr renderer, int points, int hitLifetime = 200) : base(x, y, size, size)
        {
            this.renderer = renderer;
            this.points = points;
            isHit = false;
            this.hitLifetime = hitLifetime;
            this.hitLifetimeRemaining = hitLifetime;

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
        }

        public override void Update()
        {
            if (isHit)
            {
                hitLifetimeRemaining -= 16; // Assuming Update is called every ~16ms (60 FPS)
                if (hitLifetimeRemaining <= 0)
                {
                    // Remove the enemy when the hit lifetime expires
                    isHit = false; // Mark for removal
                    rect.x = -1000; // Move it out of screen to ensure it is not rendered
                }
            }
            else
            {
                Move(0, Speed);
            }
        }

        public override void Render(IntPtr renderer)
        {
            if (hitLifetimeRemaining > 0)
            {
                IntPtr currentTexture = isHit ? hitTexture : texture;
                SDL.SDL_RenderCopy(renderer, currentTexture, IntPtr.Zero, ref rect);
            }
        }

        public void OnHit()
        {
            isHit = true;
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
        public BasicEnemy(int x, int y, int size, IntPtr renderer) : base(x, y, size, renderer, 100)
        {
        }
    }
}
