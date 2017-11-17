using System;
using System.Collections.Generic;
using System.Windows;

namespace Project2
{
    public static class Geometry
    {
        public const int ClockWise = -1;
        public const int Collinear = 0;
        public const int AntiClockWise = 1;

        public static double Epsilon = 0.00000001;

        public static readonly Point NullPoint = new Point(0, 0);
        public static readonly Segment NullSegment = new Segment(NullPoint, NullPoint);

        public static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        public static double DotProduct(Point p1, Point p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        public static double Distance(Point p1, Point p2)
        {
            double dx, dy;
            dx = p1.X - p2.X;
            dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static bool EpsilonEquals(Point p1, Point p2)
        {
            return Distance(p1, p2) < Epsilon;
        }

        public static Point SubtractPoints(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        // funkcja zwraca true jesli punkt q nalezy do
        // prostokata wyznaczonego przez punkty p1 i p2
        public static bool OnRectangle(Point p1, Point p2, Point q)
        {
            return Math.Min(p1.X, p2.X) <= q.X && q.X <= Math.Max(p1.X, p2.X) && Math.Min(p1.Y, p2.Y) <= q.Y &&
                   q.Y <= Math.Max(p1.Y, p2.Y);
        }

        public static bool Intersection(Segment s1, Segment s2)
        {
            var s1s_s2 = s2.Direction(s1.ps); // polozenie poczatku odcinka s1 wzgledem odcinka s2
            var s1e_s2 = s2.Direction(s1.pe); // polozenie konca    odcinka s1 wzgledem odcinka s2
            var s2s_s1 = s1.Direction(s2.ps); // polozenie poczatku odcinka s2 wzgledem odcinka s1
            var s2e_s1 = s1.Direction(s2.pe); // polozenie konca    odcinka s2 wzgledem odcinka s1

            var s12 = s1s_s2 * s1e_s2; // polozenie odcinka s1 wzgledem odcinka s2
            var s21 = s2s_s1 * s2e_s1; // polozenie odcinka s2 wzgledem odcinka s1

            // konce jednego z odcinkow leza po tej samej stronie drugiego
            if (s12 > 0 || s21 > 0) return false; // odcinki nie przecinaja sie

            // konce zadnego z odcinkow nie leza po tej samej stronie drugiego
            // i konce jednego z odcinkow leza po przeciwnych stronach drugiego
            if (s12 < 0 || s21 < 0) return true; // odcinki przecinaja sie

            return Math.Min(Math.Max(s1.ps.X, s1.pe.X), Math.Max(s2.ps.X, s2.pe.X)) >=
                   Math.Max(Math.Min(s1.ps.X, s1.pe.X), Math.Min(s2.ps.X, s2.pe.X)) &&
                   Math.Min(Math.Max(s1.ps.Y, s1.pe.Y), Math.Max(s2.ps.Y, s2.pe.Y)) >=
                   Math.Max(Math.Min(s1.ps.Y, s1.pe.Y), Math.Min(s2.ps.Y, s2.pe.Y));

            //return s1s_s2!=Collinear || s1e_s2!=Collinear || s2s_s1!=Collinear || s2e_s1!=Collinear ||
            //       OnRectangle(s2.ps,s2.pe,s1.ps) || OnRectangle(s2.ps,s2.pe,s1.pe) || OnRectangle(s1.ps,s1.pe,s2.ps) || OnRectangle(s1.ps,s1.pe,s2.pe) ;

            //return OnRectangle(s2.ps,s2.pe,s1.ps) || OnRectangle(s2.ps,s2.pe,s1.pe) || OnRectangle(s1.ps,s1.pe,s2.ps) || OnRectangle(s1.ps,s1.pe,s2.pe) ;
        }

        // sortowanie katowe punktow z tablicy p w kierunku przeciwnym do ruchu wskazowek zegara wzgledem punktu centralnego c
        // czyli sortowanie wzgledem rosnacych katow odcinka (c,p[i]) z osia x
        // przy pomocy parametru ifAngleEqual mozna doprecyzowaz kryterium sortowania gdy katy sa rowne
        // (domyslnie nic nie doprecyzowujemy, pozostawiamy rowne)
        public static Point[] AngleSort(Point c, Point[] p, Comparison<Point> ifAngleEqual = null)
        {
            if (ifAngleEqual == null) ifAngleEqual = (p1, p2) => 0;
            if (p == null) throw new ArgumentNullException();
            if (p.Length < 2) return p;
            Comparison<Point> cmp = delegate(Point p1, Point p2)
            {
                var r = -new Segment(c, p1).Direction(p2);
                return r != 0 ? r : ifAngleEqual(p1, p2);
            };
            var s1 = new List<Point>();
            var s2 = new List<Point>();
            for (var i = 0; i < p.Length; ++i)
                if (p[i].Y > c.Y || p[i].Y == c.Y && p[i].X >= c.X)
                    s1.Add(p[i]);
                else
                    s2.Add(p[i]);
            s1.Sort(cmp);
            s2.Sort(cmp);
            s1.AddRange(s2);
            return s1.ToArray();
        }

        public struct Direction
        {
            private readonly int X;

            private Direction(double d)
            {
                X = Math.Abs(d) < Epsilon ? 0 : (d > 0 ? 1 : -1);
            }

            public static bool operator ==(Direction a, Direction b)
            {
                return a.X == b.X;
            }

            public static bool operator !=(Direction a, Direction b)
            {
                return !(a == b);
            }

            public static int operator *(Direction a, Direction b)
            {
                return a.X * b.X;
            }

            public static implicit operator int(Direction d)
            {
                return d.X;
            }

            public static explicit operator Direction(double d)
            {
                return new Direction(d);
            }
        }

        public struct Segment
        {
            public Point ps; // poczatek odcinka
            public Point pe; // koniec odcinka

            public Segment(Point pps, Point ppe)
            {
                ps = pps;
                pe = ppe;
            }

            // funkcja zwraca orientacje punktu p wzgledem odcinka
            public Direction Direction(Point p)
            {
                return (Direction) CrossProduct(
                    new Point(pe.X - ps.X, pe.Y - ps.Y),
                    new Point(p.X - ps.X, p.Y - ps.Y));
            }
        }
    } // static partial class Geometry
} // namespace ASD