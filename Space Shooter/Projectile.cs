using SDL2;

namespace Space_Shooter
{
    public class Projectile : GameObject
    {
        public string? assetPath { get; set; }
        public IntPtr texture { get; set; }
        public int Speed { get; set; } = 5;
        public int SpeedX { get; set; }
        public GameObject Owner { get; private set; }

        public Projectile(int x, int y, int size, bool isUpwards, GameObject owner) : base(x, y, size, size)
        {
            if (isUpwards) { Speed *= -1; }
            this.Owner = owner;
        }

        public override void Update()
        {
            Move(SpeedX, Speed);
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

        public void Render(Renderer renderer)
        {
            renderer.Draw(this);
        }
    }

    public class BasicProjectile : Projectile
    {
        public BasicProjectile(int x, int y, int size, bool isUpwards, GameObject owner)
            : base(x, y, size, isUpwards, owner)
        {
            assetPath = "Assets/Projectiles/projectile.png"; 
        }
    }

    public class AdvancedProjectile : Projectile
    {
        public AdvancedProjectile(int x, int y, int size, bool isUpwards, GameObject owner)
            : base(x, y, size, isUpwards, owner)
        {
            assetPath = "Assets/Projectiles/advanced_projectile.png"; 
        }
    }

    public class BossProjectile : Projectile
    {
        public BossProjectile(int x, int y, int size, bool isUpwards, GameObject owner)
            : base(x, y, size, isUpwards, owner)
        {
            assetPath = "Assets/Bullets/boss_bullet.png"; 
        }

        public SDL.SDL_Rect GetCollisionRect()
        {
            // Return a smaller rectangle for collision detection
            int collisionWidth = rect.w / 3;
            int collisionHeight = rect.h / 2;
            return new SDL.SDL_Rect
            {
                x = rect.x + (rect.w - collisionWidth) / 2,
                y = rect.y + (rect.h - collisionHeight) / 2,
                w = collisionWidth,
                h = collisionHeight
            };
        }
    }
}
