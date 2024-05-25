using SDL2;

namespace Space_Shooter
{
    class Player : GameObject
    {
        protected string assetPath;
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }

        public Player(IntPtr renderer, int w, int h) : base((w - 100) / 2, (h - 100) / 2, 100, 100)
        {
            assetPath = "Assets/Player/player.png";
            this.PositionX = (w - 100) / 2;
            this.PositionY = (h - 100) / 2;
        }

        public override void Update()
        {
            // Update player state here
        }

        public void HandleInput(byte[] keys)
        {
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                PositionY -= 5;
                Move(0, -5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                PositionY += 5;
                Move(0, 5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                PositionX -= 5;
                Move(-5, 0);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                PositionX += 5;
                Move(5, 0);
            }
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }
    }
}
