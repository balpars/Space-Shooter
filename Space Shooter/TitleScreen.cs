using SDL2;
using System;

namespace Space_Shooter
{
    public class TitleScreen
    {
        private IntPtr texture;
        private SDL.SDL_Rect destRect;

        public TitleScreen(string assetPath, IntPtr renderer)
        {
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }
        }

        public void Render(IntPtr renderer)
        {
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref destRect);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }

        public void SetFullScreen(int screenWidth, int screenHeight)
        {
            destRect = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = screenWidth,
                h = screenHeight
            };
        }
    }


    class StoryScreen : TitleScreen
    {
        private IntPtr texture;
        private SDL.SDL_Rect destRect;

        public StoryScreen(string assetPath, IntPtr renderer) : base(assetPath, renderer) { }
    }

}
