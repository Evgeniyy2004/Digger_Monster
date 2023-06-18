// Вставьте сюда финальное содержимое файла DiggerTask.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NUnit.Framework.Constraints;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using NUnit.Framework;
using System.Reflection;
using System.Drawing;
namespace Digger
{
    public class Terrain : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            Game.Map[x, y] = new Terrain();
            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            return true;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Terrain.png";
        }
    }

    public class Player : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            var two = Game.MapHeight;
            if (Keys.Left == Game.KeyPressed)
            {
                return Left(Game.Map, x, y);
            }
            else if (Keys.Right == Game.KeyPressed)
            {
                return Right(Game.Map, x, y);
            }

            else if (Keys.Down == Game.KeyPressed)
            {
                return Down(Game.Map, x, y);
            }

            else if (Keys.Up == Game.KeyPressed)
            {
                return DoUp(Game.Map, x, y);
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand Left(ICreature[,] map, int x, int y)
        {
            if (x > 0)
            {
                if (Game.Map[x - 1, y] != null
                && Game.Map[x - 1, y].ToString() != "Sack.png"
                && (Game.Map[x - 1, y] is Terrain || Game.Map[x - 1, y] is Gold))
                {
                    return new CreatureCommand { DeltaX = -1, DeltaY = 0 };
                }

                else if (Game.Map[x - 1, y] == null)
                {
                    return new CreatureCommand { DeltaX = -1, DeltaY = 0 };
                }
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand Right(ICreature[,] map, int x, int y)
        {
            var one = Game.MapWidth;
            if (x < one - 1)
            {
                if (Game.Map[x + 1, y] != null
                && Game.Map[x + 1, y].ToString() != "Sack.png"
                && (Game.Map[x + 1, y] is Terrain || Game.Map[x + 1, y] is Gold))
                {
                    return new CreatureCommand { DeltaX = 1, DeltaY = 0 };
                }

                else if (Game.Map[x + 1, y] == null)
                {
                    return new CreatureCommand { DeltaX = 1, DeltaY = 0 };
                }
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand Down(ICreature[,] map, int x, int y)
        {
            var two = Game.MapHeight;
            if (y < two - 1)
            {
                if (Game.Map[x, y + 1] != null
                && Game.Map[x, y + 1].ToString() != "Sack.png"
                && (Game.Map[x, y + 1] is Terrain || Game.Map[x, y + 1] is Gold))
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
                }

                else if (Game.Map[x, y + 1] == null)
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = 1 };
                }
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public CreatureCommand DoUp(ICreature[,] map, int x, int y)
        {
            if (y > 0)
            {
                if (Game.Map[x, y - 1] != null
                && Game.Map[x, y - 1].ToString() != "Sack.png"
                && (Game.Map[x, y - 1] is Terrain || Game.Map[x, y - 1] is Gold))
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = -1 };
                }

                else if (Game.Map[x, y - 1] == null)
                {
                    return new CreatureCommand { DeltaX = 0, DeltaY = -1 };
                }
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject == null) return false;
            if (conflictedObject is Terrain) return false;
            if (conflictedObject is Player) return true;
            if (conflictedObject is Gold)
            {
                return false;
            }

            return conflictedObject is Sack || conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 0;
        }

        public string GetImageFileName()
        {
            return "Digger.png";
        }
    }

    public class Sack : ICreature
    {
        public int Countcells = 0;
        public CreatureCommand Act(int x, int y)
        {
            var counter = CountNulls(x, y + 1, Game.Map);
            if (counter >= Countcells && counter > 1) Countcells += counter;
            if (y < Game.MapHeight - 1)
            {
                if (counter > 0 || (Countcells > 0
                && (Game.Map[x, y + 1].ToString() == "Digger.Player"
                || Game.Map[x, y + 1] is Monster)))
                {
                    Countcells++;
                    return new CreatureCommand() { DeltaX = 0, DeltaY = 1 };
                }
            }

            if (Countcells > 1)
            {
                Countcells = 0;
                return new CreatureCommand() { DeltaX = 0, DeltaY = 0, TransformTo = new Gold() };
            }

            Countcells = 0;
            return new CreatureCommand() { DeltaX = 0, DeltaY = 0 };
        }

        public int CountNulls(int x, int y, ICreature[,] mep)
        {
            var count = 0;
            for (int i = y; i < Game.MapHeight; i++)
            {
                if (mep[x, i] == null) count++;
                else break;
            }

            return count;
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Gold) return true;
            if (conflictedObject is Player)
            {
                Game.IsOver = true;
                return false;
            }
            //if (conflictedObject is Monster) return false;
            return false;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Sack.png";
        }
    }

    public class Gold : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            return new CreatureCommand { DeltaY = 0, DeltaX = 0 };
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Gold)
            {
                return true;
            }

            if (conflictedObject is null)
            {
                return true;
            }

            if (conflictedObject is Player)
            {
                Game.Scores += 10;
                return true;
            }

            return conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Gold.png";
        }
    }

    public class Monster : ICreature
    {
        public CreatureCommand Act(int x, int y)
        {
            
            List<int[]> allcells = new List<int[]>();
            var isPlayer = FillCells(allcells);
            if (isPlayer)
            {
                List<int[]> tryit = new List<int[]>();
                List<List<int[]>> maybe = new List<List<int[]>>();
                FindTheWay(maybe, x, y, tryit, allcells);
                maybe.Sort((r, m) => r.Count.CompareTo(m.Count));
                if (maybe.Count > 0)
                {
                    var a = maybe[0][1][0];
                    var b = maybe[0][1][1];
                    maybe[0].RemoveAt(1);
                    tryit.Clear();
                    maybe.Clear();
                    return new CreatureCommand { DeltaX = a - x, DeltaY = b - y };
                }
            }

            return new CreatureCommand { DeltaX = 0, DeltaY = 0 };
        }

        public static bool FillCells(List<int[]> allcells)
        {
            var isPlayer=false;
            for (int i = 0; i < Game.MapWidth; i++)
            {
                for (int j = 0; j < Game.MapHeight; j++)
                {
                    if (Game.Map[i, j] is Player)
                    {
                        isPlayer = true;
                    }
                    allcells.Add(new int[] { i, j });
                }
            }
            return isPlayer;
        }
            
        public static void FindTheWay(List<List<int[]>> ways,  int monsterx, int monstery, List<int[]> waynow, List<int[]> availablecells)
        {            
            if (waynow.Count == 0)
            {
                List<int[]> l = availablecells.Where(r => r[0] == monsterx)
                .ToList().Where(c => c[1] == monstery).ToList();
                availablecells.Remove(l[0]);
                waynow.Add(new int[] { monsterx, monstery });
            }
            var need = waynow[waynow.Count - 1];
            if (Game.Map[need[0], need[1]] != null
            && Game.Map[need[0], need[1]] is Player)
            {
                int[][] maybe2 = new int[waynow.Count][];
                waynow.CopyTo(maybe2, 0);
                ways.Add(maybe2.ToList());
                return;
            }

            if (waynow.Count > 1)
            {
                List<int[]> l = availablecells.Where(r => r[0] == need[0]).ToList().Where(c => c[1] == need[1]).ToList();
                availablecells.Remove(l[0]);
            }     
            CheckCopyFindTheWay(ways,  monsterx, monstery, waynow, need[0] + 1, need[1], availablecells);
            CheckCopyFindTheWay(ways,  monsterx, monstery, waynow, need[0] - 1, need[1], availablecells);
            CheckCopyFindTheWay(ways,  monsterx, monstery, waynow, need[0], need[1] + 1, availablecells);
            CheckCopyFindTheWay(ways,  monsterx, monstery, waynow, need[0], need[1] - 1, availablecells);
        }

        public static void CheckCopyFindTheWay(List<List<int[]>> ways,  int mx, int my, List<int[]> waynow, int a, int b, List<int[]> availablecells)
        {
            if (a < 0 || b < 0) return;
            if (a >= Game.MapWidth) return ;
            if (b >= Game.MapHeight) return ;
            if (Game.Map[a, b] == null) ;
            else if (Game.Map[a, b] is Gold) ;
            else if (Game.Map[a, b] is Player) ;
            else if (Game.Map[a, b] is Monster || Game.Map[a, b] is Terrain || Game.Map[a, b] is Sack) return;
            List<int[]> l = availablecells.Where(r => r[0] == a).ToList().Where(c => c[1] == b).ToList();
            if (l.Count == 0) return;
            int[][] maybe5 = new int[waynow.Count + 1][];
            waynow.CopyTo(maybe5, 0);
            maybe5[waynow.Count] = new int[] { a, b };
            int[][] newcells5 = new int[availablecells.Count][];
            availablecells.CopyTo(newcells5, 0);
            FindTheWay(ways, mx, my, maybe5.ToList(), newcells5.ToList());
        }

        public static bool CheckCell(int x, int y)
        {
            if (x < 0 || y < 0) return false;
            if (x >= Game.MapWidth) return false;
            if (y >= Game.MapHeight) return false;
            if (Game.Map[x, y] == null) return true;
            if (Game.Map[x, y] is Gold) return true;
            if (Game.Map[x, y] is Player) return true;
            return !(Game.Map[x, y] is Monster || Game.Map[x, y] is Terrain || Game.Map[x, y] is Sack);
        }

        public static bool IsAvailable(int x, int y, List<int[]> availables)
        {
            List<int[]> l = availables.Where(r => r[0] == x).ToList().Where(c => c[1] == y).ToList();
            return !(l.Count == 0);
        }

        public bool DeadInConflict(ICreature conflictedObject)
        {
            if (conflictedObject is Sack) return true;
            if (conflictedObject is Player) return false;
            return conflictedObject is Monster;
        }

        public int GetDrawingPriority()
        {
            return 1;
        }

        public string GetImageFileName()
        {
            return "Monster.png";
        }
    }
}

