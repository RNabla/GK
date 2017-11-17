using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using static Project2.Geometry;

namespace Project2
{
    public static class SutherlandHodgman
    {
        /// <summary>
        ///     Oblicza pole wielokata przy pomocy formuly Gaussa
        /// </summary>
        /// <param name="polygon">Kolejne wierzcholki wielokata</param>
        /// <returns>Pole wielokata</returns>
        public static double PolygonArea(this Point[] polygon)
        {
            var area = 0.0;
            var n = polygon.Length;
            if (n <= 2)
                return area;
            for (var i = n - 1; --i > 0;)
                area += polygon[i].X * (polygon[i + 1].Y - polygon[i - 1].Y);
            // i = 0
            area += polygon[0].X * (polygon[1].Y - polygon[n - 1].Y);
            // i = n-1
            area += polygon[n - 1].X * (polygon[0].Y - polygon[n - 2].Y);

            area *= 0.5;
            area = Math.Abs(area);
            return area;
        }

        /// <summary>
        ///     Sprawdza, czy dwa punkty leza po tej samej stronie prostej wyznaczonej przez odcinek s
        /// </summary>
        /// <param name="p1">Pierwszy punkt</param>
        /// <param name="p2">Drugi punkt</param>
        /// <param name="s">Odcinek wyznaczajacy prosta</param>
        /// <returns>
        ///     True, jesli punkty leza po tej samej stronie prostej wyznaczonej przez odcinek
        ///     (lub co najmniej jeden z punktow lezy na prostej). W przeciwnym przypadku zwraca false.
        /// </returns>
        public static bool IsSameSide(Point p1, Point p2, Segment s)
        {
            var d1 = CrossProduct(SubtractPoints(s.pe, s.ps), SubtractPoints(p1 , s.ps));
            var d2 = CrossProduct(SubtractPoints(s.pe, s.ps), SubtractPoints(p2 , s.ps));
            return d1 * d2 >= 0;
        }

        /// <summary>Oblicza czesc wspolna dwoch wielokatow przy pomocy algorytmu Sutherlanda–Hodgmana</summary>
        /// <param name="subjectPolygon">Wielokat obcinany (wklesly lub wypukly)</param>
        /// <param name="clipPolygon">Wielokat obcinajacy (musi byc wypukly i zakladamy, ze taki jest)</param>
        /// <returns>Czesc wspolna wielokatow</returns>
        /// <remarks>
        ///     - mozna zalozyc, ze 3 kolejne punkty w kazdym z wejsciowych wielokatow nie sa wspolliniowe
        ///     - wynikiem dzialania funkcji moze byc tak naprawde wiele wielokatow (sytuacja taka moze wystapic,
        ///     jesli wielokat obcinany jest wklesly)
        ///     - jesli wielokat obcinany i obcinajacy zawieraja wierzcholki o tych samych wspolrzednych,
        ///     w wynikowym wielokacie moge one byc zduplikowane
        ///     - wierzcholki wielokata obcinanego, przez ktore przechodza krawedzie wielokata obcinajacego
        ///     zostaja zduplikowane w wielokacie wyjsciowym
        /// </remarks>
        public static Point[] GetIntersectedPolygon(MyPolygon userPolygon, MyPolygon randomPolygon)
        {
            var blank = new Point[0];
            if (userPolygon.MaxX <= randomPolygon.MinX)
                return blank;
            if (randomPolygon.MaxX <= userPolygon.MinX)
                return blank;
            if (userPolygon.MaxY <= randomPolygon.MinY)
                return blank;
            if (randomPolygon.MaxY <= userPolygon.MinY)
                return blank;
            if (userPolygon.Points.Count < 3 || randomPolygon.Points.Count < 3)
                return blank;
            var subjectPolygon = userPolygon.Points.ToArray();
            var clipPolygon = randomPolygon.Points.ToArray();
            var output = new List<Point>(subjectPolygon);
            var input = new List<Point>(output.Count);

            //var dx = clipPolygon.Sum(p => p.X) / clipPolygon.Length;
            //var dy = clipPolygon.Sum(p => p.Y) / clipPolygon.Length;
            var dx = (clipPolygon[0].X + clipPolygon[1].X + clipPolygon[2].X) / 3;
            var dy = (clipPolygon[0].Y + clipPolygon[1].Y + clipPolygon[2].Y) / 3;
            var pointInside = new Point(dx, dy);

            var edges = new List<Segment>(clipPolygon.Length);
            for (var i = 1; i < clipPolygon.Length; i++)
                edges.Add(new Segment(clipPolygon[i - 1], clipPolygon[i]));
            edges.Add(new Segment(clipPolygon[clipPolygon.Length - 1], clipPolygon[0]));

            foreach (var edge in edges)
            {
                input.Clear();
                input.AddRange(output);
                output.Clear();
                if (input.Count == 0) return new Point[0];
                var pp = input[input.Count - 1];
                foreach (var p in input)
                {
                    if (IsSameSide(p, pointInside, edge))
                    {
                        if (IsSameSide(pp, pointInside, edge) == false)
                            output.Add(GetIntersectionPoint(new Segment(pp, p), edge));
                        output.Add(p);
                    }
                    else if (IsSameSide(pp, pointInside, edge))
                    {
                        output.Add(GetIntersectionPoint(edge, new Segment(pp, p)));
                    }
                    pp = p;
                }
            }
            if (output.Count == 0) return new Point[0];

            var distinctOutput = new List<Point>(output.Count)
            {
                output[0]
            };

            for (var i = 1; i < output.Count; i++)
                if (EpsilonEquals(distinctOutput[distinctOutput.Count - 1], output[i]) == false)
                    distinctOutput.Add(output[i]);
            return distinctOutput.ToArray();
        }
        public static Point[] GetIntersectedPolygon2(MyPolygon userPolygon, MyPolygon randomPolygon)
        {
            var blank = new Point[0];
            if (userPolygon.MaxX <= randomPolygon.MinX)
                return blank;
            if (randomPolygon.MaxX <= userPolygon.MinX)
                return blank;
            if (userPolygon.MaxY <= randomPolygon.MinY)
                return blank;
            if (randomPolygon.MaxY <= userPolygon.MinY)
                return blank;
            var subjectPolygon = userPolygon.Points.ToArray();
            var clipPolygon = randomPolygon.Points.ToArray();
            return GetIntersectedPolygon(subjectPolygon, clipPolygon);
        }
        public static System.Windows.Point[] GetIntersectedPolygon(Point[] subjectPolYgon, Point[] clipPolYgon)
        {
            int n = clipPolYgon.Length;
            List<Point> output = new List<Point>(subjectPolYgon);
            for (int i = 0; i < clipPolYgon.Length; ++i)
            {
                if (output.Count > 0)
                {
                    Segment e = new Segment(clipPolYgon[i], clipPolYgon[(i + 1) % n]);
                    Point[] input = output.ToArray();
                    output = new List<Point>();
                    Point pp = input[input.Length - 1];
                    foreach (Point p in input)
                    {
                        if (IsSameSide(p, clipPolYgon[(i + 2) % n], e))
                        {
                            if (!IsSameSide(pp, clipPolYgon[(i + 2) % n], e))
                            {
                                Point a = (GetIntersectionPoint(new Segment(p, pp), e));
                                output.Add(a);
                            }
                            output.Add(p);
                        }
                        else
                        {
                            if (IsSameSide(pp, clipPolYgon[(i + 2) % n], e))
                            {
                                Point a = (GetIntersectionPoint(new Segment(p, pp), e));
                                output.Add(a);
                            }
                        }
                        pp = p;
                    }
                }
            }

            List<System.Windows.Point> ret = new List<System.Windows.Point>();
            int c = output.Count;
            for (int i = 0; i < output.Count; ++i)
            {
                if (output[i].X != output[(i + 1) % c].X && output[i].Y != output[(i + 1) % c].Y)
                    ret.Add(new System.Windows.Point(output[i].X, output[i].Y));

            }
            return ret.ToArray();
        }


        /// <summary>
        ///     Zwraca punkt przeciecia dwoch prostych wyznaczonych przez odcinki
        /// </summary>
        /// <param name="seg1">Odcinek pierwszy</param>
        /// <param name="seg2">Odcinek drugi</param>
        /// <returns>Punkt przeciecia prostych wyznaczonych przez odcinki</returns>
        public static Point GetIntersectionPoint(Segment seg1, Segment seg2)
        {
            var direction1 = new Point(seg1.pe.X - seg1.ps.X, seg1.pe.Y - seg1.ps.Y);
            var direction2 = new Point(seg2.pe.X - seg2.ps.X, seg2.pe.Y - seg2.ps.Y);
            var dotPerp = direction1.X * direction2.Y - direction1.Y * direction2.X;

            var c = new Point(seg2.ps.X - seg1.ps.X, seg2.ps.Y - seg1.ps.Y);
            var t = (c.X * direction2.Y - c.Y * direction2.X) / dotPerp;

            return new Point(seg1.ps.X + t * direction1.X, seg1.ps.Y + t * direction1.Y);
        }
    }
}