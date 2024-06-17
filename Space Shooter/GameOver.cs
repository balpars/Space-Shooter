using SDL2;
using System;

namespace Space_Shooter
{
    public class GameOver
    {
        private IntPtr texture;
        private SDL.SDL_Rect destRect;

        public GameOver(string assetPath, IntPtr renderer, int screenWidth, int screenHeight)
        {
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }

            // Set the destination rectangle to cover the entire screen
            destRect = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = screenWidth,
                h = screenHeight
            };
        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destRect);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
