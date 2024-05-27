using SDL2;
using System;

namespace Space_Shooter
{
    public class TitleScreen
    {
        private IntPtr texture;
        private SDL.SDL_Rect rect;

        public TitleScreen(string path, IntPtr renderer)
        {
            this.texture = SDL_image.IMG_LoadTexture(renderer, path);
            if (this.texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {path}! SDL_Error: {SDL.SDL_GetError()}");
            }

            this.rect = new SDL.SDL_Rect { x = 0, y = 0, w = 800, h = 600 };
        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref rect);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}

