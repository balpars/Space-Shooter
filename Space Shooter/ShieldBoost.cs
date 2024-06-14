// File: ShieldBoost.cs

using SDL2;
using System;

namespace Space_Shooter
{
    public class ShieldBoost : GameObject
    {
        private IntPtr renderer;
        private IntPtr texture;
        private int speed;
        private double angle;

        public ShieldBoost(int x, int y, int size, IntPtr renderer, int speed) : base(x, y, size, size)
        {
            this.renderer = renderer;
            this.speed = speed;
            this.angle = 0;

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"Failed to initialize SDL_image! SDL_image Error: {SDL.SDL_GetError()}");
            }

            string assetPath = "Assets/ShieldBoost/shield_boost.png";
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }
        }

        public override void Update()
        {
            Move(0, speed);
            angle += 2;
            if (angle > 360) angle -= 360;
        }

        public override void Render(IntPtr renderer)
        {
            SDL.SDL_Point center = new SDL.SDL_Point { x = rect.w / 2, y = rect.h / 2 };
            SDL.SDL_RenderCopyEx(renderer, texture, IntPtr.Zero, ref rect, angle, ref center, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }

        public void Cleanup()
        {
            SDL.SDL_DestroyTexture(texture);
        }
    }
}
