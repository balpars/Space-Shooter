using System;
using System.Collections.Generic;
using System.Numerics;
using SDL2;

namespace Space_Shooter
{
    public class SpawnEnemy
    {
        private IntPtr renderer;
        private Random random;

        private int objectSize = 45;
        private int projectileSize = 40;
        private int screenWidth;
        private int screenHeight;

        private List<Projectile> projectiles;
        private int shootInterval = 400;
        private uint lastShootTime;

        private List<Enemy> enemies;
        private uint lastSpawnTime;
        private int spawnInterval = 2000;

        public SpawnEnemy(IntPtr renderer, int screenWidth, int screenHeight)
        {
            this.renderer = renderer;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            random = new Random();
            projectiles = new List<Projectile>();
            enemies = new List<Enemy>();
            createenemy();
            lastShootTime = SDL.SDL_GetTicks();
            lastSpawnTime = SDL.SDL_GetTicks();
        }

        public void Update(Player player)
        {
            uint currentTime = SDL.SDL_GetTicks();

            // Update falling objects
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update();
                if (enemies[i].GetRect().y > screenHeight)
                {
                    enemies.RemoveAt(i);
                }
                else
                {
                    Console.WriteLine($"enemy at ({enemies[i].GetRect().x}, {enemies[i].GetRect().y})");
                }
            }

            // Update projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles[i].Update();
                if (projectiles[i].GetRect().y < 0)
                {
                    projectiles.RemoveAt(i);
                }
                else
                {
                    Console.WriteLine($"Projectile at ({projectiles[i].GetRect().x}, {projectiles[i].GetRect().y})");
                }
            }

            // Check for collisions
            CollisionManager.CheckCollisions(projectiles, enemies, player);

            // Handle shooting
            if (currentTime > lastShootTime + shootInterval)
            {
                Shoot();
                lastShootTime = currentTime;
            }

            // Handle spawning new objects
            if (currentTime > lastSpawnTime + spawnInterval)
            {
                createenemy();
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

        private void createenemy()
        {
            int objectX = random.Next(0, screenWidth - objectSize);
            int objectY = -objectSize;
            enemies.Add(new BasicEnemy(objectX, -50, objectSize)); Console.WriteLine($"enemy from ({objectX}, {objectY})");
        }

        private void Shoot()
        {
            foreach (var enemy in enemies)
            {
                int projectileX = enemy.GetRect().x + objectSize / 2 - 10;
                int projectileY = enemy.GetRect().y + objectSize - 5;
                projectiles.Add(new BasicProjectile(projectileX, projectileY, projectileSize)); 
                Console.WriteLine($"Projectile from ({projectileX}, {projectileY})");
            }
        }

        
    }
}
