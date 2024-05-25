using SDL2;

namespace Space_Shooter
{
    public class Enemy : GameObject
    {
        protected string? assetPath; // Path to the texture asset for the enemy
        public int Speed { get; private set; } = 3;

        public Enemy(int x, int y, int size) : base(x, y, size, size)
        {
        }

        public override void Update()
        {
            Move(0, Speed);
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

    }
}
