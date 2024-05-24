using SDL2;

namespace Space_Shooter
{
    class Player : GameObject
    {
        public Player(int x, int y, int w, int h) : base(x, y, w, h) { }

        public override void Update()
        {
            // Update player state here
        }

        public override void HandleInput(byte[] keys)
        {
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                Move(0, -5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                Move(0, 5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                Move(-5, 0);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                Move(5, 0);
            }
        }
    }
}
