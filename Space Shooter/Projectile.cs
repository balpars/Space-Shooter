using System;
using SDL2;

namespace Space_Shooter
{
    public class Projectile : GameObject
    {
        protected string? assetPath;
        public int Speed { get; private set; } = 5;

        public Projectile(int x, int y, int size) : base(x, y, size, size)
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
