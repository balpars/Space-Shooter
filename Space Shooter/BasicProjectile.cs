using SDL2;

namespace Space_Shooter
{
    class BasicProjectile : Projectile
    {
        public BasicProjectile(int x, int y, int size) : base(x, y, size)
        {
            //assetPath = "C:\\Users\\Hp\\Source\\Repos\\Space_Shooter\\Space Shooter\\Assets\\Projectiles\\projectile.png";
            assetPath = "Assets/Projectiles/projectile.png";
        }
    }
}
