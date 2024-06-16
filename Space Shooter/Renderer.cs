// File: Renderer.cs

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
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 68, h = 54 };
                SDL.SDL_Rect destRect = obj.GetRect();
                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else if (obj is Player player && player.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(player.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 100, h = 100 };
                SDL.SDL_Rect destRect;

                // Check if the current asset is the protected player asset
                if (player.GetAssetPath().Contains("protected_player.png"))
                {
                    // Render the protected player asset at 60x60 size
                    destRect = new SDL.SDL_Rect { x = player.PositionX+20, y = player.PositionY, w = 60, h = 60 };
                }
                else
                {
                    // Render other player assets at their original size
                    destRect = new SDL.SDL_Rect { x = player.PositionX, y = player.PositionY, w = 100, h = 100 };
                }

                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else if (obj is Projectile projectile && projectile.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(projectile.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 100, h = 100 };
                SDL.SDL_Rect destRect = obj.GetRect();
                // Ensure the projectile texture is scaled correctly
                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else if (obj is Heart heart && heart.GetAssetPath() != null)
            {
                IntPtr texture = TextureManager.LoadTexture(heart.GetAssetPath(), renderer);
                SDL.SDL_Rect srcRect = new SDL.SDL_Rect { x = 0, y = 0, w = 100, h = 100 };
                SDL.SDL_Rect destRect = obj.GetRect();
                // Ensure the heart texture is scaled correctly
                SDL.SDL_RenderCopy(renderer, texture, ref srcRect, ref destRect);
            }
            else // If there are no assets, draw red rectangles
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

        public static void RenderText(IntPtr renderer, IntPtr font, string text, int x, int y)
        {
            SDL.SDL_Color color = new SDL.SDL_Color { r = 255, g = 255, b = 255, a = 255 };
            IntPtr surface = SDL_ttf.TTF_RenderText_Solid(font, text, color);
            IntPtr texture = SDL.SDL_CreateTextureFromSurface(renderer, surface);

            SDL.SDL_QueryTexture(texture, out _, out _, out int width, out int height);
            SDL.SDL_Rect dstRect = new SDL.SDL_Rect { x = x, y = y, w = width, h = height };

            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, ref dstRect);

            SDL.SDL_FreeSurface(surface);
            SDL.SDL_DestroyTexture(texture);
        }

        public static void RenderTextLetterByLetter(IntPtr renderer, IntPtr font, string text, int x, int y, ref string currentText, ref int currentIndex, int delay, ref uint lastTime)
        {
            uint currentTime = SDL.SDL_GetTicks();
            if (currentTime - lastTime >= delay && currentIndex < text.Length)
            {
                currentText += text[currentIndex];
                currentIndex++;
                lastTime = currentTime;
            }

            RenderText(renderer, font, currentText, x, y);
        }


    }
}
