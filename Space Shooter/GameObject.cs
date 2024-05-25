using SDL2;

namespace Space_Shooter
{
    abstract class GameObject
    {

        private SDL.SDL_Rect rect;

        public GameObject(int x, int y, int width, int height)
        {
            rect = new SDL.SDL_Rect { x = x, y = y, w = width, h = height };
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

    }
}
