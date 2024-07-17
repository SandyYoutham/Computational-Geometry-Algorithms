using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class InsertingDiagonals : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<Point> verteces = new List<Point>();
            List<Point> transitionList = new List<Point>();

            for (int i = 0; i < polygons[0].lines.Count; i++)
            {
                verteces.Add(polygons[0].lines[i].Start);
            }
            if (verteces.Count <= 3) return;




            int partition1 = -1; int partition2 = -1;
            for (int i = 0; i < verteces.Count; i++)
            {
                bool flag = false;
                for (int j = i + 2; j != i; j = (j + 1) % verteces.Count)
                {
                    j %= verteces.Count;
                    Line l = new Line(verteces[i], verteces[j]);
                    bool diagonal = true;
                    bool flag1 = true;

                    for (int k = 0; k < verteces.Count - 1; k++)
                    {
                        if (k == i || k == j || k + 1 == i || k + 1 == j) continue;
                        flag1 = IsIntersecting(verteces[i], verteces[j], verteces[k], verteces[k + 1]);
                        if (flag1)
                        {
                            diagonal = false;
                            break;
                        }
                    }
                    flag1 = IsIntersecting(verteces[i], verteces[j], verteces[verteces.Count - 1], verteces[0]);
                    if (flag1 && i != 0 && j != 0 && verteces.Count - 1 != i &&
                        verteces.Count - 1 != j)
                    {
                        diagonal = false;
                    }


                    if (diagonal)
                    {
                        Point prev;
                        Point next;
                        if (i == 0) prev = verteces[verteces.Count - 1];
                        else prev = verteces[i - 1];

                        if (i == verteces.Count - 1) next = verteces[0];
                        else next = verteces[i + 1];

                        if ((HelperMethods.CheckTurn(l, next) == Enums.TurnType.Left)
                        && (HelperMethods.CheckTurn(l, prev) == Enums.TurnType.Right))
                        {
                            flag = true;
                            outLines.Add(l);
                            partition1 = i; partition2 = j;
                            break;
                        }
                    }
                }
                if (flag) break;
            }

            List<Polygon> p1 = new List<Polygon>();
            List<Polygon> p2 = new List<Polygon>();
            List<Line> firstPol = new List<Line>();
            List<Line> secondPol = new List<Line>();

            for (int i = partition1; i != partition2; i = (i + 1) % (verteces.Count))
            {
                firstPol.Add(new Line(verteces[i], verteces[((i + 1) % verteces.Count)]));
            }
            for (int i = partition2; i != partition1; i = (i + 1) % (verteces.Count))
            {
                secondPol.Add(new Line(verteces[i], verteces[((i + 1) % verteces.Count)]));
            }
            firstPol.Add(new Line(verteces[partition2], verteces[partition1]));
            secondPol.Add(new Line(verteces[partition1], verteces[partition2]));
            p1.Add(new Polygon(firstPol));
            p2.Add(new Polygon(secondPol));

            Run(points, lines, p1, ref outPoints, ref outLines, ref outPolygons);
            Run(points, lines, p2, ref outPoints, ref outLines, ref outPolygons);
            return;
        }

        public override string ToString()
        {
            return "Inserting Diagonals";
        }

        bool IsIntersecting(Point p1, Point p2, Point p3, Point p4)
        {
            Enums.TurnType firstOrientation = HelperMethods.CheckTurn(new Line(p1, p2), p3);
            Enums.TurnType secondOrientation = HelperMethods.CheckTurn(new Line(p1, p2), p4);
            Enums.TurnType thirdOrientation = HelperMethods.CheckTurn(new Line(p3, p4), p1);
            Enums.TurnType fourthOrientation = HelperMethods.CheckTurn(new Line(p3, p4), p2);

            if (firstOrientation != secondOrientation && thirdOrientation != fourthOrientation)
            {
                return true;
            }
            if (firstOrientation == 0)
            {
                if (p3.X <= Math.Max(p1.X, p2.X) && p3.X >= Math.Min(p1.X, p2.X) &&
                   p3.Y <= Math.Max(p1.Y, p2.Y) && p3.Y >= Math.Min(p1.Y, p2.Y))
                    return true;
            }
            if (secondOrientation == 0)
            {
                if ((p4.X <= Math.Max(p1.X, p2.X) && p4.X >= Math.Min(p1.X, p2.X) &&
                     p4.Y <= Math.Max(p1.Y, p2.Y) && p4.Y >= Math.Min(p1.Y, p2.Y)))
                    return true;
            }
            if (thirdOrientation == 0)
            {
                if ((p1.X <= Math.Max(p3.X, p4.X) && p1.X >= Math.Min(p3.X, p4.X) &&
                     p1.Y <= Math.Max(p3.Y, p4.Y) && p1.Y >= Math.Min(p3.Y, p4.Y)))
                    return true;
            }
            if (fourthOrientation == 0)
            {
                if ((p2.X <= Math.Max(p3.X, p4.X) && p2.X >= Math.Min(p3.X, p4.X) &&
                 p2.Y <= Math.Max(p3.Y, p4.Y) && p2.Y >= Math.Min(p3.Y, p4.Y)))
                    return true;
            }
            return false;
        }

    }
}
