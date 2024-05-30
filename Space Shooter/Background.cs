using SDL2;
using System;

namespace Space_Shooter
{
    public class Background
    {
        private IntPtr texture;
        private SDL.SDL_Rect srcRect;
        private SDL.SDL_Rect destRect1;
        private SDL.SDL_Rect destRect2;
        private int speed;
        private int screenWidth;
        private int screenHeight;

        public Background(string assetPath, IntPtr renderer, int speed, int screenWidth, int screenHeight)
        {
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }

            this.speed = speed;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = screenWidth, h = screenHeight };
            destRect1 = new SDL.SDL_Rect { x = 0, y = 0, w = screenWidth, h = screenHeight };
            destRect2 = new SDL.SDL_Rect { x = 0, y = -screenHeight, w = screenWidth, h = screenHeight };
        }

        public void Update()
        {
            // Move the backgrounds down by speed
            destRect1.y += speed;
            destRect2.y += speed;

            // If the first background moved fully below the screen, reset its position
            if (destRect1.y >= screenHeight)
            {
                destRect1.y = destRect2.y - screenHeight;
            }

            // If the second background moved fully below the screen, reset its position
            if (destRect2.y >= screenHeight)
            {
                destRect2.y = destRect1.y - screenHeight;
            }
        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect1);
            SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect2);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
