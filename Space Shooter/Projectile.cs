using SDL2;

namespace Space_Shooter
{
    public class Projectile : GameObject
    {
        protected string? assetPath;
        public int Speed { get; private set; }
        public GameObject Owner { get; private set; }

        public Projectile(int x, int y, int size, bool isUpwards, GameObject owner) : base(x, y, size, size)
        {
            Speed = isUpwards ? -5 : 5; // Default speed
            this.Owner = owner;
        }

        public override void Update()
        {
            Move(0, Speed);
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

        public void Render(Renderer renderer)
        {
            renderer.Draw(this);
        }

        public void SetSpeed(int newSpeed)
        {
            Speed = newSpeed;
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
}
