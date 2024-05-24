using SDL2;

namespace Space_Shooter
{
    class BasicEnemy : Enemy
    {
        public BasicEnemy(int x, int y, int w, int h) : base(x, y, w, h)
        {
            assetPath = "Assets/Enemy/enemy.png"; // Define the asset path for the basic enemy
        }
    }
}
