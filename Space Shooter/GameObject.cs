using SDL2;

namespace Space_Shooter
{
    abstract class GameObject
    {
        private SDL.SDL_Rect rect;

        public GameObject(int x, int y, int w, int h)
        {
            rect = new SDL.SDL_Rect { x = x, y = y, w = w, h = h };
        }

        public void Move(int deltaX, int deltaY)
        {
            rect.x += deltaX;
            rect.y += deltaY;
        }

        public SDL.SDL_Rect GetRect()
        {
            return rect;
        }

        public abstract void Update();
        public abstract void HandleInput(byte[] keys);
    }
}
