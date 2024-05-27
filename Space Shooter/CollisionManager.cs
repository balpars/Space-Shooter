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
                        //Console.WriteLine($"Collision detected at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                        int effectX = projectile.GetRect().x + projectile.GetRect().w / 2;
                        int effectY = projectile.GetRect().y + projectile.GetRect().h / 2;
                        game.AddCollisionEffect(effectX, effectY);
                        projectiles.RemoveAt(i);
                        enemies.RemoveAt(j);
                        game.PlayCollisionSound();
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                }

                if (projectile.Owner is Enemy && IsColliding(projectile, player))
                {
                    //Console.WriteLine($"Collision detected between enemy projectile and player at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                    int effectX = projectile.GetRect().x + projectile.GetRect().w / 2;
                    int effectY = projectile.GetRect().y + projectile.GetRect().h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    projectiles.RemoveAt(i);
                    game.PlayCollisionSound();
                    // Add logic for what happens when player is hit (e.g., reduce health)
                }
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                if (IsColliding(player, enemy))
                {
                    //Console.WriteLine($"Collision detected between player and enemy at ({enemy.GetRect().x}, {enemy.GetRect().y})");
                    int effectX = enemy.GetRect().x + enemy.GetRect().w / 2;
                    int effectY = enemy.GetRect().y + enemy.GetRect().h / 2;
                    game.AddCollisionEffect(effectX, effectY);
                    enemies.RemoveAt(i);
                    game.PlayCollisionSound();
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

        public static bool IsColliding(Player player, Enemy enemy)
        {
            List<Point> playerVertices = player.GetVertices();
            List<Point> enemyVertices = new List<Point>
            {
                new Point(enemy.GetRect().x, enemy.GetRect().y),
                new Point(enemy.GetRect().x + enemy.GetRect().w, enemy.GetRect().y),
                new Point(enemy.GetRect().x, enemy.GetRect().y + enemy.GetRect().h),
                new Point(enemy.GetRect().x + enemy.GetRect().w, enemy.GetRect().y + enemy.GetRect().h)
            };

            foreach (var enemyVertex in enemyVertices)
            {
                if (PointInTriangle(enemyVertex, playerVertices[0], playerVertices[1], playerVertices[2]))
                {
                    return true;
                }
            }

            foreach (var playerVertex in playerVertices)
            {
                if (PointInTriangle(playerVertex, enemyVertices[0], enemyVertices[1], enemyVertices[2]) ||
                    PointInTriangle(playerVertex, enemyVertices[0], enemyVertices[2], enemyVertices[3]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsColliding(Player player, Projectile projectile)
        {
            List<Point> playerVertices = player.GetVertices();
            SDL.SDL_Rect projectileRect = projectile.GetRect();
            List<Point> projectileVertices = new List<Point>
            {
                new Point(projectileRect.x, projectileRect.y),
                new Point(projectileRect.x + projectileRect.w, projectileRect.y),
                new Point(projectileRect.x, projectileRect.y + projectileRect.h),
                new Point(projectileRect.x + projectileRect.w, projectileRect.y + projectileRect.h)
            };

            foreach (var projectileVertex in projectileVertices)
            {
                if (PointInTriangle(projectileVertex, playerVertices[0], playerVertices[1], playerVertices[2]))
                {
                    return true;
                }
            }

            foreach (var playerVertex in playerVertices)
            {
                if (PointInTriangle(playerVertex, projectileVertices[0], projectileVertices[1], projectileVertices[2]) ||
                    PointInTriangle(playerVertex, projectileVertices[0], projectileVertices[2], projectileVertices[3]))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool PointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        public static float Sign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }
    }
}
