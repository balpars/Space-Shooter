using SDL2;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class Player : GameObject
    {
        protected string assetPath;
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        private List<Projectile> projectiles;
        private uint lastShootTime;
        private int shootInterval = 500; // milliseconds

        public Player(IntPtr renderer, int w, int h) : base((w - 100) / 2, (h - 100) / 2, 100, 100)
        {
            assetPath = "Assets/Player/player.png";
            this.PositionX = (w - 100) / 2;
            this.PositionY = (h - 100) / 2;
            projectiles = new List<Projectile>();
            lastShootTime = SDL.SDL_GetTicks();
        }

        public override void Update()
        {
            // Update player state here
            UpdateProjectiles();
        }

        public void HandleInput(byte[] keys)
        {
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_UP] == 1)
            {
                PositionY -= 5;
                Move(0, -5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_DOWN] == 1)
            {
                PositionY += 5;
                Move(0, 5);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_LEFT] == 1)
            {
                PositionX -= 5;
                Move(-5, 0);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_RIGHT] == 1)
            {
                PositionX += 5;
                Move(5, 0);
            }
            if (keys[(int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE] == 1)
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            uint currentTime = SDL.SDL_GetTicks();
            if (currentTime > lastShootTime + shootInterval)
            {
                int projectileX = PositionX + rect.w / 2 - 5; // Adjust the x position to center the projectile
                int projectileY = PositionY; // Projectile starts at the top of the player
                projectiles.Add(new BasicProjectile(projectileX, projectileY, 20,true)); // Add projectile to the list
                lastShootTime = currentTime;
            }
        }

        private void UpdateProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].GetRect().y < 0)
                {
                    projectiles.RemoveAt(i);
                }
            }
        }

        public void RenderProjectiles(Renderer renderer)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Render(renderer);
            }
        }

        public string? GetAssetPath()
        {
            return assetPath;
        }

        public List<Projectile> GetProjectiles()
        {
            return projectiles;
        }
    }
}
