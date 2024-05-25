using SDL2;

namespace Space_Shooter
{
    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int size) : base(x, y, size)
        {
            assetPath = "Assets/Enemy/enemy.png"; // Define the asset path for the basic enemy
        }
    }
}
