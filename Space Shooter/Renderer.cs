using SDL2;
using System;

namespace Space_Shooter
{
    class Renderer
    {
        private IntPtr renderer;

        public IntPtr RendererHandle
        {
            get { return renderer; }
        }

        public void Init(IntPtr window)
        {
            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (renderer == IntPtr.Zero)
            {
                Console.WriteLine($"Renderer could not be created! SDL_Error: {SDL.SDL_GetError()}");
                SDL.SDL_DestroyWindow(window);
                SDL.SDL_Quit();
            }
        }

        public void Clear()
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 139, 255);
            SDL.SDL_RenderClear(renderer);
        }

        public void DrawBackground(Background bg)
        {
            bg.Render(renderer);
        }

        public void Draw(GameObject obj)
        {
            SDL.SDL_Rect rect = obj.GetRect();
            SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);
            SDL.SDL_RenderFillRect(renderer, ref rect);
        }

        public void Present()
        {
            SDL.SDL_RenderPresent(renderer);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyRenderer(renderer);
        }
    }
}
