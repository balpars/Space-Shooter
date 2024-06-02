// File: EnemyManager.cs

using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class EnemyManager
    {
        private IntPtr renderer;
        private Random random;
        private Game game;

        private int objectSize = 45;
        private int projectileSize = 40;
        private int screenWidth;
        private int screenHeight;

        public List<Projectile> projectiles;
        private int shootInterval = 400;
        private uint lastShootTime;

        private List<Enemy> enemies;
        private uint lastSpawnTime;
        private int spawnInterval = 2000;

        public EnemyManager(IntPtr renderer, int screenWidth, int screenHeight, List<Enemy> enemies, Game game)
        {
            this.renderer = renderer;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.game = game;
            random = new Random();
            projectiles = new List<Projectile>();
            this.enemies = enemies;
            CreateEnemy();
            lastShootTime = SDL.SDL_GetTicks();
            lastSpawnTime = SDL.SDL_GetTicks();
        }

        public void Update(Player player)
        {
            uint currentTime = SDL.SDL_GetTicks();

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update();
                if (enemies[i].GetRect().y > screenHeight || enemies[i].IsExpired())
                {
                    enemies.RemoveAt(i);
                }
            }

            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].GetRect().y > screenHeight)
                {
                    projectiles.RemoveAt(i);
                }
            }

            CollisionManager.CheckCollisions(projectiles, enemies, player, game);

            if (currentTime > lastShootTime + shootInterval)
            {
                Shoot();
                lastShootTime = currentTime;
            }

            if (currentTime > lastSpawnTime + spawnInterval)
            {
                CreateEnemy();
                lastSpawnTime = currentTime;
            }
        }

        public void Render(Renderer renderer)
        {
            foreach (var enemy in enemies)
            {
                renderer.Draw(enemy);
            }

            foreach (var projectile in projectiles)
            {
                renderer.Draw(projectile);
            }
        }

        private void CreateEnemy()
        {
            int objectX = random.Next(0, screenWidth - objectSize);
            int objectY = -objectSize;
            int speedX = random.Next(1, 4) * (random.Next(2) == 0 ? 1 : -1); // Random speed between -3 and 3
            int speedY = random.Next(2, 5); // Random speed between 2 and 4

            enemies.Add(new BasicEnemy(objectX, objectY, objectSize, renderer, speedX, speedY));
        }

        private void Shoot()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsHit())
                {
                    int projectileX = enemy.GetRect().x + objectSize / 2 - 23;
                    int projectileY = enemy.GetRect().y + objectSize - 5;
                    projectiles.Add(new BasicProjectile(projectileX, projectileY, projectileSize, false, enemy));
                }
            }
        }

        public List<Enemy> GetEnemies()
        {
            return this.enemies;
        }
    }
}
