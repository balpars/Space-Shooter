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
        private int advancedObjectSize = 90; // Increased size for advanced enemies
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
        private List<ShieldBoost> shieldBoosts;
        private uint lastSpawnTime;
        private uint lastAdvancedEnemySpawnTime;
        private uint lastRockSpawnTime;
        private uint lastHealthBoostSpawnTime;
        private uint lastBulletBoostSpawnTime;
        private uint lastShieldBoostSpawnTime;
        private int spawnInterval = 2000;
        private int advancedEnemySpawnInterval = 10000; // 10 seconds interval for advanced enemies
        private int rockSpawnInterval = 1500;
        private int healthBoostSpawnInterval = 10000; // 10 seconds interval
        private int bulletBoostSpawnInterval = 15000; // 15 seconds interval
        private int shieldBoostSpawnInterval = 20000; // 20 seconds interval

        private bool isFastMode;
        private int enemySequenceIndex = 0;
        private EnemyType[] enemySequence = { EnemyType.Basic, EnemyType.Basic, EnemyType.Basic, EnemyType.Basic, EnemyType.Advanced };
        private bool advancedEnemiesActive = false;

        private enum EnemyType
        {
            Basic,
            Advanced
        }

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
            shieldBoosts = new List<ShieldBoost>();
            CreateEnemy();
            lastShootTime = SDL.SDL_GetTicks();
            lastSpawnTime = SDL.SDL_GetTicks();
            lastAdvancedEnemySpawnTime = SDL.SDL_GetTicks();
            lastRockSpawnTime = SDL.SDL_GetTicks();
            lastHealthBoostSpawnTime = SDL.SDL_GetTicks();
            lastBulletBoostSpawnTime = SDL.SDL_GetTicks();
            lastShieldBoostSpawnTime = SDL.SDL_GetTicks();
            isFastMode = false;
        }

        public void Update(Player player)
        {
            uint currentTime = SDL.SDL_GetTicks();

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update();
                if (enemies[i].GetRect().y > screenHeight || enemies[i].IsHit())
                {
                    enemies[i].Cleanup();
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
                    rocks[i].Cleanup();
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

            for (int i = shieldBoosts.Count - 1; i >= 0; i--)
            {
                shieldBoosts[i].Update();
                if (shieldBoosts[i].GetRect().y > screenHeight)
                {
                    shieldBoosts.RemoveAt(i);
                }
            }

            CollisionManager.CheckEnemyCollisions(projectiles, enemies, player, game); // Enemy projectile to Player
            CollisionManager.CheckEnemyCollisions(player.GetProjectiles(), enemies, player, game); // Player projectile to Enemy
            CollisionManager.CheckRockCollisions(rocks, player.GetProjectiles(), player, game);
            CollisionManager.CheckHealthBoostCollisions(healthBoosts, player, game);
            CollisionManager.CheckBulletBoostCollisions(bulletBoosts, player, game);
            CollisionManager.CheckShieldBoostCollisions(shieldBoosts, player, game);

            if (currentTime > lastShootTime + shootInterval)
            {
                Shoot();
                lastShootTime = currentTime;
            }


            if (game.GetScore() >= 3000)
            {
                advancedEnemiesActive = true;
                if (currentTime > lastAdvancedEnemySpawnTime + advancedEnemySpawnInterval)
                {
                    SpawnBossEnemy();
                    SpawnAdvancedEnemies();
                    lastAdvancedEnemySpawnTime = currentTime;
                }
                if (currentTime > lastSpawnTime + spawnInterval)
                {
                    CreateEnemy();
                    lastSpawnTime = currentTime;
                }
            }
            else if (game.GetScore() >= 1000)
            {
                advancedEnemiesActive = true;
                if (currentTime > lastAdvancedEnemySpawnTime + advancedEnemySpawnInterval)
                {
                    SpawnAdvancedEnemies();
                    lastAdvancedEnemySpawnTime = currentTime;
                }
            }

            else
            {
                advancedEnemiesActive = false;
                if (currentTime > lastSpawnTime + spawnInterval)
                {
                    CreateEnemy();
                    lastSpawnTime = currentTime;
                }
            }

            if (currentTime > lastRockSpawnTime + rockSpawnInterval)
            {
                CreateRock();
                lastRockSpawnTime = currentTime;
            }

            if (currentTime > lastHealthBoostSpawnTime + healthBoostSpawnInterval && player.Health <= 3)
            {
                CreateHealthBoost();
                lastHealthBoostSpawnTime = currentTime;
            }

            if (currentTime > lastBulletBoostSpawnTime + bulletBoostSpawnInterval)
            {
                CreateBulletBoost();
                lastBulletBoostSpawnTime = currentTime;
            }

            if (currentTime > lastShieldBoostSpawnTime + shieldBoostSpawnInterval)
            {
                CreateShieldBoost();
                lastShieldBoostSpawnTime = currentTime;
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

            foreach (var shieldBoost in shieldBoosts)
            {
                shieldBoost.Render(renderer.RendererHandle);
            }
        }

        private void CreateEnemy()
        {
            int objectX = random.Next(0, screenWidth - objectSize);
            int objectY = -objectSize;
            int speedX = random.Next(1, 4) * (random.Next(2) == 0 ? 1 : -1);
            int speedY = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5);

            var enemy = new BasicEnemy(objectX, objectY, objectSize, renderer, speedX, speedY, game);
            enemies.Add(enemy);
        }

        private void SpawnAdvancedEnemies()
        {
            int spacing = screenWidth / 3; // Change to divide screen width into three parts
            for (int i = 0; i < 2; i++) // Loop for only two enemies
            {
                int objectX = spacing * (i + 1) - advancedObjectSize / 2; // Adjust for larger size and new spacing
                int objectY = 0;
                Direction direction = i switch
                {
                    0 => Direction.RightDiagonal,
                    1 => Direction.LeftDiagonal,
                    //_ => Direction.Straight
                };
                int moveDirection = i == 0 ? 1 : -1; // First enemy moves right, second moves left
                var enemy = new AdvancedEnemy(objectX, objectY, advancedObjectSize, renderer, moveDirection * 2, 0, game, direction); // Set initial horizontal speed
                enemies.Add(enemy);
            }
        }

        private void SpawnBossEnemy()
        {
            int spacing = screenWidth / 3; // Change to divide screen width into three parts
            for (int i = 0; i < 2; i++) // Loop for only two enemies
            {
                int objectX = spacing * (i + 1) - advancedObjectSize / 2; // Adjust for larger size and new spacing
                int objectY = 0;
                Direction direction = i switch
                {
                    0 => Direction.RightDiagonal,
                    1 => Direction.LeftDiagonal,
                    _ => Direction.Straight
                };
                int moveDirection = i == 0 ? 1 : -1; // First enemy moves right, second moves left
                var enemy = new BossEnemy(objectX, objectY, advancedObjectSize, renderer, moveDirection * 2, 0, game, direction); // Set initial horizontal speed
                enemies.Add(enemy);
            }
        }

        private void CreateRock()
        {
            int rockX = random.Next(0, screenWidth - objectSize);
            int rockY = -objectSize;
            int rockSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5);

            rocks.Add(new Rock(rockX, rockY, objectSize, renderer, rockSpeed));
        }

        private void CreateHealthBoost()
        {
            int boostX = random.Next(0, screenWidth - objectSize);
            int boostY = -objectSize;
            int boostSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5);

            healthBoosts.Add(new HealthBoost(boostX, boostY, objectSize, renderer, boostSpeed));
        }

        private void CreateBulletBoost()
        {
            int boostX = random.Next(0, screenWidth - objectSize);
            int boostY = -objectSize;
            int boostSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5);

            bulletBoosts.Add(new BulletBoost(boostX, boostY, objectSize, renderer, boostSpeed));
        }

        private void CreateShieldBoost()
        {
            int boostX = random.Next(0, screenWidth - objectSize);
            int boostY = -objectSize;
            int boostSpeed = random.Next(isFastMode ? 5 : 2, isFastMode ? 8 : 5);

            shieldBoosts.Add(new ShieldBoost(boostX, boostY, objectSize, renderer, boostSpeed));
        }

        private void Shoot()
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsHit() && enemy is AdvancedEnemy advancedEnemy)
                {
                    advancedEnemy.Shoot(projectiles);
                }
                else
                {
                    int projectileX = enemy.GetRect().x + objectSize / 2 - 23;
                    int projectileY = enemy.GetRect().y + objectSize - 5;
                    var projectile = new BasicProjectile(projectileX, projectileY, projectileSize, false, enemy);
                    if (isFastMode)
                    {
                        projectile.Speed = 10;
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
            spawnInterval = isFast ? 1000 : 2000;
            rockSpawnInterval = isFast ? 1000 : 1500;
            healthBoostSpawnInterval = isFast ? 5000 : 10000;
            bulletBoostSpawnInterval = isFast ? 10000 : 15000;
            shieldBoostSpawnInterval = isFast ? 10000 : 20000;
        }
    }
}
