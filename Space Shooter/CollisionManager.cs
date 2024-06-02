// File: CollisionManager.cs

using SDL2;
using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public static class CollisionManager
    {
        public static void CheckCollisions(List<Projectile> projectiles, List<Enemy> enemies, Player player, Game game)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    if (projectile.Owner != enemy && IsColliding(projectile, enemy))
                    {
                        int effectX = projectile.GetRect().x + projectile.GetRect().w / 2;
                        int effectY = projectile.GetRect().y + projectile.GetRect().h / 2;
                        game.AddCollisionEffect(effectX, effectY);
                        projectiles.RemoveAt(i);
                        if (!enemy.IsHit())
                        {
                            enemy.OnHit();
                            game.IncreaseScore(enemy.GetPoints());
                        }
                        game.PlayCollisionSound();
                        break;
                    }
                }

                if (projectile.Owner is Enemy && IsColliding(projectile, player))
                {
                    int effectX = projectile.GetRect().x + projectile.GetRect().w / 2;
                    int effectY = projectile.GetRect().y + projectile.GetRect().h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    projectiles.RemoveAt(i);
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.Health--;
                    }
                    else
                    {
                        game.GameOver();
                    }
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                if (IsColliding(player, enemy))
                {
                    int effectX = enemy.GetRect().x + enemy.GetRect().w / 2;
                    int effectY = enemy.GetRect().y + enemy.GetRect().h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    if (!enemy.IsHit())
                    {
                        enemy.OnHit();
                        game.IncreaseScore(enemy.GetPoints());
                    }
                    game.PlayCollisionSound();
                    if (player.Health > 0)
                    {
                        player.Health--;
                    }
                    else
                    {
                        game.GameOver();
                    }
                }
            }
        }

        public static bool IsColliding(GameObject object1, GameObject object2)
        {
            var rect1 = object1.GetRect();
            var rect2 = object2.GetRect();

            return rect1.x < rect2.x + rect2.w &&
                   rect1.x + rect1.w > rect2.x &&
                   rect1.y < rect2.y + rect2.h &&
                   rect1.y + rect1.h > rect2.y;
        }
    }
}
