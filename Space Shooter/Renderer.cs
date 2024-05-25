using SDL2;
using System;

namespace Space_Shooter
{
    public class Renderer
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
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
            SDL.SDL_RenderClear(renderer);
        }

        public void DrawBackground(Background bg)
        {
            bg.Render(renderer);
        }

        public void Draw(GameObject obj)
        {
            if (obj is Enemy enemy && enemy.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(enemy.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = obj.GetRect().w, h = obj.GetRect().h };
                SDL.SDL_Rect destRect = obj.GetRect();
                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else if (obj is Player player && player.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(player.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 100, h = 100 };
                SDL.SDL_Rect destRect = new SDL.SDL_Rect { x = player.PositionX, y = player.PositionY, w = 100, h = 100 };
                // TO-DO Make w and h size of asset instead of hardcoding

                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else if (obj is Projectile projectile && projectile.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(projectile.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 100, h = 100 };
                SDL.SDL_Rect destRect = obj.GetRect();
                // TO-DO Make w and h size of asset instead of hardcoding

                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else // If There are no assets draw Red rectangles
            {
                SDL.SDL_Rect rect = obj.GetRect();
                SDL.SDL_SetRenderDrawColor(renderer, 0xFF, 0x00, 0x00, 0xFF);
                SDL.SDL_RenderFillRect(renderer, ref rect);
            }
        }


        public void Present()
        {
            SDL.SDL_RenderPresent(renderer);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyRenderer(renderer);
        }

        public IntPtr GetRenderer()
        {
            return this.renderer;
        }

    }
}
