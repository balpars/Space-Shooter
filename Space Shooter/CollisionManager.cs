using System;
using System.Collections.Generic;

namespace Space_Shooter
{
    public class CollisionManager
    {
        public static void CheckCollisions(List<Projectile> projectiles, List<Enemy> enemies, Player player)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                var projectile = projectiles[i];
                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    var enemy = enemies[j];
                    if (IsColliding(projectile, player))
                    {
                        Console.WriteLine($"Collision detected at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                        projectiles.RemoveAt(i);
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                    else if (IsColliding(player, enemy))
                    {
                        Console.WriteLine($"Collision detected at ({enemy.GetRect().x}, {enemy.GetRect().y})");
                        enemies.RemoveAt(j);
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                    else if(IsColliding(projectile, enemy))
                    {
                        Console.WriteLine($"Collision detected at ({projectile.GetRect().x}, {projectile.GetRect().y})");
                        projectiles.RemoveAt(i);
                        enemies.RemoveAt(j);
                        break; // Break out of the inner loop to avoid issues with index shifting
                    }
                    
                }
            }
        }

        public static bool IsColliding(GameObject object1, GameObject object2)
        {
            int objectSize = object1.GetRect().w; // Assuming projectile size is used for collision detection
            return object1.GetRect().x < object2.GetRect().x + objectSize &&
                   object1.GetRect().x + objectSize > object2.GetRect().x &&
                   object1.GetRect().y < object2.GetRect().y + objectSize &&
                   object1.GetRect().y + objectSize > object2.GetRect().y;
        }
    }
}
