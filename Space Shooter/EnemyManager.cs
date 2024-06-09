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
        private List<Rock> rocks;
        private List<HealthBoost> healthBoosts;
        private List<BulletBoost> bulletBoosts;
        private uint lastSpawnTime;
        private uint lastRockSpawnTime;
        private uint lastHealthBoostSpawnTime;
        private uint lastBulletBoostSpawnTime;
        private int spawnInterval = 2000;
        private int rockSpawnInterval = 1500;
        private int healthBoostSpawnInterval = 10000; // 10 seconds interval
        private int bulletBoostSpawnInterval = 15000; // 15 seconds interval

        private bool isFastMode;

        public EnemyManager(IntPtr renderer, int screenWidth, int screenHeight, List<Enemy> enemies, Game game)
        {
            this.renderer = renderer;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.game = game;
            random = new Random();
            projectiles = new List<Projectile>();
            this.enemies = enemies;
            rocks = new List<Rock>();
            healthBoosts = new List<HealthBoost>();
            bulletBoosts = new List<BulletBoost>();
            CreateEnemy();
            lastShootTime = SDL.SDL_GetTicks();
            lastSpawnTime = SDL.SDL_GetTicks();
            lastRockSpawnTime = SDL.SDL_GetTicks();
            lastHealthBoostSpawnTime = SDL.SDL_GetTicks();
            lastBulletBoostSpawnTime = SDL.SDL_GetTicks();
            isFastMode = false;
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

            for (int i = rocks.Count - 1; i >= 0; i--)
            {
                rocks[i].Update();
                if (rocks[i].GetRect().y > screenHeight)
                {
                    rocks.RemoveAt(i);
                }
            }

            for (int i = healthBoosts.Count - 1; i >= 0; i--)
            {
                healthBoosts[i].Update();
                if (healthBoosts[i].GetRect().y > screenHeight)
                {
                    healthBoosts.RemoveAt(i);
                }
            }

            for (int i = bulletBoosts.Count - 1; i >= 0; i--)
            {
                bulletBoosts[i].Update();
                if (bulletBoosts[i].GetRect().y > screenHeight)
                {
                    bulletBoosts.RemoveAt(i);
                }
            }

            CollisionManager.CheckCollisions(projectiles, enemies, player, game);
            CollisionManager.CheckRockCollisions(rocks, player, game);
            CollisionManager.CheckHealthBoostCollisions(healthBoosts, player, game);
            CollisionManager.CheckBulletBoostCollisions(bulletBoosts, player, game);

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

            if (currentTime > lastRockSpawnTime + rockSpawnInterval)
            {
                CreateRock();
                lastRockSpawnTime = currentTime;
            }

            if (currentTime > lastHealthBoostSpawnTime + healthBoostSpawnInterval && player.Health <= 3) // Spawn only if health is 3 or less
            {
                CreateHealthBoost();
                lastHealthBoostSpawnTime = currentTime;
            }

            if (currentTime > lastBulletBoostSpawnTime + bulletBoostSpawnInterval)
            {
                CreateBulletBoost();
                lastBulletBoostSpawnTime = currentTime;
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

            foreach (var rock in rocks)
            {
                rock.Render(renderer.RendererHandle);
            }

            foreach (var healthBoost in healthBoosts)
            {
                healthBoost.Render(renderer.RendererHandle);
            }

            foreach (var bulletBoost in bulletBoosts)
            {
                bulletBoost.Render(renderer.RendererHandle);
            }
        }

        private void CreateEnemy()
        {
            int objectX = random.Next(0, screenWidth - objectSize);
            int objectY = -objectSize;
            int speedX = random.Next(1, 4) * (random.Next(2) == 0 ? 1 : -1); // Random speed between -3 and 3
            int speedY = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5); // Increase speed in fast mode

            enemies.Add(new BasicEnemy(objectX, objectY, objectSize, renderer, speedX, speedY, screenWidth));
        }

        private void CreateRock()
        {
            int rockX = random.Next(0, screenWidth - objectSize);
            int rockY = -objectSize;
            int rockSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5); // Increase speed in fast mode

            rocks.Add(new Rock(rockX, rockY, objectSize, renderer, rockSpeed));
        }

        private void CreateHealthBoost()
        {
            int boostX = random.Next(0, screenWidth - objectSize);
            int boostY = -objectSize;
            int boostSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5); // Increase speed in fast mode

            healthBoosts.Add(new HealthBoost(boostX, boostY, objectSize, renderer, boostSpeed));
        }

        private void CreateBulletBoost()
        {
            int boostX = random.Next(0, screenWidth - objectSize);
            int boostY = -objectSize;
            int boostSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5); // Increase speed in fast mode

            bulletBoosts.Add(new BulletBoost(boostX, boostY, objectSize, renderer, boostSpeed));
        }

        private void Shoot()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsHit())
                {
                    int projectileX = enemy.GetRect().x + objectSize / 2 - 23;
                    int projectileY = enemy.GetRect().y + objectSize - 5;
                    var projectile = new BasicProjectile(projectileX, projectileY, projectileSize, false, enemy);
                    if (isFastMode)
                    {
                        projectile.SetSpeed(10); // Increase speed in fast mode
                    }
                    projectiles.Add(projectile);
                }
            }
        }

        public List<Enemy> GetEnemies()
        {
            return this.enemies;
        }

        public void SetFastMode(bool isFast)
        {
            isFastMode = isFast;
            spawnInterval = isFast ? 1000 : 2000; // Decrease spawn interval in fast mode
            rockSpawnInterval = isFast ? 1000 : 1500; // Decrease rock spawn interval in fast mode
            healthBoostSpawnInterval = isFast ? 5000 : 10000; // Decrease health boost spawn interval in fast mode
            bulletBoostSpawnInterval = isFast ? 10000 : 15000; // Decrease bullet boost spawn interval in fast mode
        }
    }
}
