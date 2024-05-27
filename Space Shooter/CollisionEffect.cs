using SDL2;
using System;

namespace Space_Shooter
{
    public class CollisionEffect : GameObject
    {
        private IntPtr texture;
        private SDL.SDL_Rect srcRect;
        private SDL.SDL_Rect destRect;
        private int duration;
        private uint startTime;

        public CollisionEffect(int x, int y, int size, string assetPath, int duration, IntPtr renderer) : base(x, y, size, size)
        {
            this.texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (this.texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }

            this.duration = duration;
            this.startTime = SDL.SDL_GetTicks();

            this.srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 35, h =35  };
            this.destRect = new SDL.SDL_Rect { x = x, y = y, w = 35, h = 35 };
        }

        public override void Update()
        {
            // No specific update logic for collision effects in this case
        }

        public bool IsActive()
        {
            return SDL.SDL_GetTicks() - startTime < duration;
        }

        public void Render(IntPtr renderer)
        {
            if (IsActive())
            {
                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
