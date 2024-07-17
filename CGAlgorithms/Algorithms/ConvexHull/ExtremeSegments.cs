using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms.Algorithms.ConvexHull;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {

            if (points.Count <= 3)
            {
                outPoints = points;
            }

            // Removing Duplicates
            List<Point> pts = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {

                bool flg = false;
                for (int j = 0; j < pts.Count; j++)
                {
                    if (points[i].X == pts[j].X && points[i].Y == pts[j].Y)
                    {
                        flg = true;
                        break;
                    }
                }
                if (!flg) pts.Add(points[i]);
            }

            // Finding extreme segments
            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = i + 1; j < pts.Count; j++)
                {
                    bool isExtreme = true;
                    int right = 0;
                    int left = 0;
                    int coll = 0;

                    for (int k = 0; k < pts.Count; k++)
                    {

                        if (k == i || k == j) continue;

                        Enums.TurnType turn = HelperMethods.CheckTurn(new Line(pts[i], pts[j]), pts[k]);
                        if (turn == Enums.TurnType.Colinear)
                        {
                            coll++;
                        }
                        else if (turn == Enums.TurnType.Right)
                        {
                            right++;
                        }
                        else if (turn == Enums.TurnType.Left)
                        {
                            left++;
                        }

                    }
                    if (right != 0 && left != 0)
                    {
                        isExtreme = false;
                    }

                    if (isExtreme)
                    {
                        lines.Add(new Line(pts[i], pts[j]));
                    }
                }
            }

            List<Line> new_lines = new List<Line>();

            // Filtering extreme segments
            for (int i = 0; i < lines.Count; i++)
            {
                bool flag = false;
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[i].Start == lines[j].Start)
                    {
                        Enums.TurnType turn = HelperMethods.CheckTurn(lines[i], lines[j].End);
                        if (turn == Enums.TurnType.Colinear)
                        {
                            flag = true;
                            double diff_x = lines[i].Start.X - lines[i].End.X;
                            double diff_y = lines[i].Start.Y - lines[i].End.Y;
                            double distance1 = Math.Sqrt((diff_x * diff_x) + (diff_y * diff_y));
                            double diff_x2 = lines[j].Start.X - lines[j].End.X;
                            double diff_y2 = lines[j].Start.Y - lines[j].End.Y;
                            double distance2 = Math.Sqrt((diff_x2 * diff_x2) + (diff_y2 * diff_y2));
                            if (distance1 > distance2)
                            {
                                new_lines.Add(lines[i]);
                            }
                            else
                            {
                                new_lines.Add(lines[j]);
                            }

                        }

                    }
                    else if (lines[i].End == lines[j].Start || lines[i].Start == lines[j].End)
                    {
                        Enums.TurnType turn = HelperMethods.CheckTurn(lines[i], lines[j].End);
                        if (turn == Enums.TurnType.Colinear)
                        {
                            flag = true;
                            Line l = new Line(lines[i].Start, lines[j].End);
                            if (!lines.Contains(l) && !new_lines.Contains(l))
                            {
                                new_lines.Add(l);
                            }
                            double diff_x = lines[i].Start.X - lines[i].End.X;
                            double diff_y = lines[i].Start.Y - lines[i].End.Y;
                            double distance1 = Math.Sqrt((diff_x * diff_x) + (diff_y * diff_y));

                            double diff_x2 = lines[i].Start.X - lines[j].End.X;
                            double diff_y2 = lines[i].Start.Y - lines[j].End.Y;
                            double distance2 = Math.Sqrt((diff_x2 * diff_x2) + (diff_y2 * diff_y2));
                            if (distance1 > distance2)
                            {
                                new_lines.Add(lines[i]);
                            }
                            else
                            {
                                new_lines.Add(lines[j]);
                            }
                        }
                    }
                }
                if (!flag )
                {
                    new_lines.Add(lines[i]);
                }
            }

            foreach (Line line in new_lines)
            {
                if (!outPoints.Contains(line.Start))
                {
                    outPoints.Add(line.Start);
                }
                if (!outPoints.Contains(line.End))
                {
                    outPoints.Add(line.End);
                }
            }

            List<Point> pointsToDelete = new List<Point>();
            List<Point> Validpoints = new List<Point>();

            // Copy points from source list to target list
            foreach (Point p in outPoints)
            {
                Validpoints.Add(new Point(p.X, p.Y));
            }
            for (int i = 0; i < Validpoints.Count(); i++)
            {
                for (int j = 0; j < Validpoints.Count(); j++)
                {
                    for (int k = 0; k < Validpoints.Count(); k++)
                    {
                        if (i != j && j != k && i != k)
                        {
                            int toRemove = -1;
                            if (HelperMethods.PointOnSegment(Validpoints[i], Validpoints[j], Validpoints[k]))
                            {
                                toRemove = i;
                            }
                            else if (HelperMethods.PointOnSegment(Validpoints[j], Validpoints[i], Validpoints[k]))
                            {
                                toRemove = j;
                            }
                            else if (HelperMethods.PointOnSegment(Validpoints[k], Validpoints[j], Validpoints[i]))
                            {
                                toRemove = k;
                            }

                            if (toRemove != -1)
                            {
                                pointsToDelete.Add(Validpoints[toRemove]);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < pointsToDelete.Count; i++)
            {
                Validpoints.Remove(pointsToDelete[i]);
            }

            outPoints = Validpoints;

        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
