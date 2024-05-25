using SDL2;

namespace Space_Shooter
{
    class Enemy : GameObject
    {
        protected string? assetPath; // Path to the texture asset for the enemy

        public Enemy(int x, int y, int w, int h) : base(x, y, w, h)
        {
        }


        public override void Update()
        {
            // Example enemy movement: move downwards
            Move(0, 2);
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

    }
}
