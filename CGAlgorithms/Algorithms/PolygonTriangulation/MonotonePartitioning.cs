using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotonePartitioning : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            if (lines.Count > 3)
            {
                //rotata anti-clockwise
                foreach (Line l in lines)
                {
                    points.Add(l.Start);
                }
                EnsureAnticlockwiseRotation(points);
                List<Point> orderedPoints = points.OrderByDescending(point => point.Y).ToList();

                Dictionary<Point, Tuple<Line, Line>> adjacentedges = new Dictionary<Point, Tuple<Line, Line>>(); //point: Item1:prevedge Item2:nextegde
                Dictionary<Tuple<Point, Point>, Point> Helper = new Dictionary<Tuple<Point, Point>, Point>();
                Dictionary<Point, string> PointsStat = new Dictionary<Point, string>();
                List<Line> T = new List<Line>();

                for (int i = 0; i < points.Count; i++)
                {
                    String stat = " ";
                    Point prev = points[(i == 0) ? (points.Count - 1) : (i - 1)];
                    Point next = points[(i + 1) % points.Count];
                    Line templ = new Line(prev, next);
                    Enums.TurnType turn = HelperMethods.CheckTurn(templ, points[i]);
                    //if turn is right-> angle < 180 -->Start, End
                    //if turn is left ->> angle > 180 -->Merge, Split

                    if (points[i].Y > prev.Y && points[i].Y > next.Y)
                    {
                        stat = turn == Enums.TurnType.Right ? "Start" : turn == Enums.TurnType.Left ? "Split" : stat;
                    }
                    else if (points[i].Y < prev.Y && points[i].Y < next.Y)
                    {
                        stat = turn == Enums.TurnType.Right ? "End" : turn == Enums.TurnType.Left ? "Merge" : stat;
                    }
                    else
                    {
                        stat = (points[i].Y < prev.Y && points[i].Y > next.Y) ? "Left-Reg" : "Right-Reg";
                    }

                    PointsStat.Add(points[i], stat);
                    adjacentedges.Add(points[i], new Tuple<Line, Line>(new Line(prev, points[i]), new Line(points[i], next)));
                }

                // --------------------------------------------------------apply algo
                foreach (Point Currpoint in orderedPoints)
                {
                    String Status = PointsStat[Currpoint];
                    Line nextEdge = adjacentedges[Currpoint].Item2;
                    Line prevEdge = adjacentedges[Currpoint].Item1;

                    if (Status == "Start")
                    {
                        T.Add(nextEdge);
                        Helper.Add(new Tuple<Point, Point>(nextEdge.Start, nextEdge.End), Currpoint);
                    }
                    else if (Status == "Split")
                    {
                        Line l = FindNearestEdge(Currpoint, T);
                        outLines.Add(new Line(Currpoint, Helper[new Tuple<Point, Point>(l.Start, l.End)]));
                        Helper[new Tuple<Point, Point>(l.Start, l.End)] = Currpoint;

                        T.Add(nextEdge);
                        Helper.Add(new Tuple<Point, Point>(nextEdge.Start, nextEdge.End), Currpoint);

                    }
                    else if (Status == "Merge")
                    {
                        IsItMerge(prevEdge, Currpoint, Helper, PointsStat, ref outLines);
                        T = RemoveEdge(T, prevEdge);

                        Line l = FindNearestEdge(Currpoint, T);
                        IsItMerge(l, Currpoint, Helper, PointsStat, ref outLines);
                        Helper[new Tuple<Point, Point>(l.Start, l.End)] = Currpoint;

                    }
                    else if (Status == "End")
                    {
                        IsItMerge(prevEdge, Currpoint, Helper, PointsStat, ref outLines);
                        T = RemoveEdge(T, prevEdge);
                    }
                    else if (Status == "Left-Reg")
                    {
                        IsItMerge(prevEdge, Currpoint, Helper, PointsStat, ref outLines);
                        T = RemoveEdge(T, prevEdge);

                        T.Add(nextEdge);
                        Helper.Add(new Tuple<Point, Point>(nextEdge.Start, nextEdge.End), Currpoint);
                    }
                    else if (Status == "Right-Reg")
                    {
                        Line l = FindNearestEdge(Currpoint, T);
                        Point p = Helper[new Tuple<Point, Point>(l.Start, l.End)];
                        if (PointsStat[p] == "Merge")
                        {
                            outLines.Add(new Line(Currpoint, Helper[new Tuple<Point, Point>(l.Start, l.End)]));
                            Helper[new Tuple<Point, Point>(l.Start, l.End)] = Currpoint;
                        }

                    }
                }
            }
        }
        static Line FindNearestEdge(Point CurrPoint, List<Line> T)
        {
            Line nearestEdge = null;
            double minDistance = double.MaxValue;

            foreach (Line e in T)
            {
                if (e.Start.X <= CurrPoint.X && e.End.X <= CurrPoint.X)
                {
                    // Find the intersection point
                    Point intersectionPoint = GetIntersectionPoint(e.Start, e.End, CurrPoint.Y);

                    if (intersectionPoint != null)
                    {
                        // Calculate the distance between the intersection point and the target point
                        double distance = CalculateDistance(intersectionPoint, CurrPoint);

                        // Update the nearest segment if the current one is closer
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestEdge = e;
                        }
                    }
                }
            }

            return nearestEdge;
        }
        static void IsItMerge(Line l, Point Currpoint, Dictionary<Tuple<Point, Point>, Point> Helper, Dictionary<Point, string> PointsStat, ref List<CGUtilities.Line> outLines)
        {
            Point p = Helper[new Tuple<Point, Point>(l.Start, l.End)];
            if (PointsStat[p] == "Merge")
            {
                outLines.Add(new Line(Currpoint, Helper[new Tuple<Point, Point>(l.Start, l.End)]));
            }
        }
        static List<Line> RemoveEdge(List<Line> T, Line linetoremove)
        {
            List<Line> tempT = new List<Line>();
            foreach (Line l in T)
            {
                if (!(l.Start.Equals(linetoremove.Start) && l.End.Equals(linetoremove.End)))
                {
                    tempT.Add(l);
                }
            }
            return tempT;
        }
        static Point GetIntersectionPoint(Point start, Point end, double horizontalY)
        {
            // Check if the edge intersects with the horizontal line
            if ((start.Y <= horizontalY && end.Y >= horizontalY) || (start.Y >= horizontalY && end.Y <= horizontalY))
            {
                // Calculate the intersection point using the parametric equation of a line
                double t = (horizontalY - start.Y) / (end.Y - start.Y);
                double intersectionX = start.X + t * (end.X - start.X);

                return new Point(intersectionX, horizontalY);
            }
            return null;
        }
        static double CalculateDistance(Point point1, Point point2)
        {
            // Euclidean distance formula
            double dx = point2.X - point1.X;
            return Math.Sqrt(dx * dx);
        }
        static void EnsureAnticlockwiseRotation(List<Point> points)
        {
            bool CCwise = CheckRotationDirection(points);
            if (!CCwise)
            {
                // Reverse the order of vertices to ensure anticlockwise rotation
                points.Reverse();
            }
        }
        static bool CheckRotationDirection(List<Point> points)
        {
            int count = points.Count;

            // Calculate the cross product of consecutive edges
            double crossProductSum = 0;

            for (int i = 0; i < count; i++)
            {
                Point currentVertex = points[i];
                Point nextVertex = points[(i + 1) % count]; // Wrap around for the last vertex

                crossProductSum += HelperMethods.CrossProduct(currentVertex, nextVertex);
            }

            // Determine the rotation direction based on the sign of the cross product
            //true -> anti-clockwise
            return crossProductSum > 0;
        }
        public override string ToString()
        {
            return "Monotone Partitioning";
        }
    }
}
