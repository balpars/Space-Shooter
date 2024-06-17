using SDL2;
using System;

namespace Space_Shooter
{
    public class HealthBoost : GameObject
    {
        private IntPtr renderer;
        private IntPtr texture;
        private int speed;
        private double angle; // Rotation angle

        public HealthBoost(int x, int y, int size, IntPtr renderer, int speed) : base(x, y, size, size)
        {
            this.renderer = renderer;
            this.speed = speed;
            this.angle = 0; // Initial angle

            if (SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_PNG) == 0)
            {
                Console.WriteLine($"Failed to initialize SDL_image! SDL_image Error: {SDL.SDL_GetError()}");
            }

            string assetPath = "Assets/HealthBoost/health_boost.png";
            texture = SDL_image.IMG_LoadTexture(renderer, assetPath);
            if (texture == IntPtr.Zero)
            {
                Console.WriteLine($"Unable to load texture {assetPath}! SDL_Error: {SDL.SDL_GetError()}");
            }
        }

        public override void Update()
        {
            Move(0, speed);
            angle += 2; // Increment angle for rotation effect
            if (angle > 360) angle -= 360; // Keep angle within 0-360 degrees
        }

        public override void Render(IntPtr renderer)
        {
            SDL.SDL_Point center = new SDL.SDL_Point { x = rect.w / 2, y = rect.h / 2 }; // Center of the texture
            SDL.SDL_RenderCopyEx(renderer, texture, IntPtr.Zero, ref rect, angle, ref center, SDL.SDL_RendererFlip.SDL_FLIP_NONE);
        }
    }
}
