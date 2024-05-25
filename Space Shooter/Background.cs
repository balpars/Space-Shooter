using SDL2;
using System;

namespace Space_Shooter
{
    public class Background
    {
        private SDL.SDL_Rect rect;
        private IntPtr texture;
        private int speed;

        public Background(string path, IntPtr renderer, int speed)
        {
            this.texture = SDL_image.IMG_LoadTexture(renderer, path);
            if (this.texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {path}! SDL_Error: {SDL.SDL_GetError()}");
            }

            this.rect = new SDL.SDL_Rect { x = 0, y = 0, w = 800, h = 600 };
            this.speed = speed;
        }

        public void Update()
        {
            rect.y += speed;
            if (rect.y >= 600)
            {
                rect.y = 0;
            }
        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref rect);

            SDL.SDL_Rect secondRect = new SDL.SDL_Rect { x = rect.x, y = rect.y - rect.h, w = rect.w, h = rect.h };
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref secondRect);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
