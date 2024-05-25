using SDL2;

namespace Space_Shooter
{
    class BasicProjectile : Projectile
    {
        public BasicProjectile(int x, int y, int size) : base(x, y, size)
        {
            assetPath = "C:\\Users\\ebrar\\Source\\Repos\\Space-Shooter\\Space Shooter\\Assets\\Projectiles\\projectile.png";
        }
    }
}
