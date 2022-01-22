using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ttt4x4x4
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.RegisterLevel(0, "NOTSET");
            Logger.RegisterLevel(10, "DEBUG");
            Logger.RegisterLevel(20, "INFO");
            Logger.RegisterLevel(30, "WARNING");
            Logger.RegisterLevel(40, "ERROR");
            Logger.RegisterLevel(50, "CRITICAL");

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Window());
        }
        public static int[,,] copy3dArray(int[,,] array) {
            return (int[,,])array.Clone();
        }
        
        /// <summary>
        ///  Look at the List at look for it
        /// </summary>
        /// <returns>first index of the item</returns>
        public static int IsPointInList(List<Point> l, Point searchItem) {
            for (int i = 0; i < l.Count; i++) {
                if (l[i] == searchItem) {
                    return i;
                }
            }
            return -1;
        }

        public static List<Point> flatten(List<List<Point>> rows) {
            List<Point> flattends = new List<Point>();

            foreach (List<Point> row in rows) {
                foreach (Point p in row) {
                    flattends.Add(p);
                }
            }
            
            return flattends;
        }

        public static List<Point> RemoveDouble(List<Point> a) {
            List<Point> result = new List<Point>();

            foreach (Point p in a) {
                if (IsPointInList(result, p) == -1) {
                    result.Add(p);
                }
            }
            return result;
        }

        public static List<Point> RemoveSingle(List<Point> a) {
            List<Point> result = new List<Point>();

            for (int i = 0; i < a.Count; i++) {
                Point item = a[i];
                if (IsPointInList(result, item) == -1) {
                    for (int j = i; j < a.Count; i++) {
                        if (item == a[j]) {
                            result.Add(item.Copy());
                            break;
                        }
                    }
                }
            }

            return result;
        }

        public static bool all(int[] array, int check) {
            foreach (int i in array) {
                if (i != check) {
                    return false;
                }
            }
            return true;
        }
    }

    public static class Logger {
        private static Dictionary<int, string> LogLevels = new Dictionary<int, string>();
        public static int NotAllowedUpTo = 20;

        public static void Log(int level, string message) {
            if (level < NotAllowedUpTo) return;

            for (int i = 0; i < LogLevels.Keys.ToList().Count; i++)
            {
                if (level == LogLevels.Keys.ToArray()[i])
                {
                    Console.WriteLine(LogLevels[i] + ": " + message);
                    return;
                }
            }
            Console.WriteLine(level + ": " + message);
        }

        public static void Debug(string message)
        {
            if (NotAllowedUpTo > 10) return;
            Console.WriteLine(LogLevels[10] + ": " + message);
        }

        public static void Info(string message) {
            if (NotAllowedUpTo > 20) return;
            Console.WriteLine(LogLevels[20] + ": " + message);
        }

        public static void Warning(string message)
        {
            if (NotAllowedUpTo > 30) return;
            Console.WriteLine(LogLevels[30] + ": " + message);
        }

        public static void Error(string message)
        {
            if (NotAllowedUpTo > 40) return;
            Console.WriteLine(LogLevels[40] + ": " + message);
        }

        public static void RegisterLevel(int level, string name) {
            LogLevels[level] = name;
        }
    }

    public struct Point
    {
        public int x, y, z;
        public bool Empty;
        public Point(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.Empty = false;
        }

        public Point(bool empty)
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.Empty = empty;
        }

        public Point(int x, int y, int z, bool empty)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.Empty = empty;
        }

        public Point Copy()
        {
            return new Point(x, y, z, Empty);
        }

        # region operators
        public static Point operator +(Point p1, Point p2) => new Point(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        public static Point operator -(Point p1, Point p2) => new Point(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
        public static Point operator -(Point p) => p * -1;
        public static int operator *(Point p1, Point p2) => p1.x * p2.x + p1.y * p2.y + p1.z * p2.z;
        public static Point operator *(Point p1, int n) => new Point(p1.x * n, p1.y * n, p1.z * n);
        public static bool operator ==(Point p1, Point p2) {
            return p1.Equals(p2);
        }
        public static bool operator !=(Point p1, Point p2) {
            return !p1.Equals(p2);
        }
        # endregion // operators

        public bool Equals(Point p) {
            return this.x == p.x && this.y == p.y && this.z == p.z;
        }

        public override bool Equals(object o) {
            throw new ArgumentException("Only taking a Point object");
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Point(" + x + ", " + y + ", " + z + ")";
        }

        public Tuple<int, int, int> ToTuple()
        {
            return new Tuple<int, int, int>(x, y, z);
        }

        public int[] ToArray()
        {
            return new int[] { x, y, z };
        }

        public List<int> ToList()
        {
            return new List<int>() { x, y, z };
        }
    }
}
