using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;

namespace MinimalSpanningTreeChallenge
{    
    class Program
    {
        static int size = 8;
        static int dirty = 18;
        static Square[] Dirtysquares = new Square[dirty];

        static void Main(string[] args)
        {
            Roomba roomba = new Roomba(7,0);//set bottom left

            Square[,] squares = Generate();
            Display(squares);

            //int x = roomba.GetLocation().x, y = roomba.GetLocation().y;

            var path = CalcPath(squares);
            Console.WriteLine("Roomba path:");
            TracePath(path, roomba,squares);
            CreateImg(path);

            //while(x >= 0)
            //{
            //    //Console.WriteLine("x:"+x+" y:"+y);
            //    roomba.SetNewLocation(x, y, squares[x--, y].IsDirty);                
            //}
            //for (int i = 0; i <= 7; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        for (int j = 1; j <= 7; j++)
            //            roomba.SetNewLocation(i, j, squares[i, j].IsDirty);
            //    }
            //    else
            //    {
            //        for (int j = 7; j > 0; j--)
            //            roomba.SetNewLocation(i, j, squares[i, j].IsDirty);
            //    }
            //}
            //roomba.SetNewLocation(7, 0, false);//set to start

            //Console.WriteLine("x:"+roomba.GetLocation().x+" y:"+roomba.GetLocation().y);
            Console.WriteLine("Battery:"+roomba.Battery());
            Console.WriteLine("Points:"+roomba.GetPoints());

            Console.WriteLine("done");
            Console.ReadKey();
        }
        static List<Vector2> CalcPath(Square[,] squares)
        {
            List<Vector2> path = new List<Vector2>();
            List<Square> Dirty = Dirtysquares.ToList();
            //Console.WriteLine(Dirty[0].listLocation);

            path.Add(squares[size-1, 0].location);
            Square current = squares[size-1, 0];

            Dirty.Remove(current);

            while(Dirty.Count > 0)
            {
                Square next = GetNearestDirty(current, Dirty);               
                path.Add(next.location);

                //foreach (var item in Dirty)
                //{
                //    Console.WriteLine("Count: "+Dirty.Count+"  x:"+item.location.x+" y:"+item.location.y);
                //}
                //Console.WriteLine();

                Dirty.Remove(next);
                current = next;
            }
            //for (int i = 0; i < path.Count; i++)
            //{
            //    Console.WriteLine("x:"+path[i].x+" y:"+ path[i].y);
            //}
            path.Add(squares[size - 1, 0].location);//return to start

            List<Vector2> fullPath = new List<Vector2>();

            for (int i = 0; i < path.Count-1; i++)
            {
                fullPath.AddRange(GetInbetween(path[i].x, path[i].y, path[i + 1].x, path[i + 1].y));
            }
            fullPath.Distinct().ToList();
            return fullPath;
        }
        static void TracePath(List<Vector2> path,Roomba roomba,Square[,] squares)
        {
            for (int i = 0; i < path.Count; i++)
            {
                roomba.SetNewLocation(path[i].x, path[i].y, squares[path[i].x, path[i].y].IsDirty);
            }
        }
        static void CreateImg(List<Vector2> path)
        {
            int factor = 70;
            Bitmap bmp = new Bitmap(Dirtysquares.Length * factor, Dirtysquares.Length * factor);
            SolidBrush black = new SolidBrush(Color.Black);
            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                {
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.FillEllipse(black, new RectangleF(path[i].x * factor - 7, path[i].y * factor - 7, 14, 14));
                        graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 0), 4), path[i].x * factor, path[i].y * factor, path[0].x * factor, path[0].y * factor);
                    }
                }
                //Console.WriteLine("From x=" + Dirtysquares[i].location.y + " y=" + Dirtysquares[i].location.x + " To x=" + Dirtysquares[0].location.y + " y=" + Dirtysquares[0].location.x);
                else
                {
                    using (var graphics = Graphics.FromImage(bmp))
                    {
                        graphics.FillEllipse(black, new RectangleF(path[i].x * factor - 7, path[i].y * factor - 7, 14, 14));
                        graphics.DrawLine(new Pen(Color.FromArgb(0, 0, 0), 4), path[i].x * factor, path[i].y * factor, path[i+1].x * factor, path[i+1].y * factor);
                    }
                }
                //Console.WriteLine("From x="+Dirtysquares[i].location.y+" y="+ Dirtysquares[i].location.x+" To x=" +Dirtysquares[i+1].location.y+" y="+ Dirtysquares[i + 1].location.x);
            }

            bmp.Save("res.png");
            Process.Start("res.png");
        }
        public static List<Vector2> GetInbetween(int x, int y, int x2, int y2) //bresenhams-line-algorithm
        {
            List<Vector2> points = new List<Vector2>();
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                points.Add(new Vector2(x, y));
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
            return points;
        }
        static Square GetNearestDirty(Square src,List<Square> dirty)
        {
            Square best = dirty[0];
            double shortest = Dist(src.location, dirty[0].location);
            for (int i = 0; i < dirty.Count; i++)
            {
                if (src.location.x == dirty[i].location.x && src.location.y == dirty[i].location.y) continue;
                double d = Dist(src.location, dirty[i].location);

                if (d < shortest)
                {
                    shortest = d;
                    best = dirty[i];
                    best.listLocation = i;                  
                }
            }
            //if (best.location.x == size - 1 && best.location.y == 0)
              //  return null;
            return best;
        }
        static long factorial(long n)
        {
            if (n == 1)
                return 1;
            else
                return n * factorial(n - 1);
        }
        public static Square[,] Generate()
        {
            Square[,] squares = new Square[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    squares[i, j] = new Square(false, new Vector2(i, j));

            Random r = new Random();

            int cnt = 0;
            while (cnt < dirty)
            {
                int x = r.Next(0, size), y = r.Next(0, size);
                if (!squares[x, y].IsDirty)
                {
                    squares[x, y].IsDirty = true;
                    squares[x, y].listLocation = cnt;
                    Dirtysquares[cnt++] = squares[x, y]; 
                    
                }
            }
            return squares;
        }
        public static void Display(Square[,] squares)
        {
            for (int i = 0; i < squares.GetLength(0); i++)
            {
                for (int j = 0; j < squares.GetLength(1); j++)
                {
                    Console.Write(squares[i, j].IsDirty ? "Dirt" + i + "" + j + " " : "Empt" + i + "" + j + " ");
                }
                Console.WriteLine("\n");
            }
        }
        public static double Dist(Vector2 p1, Vector2 p2)
        {
            return Math.Sqrt(((p1.x - p2.x) * (p1.x - p2.x)) + ((p1.y - p2.y) * (p1.y - p2.y)));
        }
        static void swap(int[] a, int i, int j)
        {
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }
    }
    public class Square
    {
        public bool IsDirty;
        public Vector2 location;
        public int listLocation;
        public Square(bool IsDirty, Vector2 location)
        {
            this.IsDirty = IsDirty;
            this.location = location;
        }
    }
    class Roomba
    {
        private int points;
        private int battery;
        private Vector2 location;

        public Roomba(int x, int y)
        {
            battery = 1000;
            points = 0;
            location.x = x;
            location.y = y;
        }
        public int Battery()
        {
            return battery;
        }
        void OnDirty()
        {
            points += 250;
            battery--;
        }
        void OnClean()
        {
            points -= 10;
            battery--;
        }
        public int GetPoints()
        {
            return points;
        }
        public Vector2 GetLocation()
        {
            return location;
        }
        public void SetNewLocation(int x, int y,bool IsDirty)
        {
            Console.WriteLine("x:"+x+" y:"+y);
            location.x = x;
            location.y = y;

            if (IsDirty)
                OnDirty();
            else
                OnClean();           
        }
    }
    public struct Vector2
    {
        public int x, y;
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
