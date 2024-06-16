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
            assetPath = "Assets/Projectiles/projectile.png"; // Path to your projectile image
        }
    }

    public class AdvancedProjectile : Projectile
    {
        public AdvancedProjectile(int x, int y, int size, bool isUpwards, GameObject owner)
            : base(x, y, size, isUpwards, owner)
        {
            assetPath = "Assets/Projectiles/advanced_projectile.png"; // Path to your advanced projectile image
        }
    }

    public class BossProjectile : Projectile
    {
        public BossProjectile(int x, int y, int size, bool isUpwards, GameObject owner)
            : base(x, y, size, isUpwards, owner)
        {
            assetPath = "Assets/Bullets/boss_bullet.png"; // Path to your boss projectile image
        }
    }
}
