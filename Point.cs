using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DungeonGenerationDemo
{
    struct Point
    {
        public Point(int x, int y)
        {
            Col = x;
            Row = y;
        }

        public int Col { get; }
        public int Row { get; }


        /// <summary>
        /// Sees if the provided object is the same object as this
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point p = (Point)obj;
                return (Col == p.Col) && (Row == p.Row);
            }
        }
        public override int GetHashCode()
        {
            return (Col << 2) ^ Row;
        }

        public static bool operator ==(Point x, Point y)
        {
            return x.Col == y.Col && x.Row == y.Row;
        }

        public static bool operator !=(Point x, Point y)
        {
            return !(x == y);
        }

        public static Point operator +(Point x, Point y)
        {
            return new Point(x.Col + y.Col, x.Row + y.Row);
        }

        public static Point operator -(Point x, Point y)
        {
            return new Point(x.Col - y.Col, x.Row - y.Row);
        }

        public override string ToString()
        {
            return String.Format("Point({0}, {1})", Col, Row);
        }

        public double Distance(Point target)
        {
            return Math.Sqrt(Math.Pow(target.Col - Col, 2d) + Math.Pow(target.Row - Row, 2d));
        }
        public Point Normalize(Point target)
        {
            //System.Diagnostics.Debug.Print($"point normalize: this {this} that {target}");
            Point difference = new Point(target.Col - Col, target.Row - Row);

            double distance = Math.Sqrt(Math.Pow(difference.Col, 2d) + Math.Pow(difference.Row, 2d));

            Point direction = new Point((int)Math.Round(difference.Col / distance), (int)Math.Round(difference.Row / distance));

            if (Math.Abs(direction.Col) == Math.Abs(direction.Row))  // Diagonals turn into vertical moves
            {
                direction = new Point(direction.Col, 0);
            }

            return direction;
        }
    }
}
