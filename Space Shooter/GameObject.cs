using SDL2;
using System;

namespace Space_Shooter
{
    public abstract class GameObject
    {
        protected SDL.SDL_Rect rect;

        public GameObject(int x, int y, int w, int h)
        {
            rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
        }

        public virtual void Update()
        {
        }

        public virtual void Render(IntPtr renderer)
        {
        }

        public virtual SDL.SDL_Rect GetRect()
        {
            return rect;
        }

        public void Move(int deltaX, int deltaY)
        {
            rect.x += deltaX;
            rect.y += deltaY;
        }
    }
}
