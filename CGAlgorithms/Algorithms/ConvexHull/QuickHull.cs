using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            List<Point> extremePoints = new List<Point>();
            List<Line> edges = new List<Line>();
            List<Line> edges2 = new List<Line>();

            if (points.Count() <=3)
            {
                outPoints = points;
            }
            else
            {
                //get extreme points
                double MinX = 100000, MaxX = -1000000, MinY = 10000, MaxY = -1000000;
                Point minXpoint = null, maxXpoint = null, minYpoint = null, maxYpoint = null;
                foreach(Point p in points)
                {
                    if(p.X < MinX) { MinX = p.X; minXpoint = p; } 
                    else if(p.X > MaxX) { MaxX = p.X; maxXpoint = p; }
                    
                    if(p.Y < MinY) { MinY = p.Y; minYpoint = p; }
                    else if(p.Y > MaxY) { MaxY = p.Y; maxYpoint = p; }
                    
                }               
                
                edges.Add(new Line(minXpoint, minYpoint)); 
                edges.Add(new Line(minYpoint, maxXpoint)); 
                edges.Add(new Line(maxXpoint, maxYpoint)); 
                edges.Add(new Line(maxYpoint, minXpoint));

                //in case we have 3 extreme points only -> we need to remove the extra edge 
                foreach (Line l in edges)
                {
                    if (l.Start != l.End)
                    {
                        edges2.Add(l);
                        extremePoints.Add(l.Start);
                    }
                }

                List<Point> remaining_points = GetRemainingPoints(points, extremePoints, edges2);
                List<Line> newl= GetNewEdges(edges2, remaining_points, extremePoints);

                while (remaining_points.Count() != 0)
                {
                    remaining_points = GetRemainingPoints(remaining_points, extremePoints, newl);
                    newl = GetNewEdges(newl, remaining_points, extremePoints);
                }


                outPoints = extremePoints;
            }
        }
        public double calculate_distance(Point A, Point B, Point P)
        {
            //sqrt((x1-x0)^2+(y1-y0)^2)
            double deltaX = P.X - A.X;
            double deltaY = P.Y - A.Y;
            double rightside = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            deltaX = P.X - B.X;
            deltaY = P.Y - B.Y;
            double leftside = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return rightside+leftside;
        }
        public List<Point> GetRemainingPoints(List<Point> points, List<Point> extremePoints, List<Line> edges2)
        {
            List<Point> rest = new List<Point>();
            foreach (Point p in points)
            {
                bool remove = true;
                if (!extremePoints.Contains(p))
                {
                    Enums.TurnType t1 = 0;
                    foreach (Line l in edges2)
                    {
                        if (l == edges2[0])
                        {
                            t1 = HelperMethods.CheckTurn(l, p);
                        }
                        else
                        {
                            Enums.TurnType t2 = HelperMethods.CheckTurn(l, p);
                            if (t1 != Enums.TurnType.Colinear && t2 != Enums.TurnType.Colinear && t1 != t2)
                            {
                                remove = false;
                            }
                        }
                    }
                }

                if (!remove)
                {
                    rest.Add(p);
                }
            }
            return rest;
        }
        public List<Line> GetNewEdges(List<Line> edges2,List<Point> remaining_points, List<Point> extremePoints)
        {
            List<Line> newEdges = new List<Line>();
            foreach (Line l in edges2)
            {
                double Maxdis = 0;
                Point bestPoint = null;
                bool found = false;
                foreach (Point p in remaining_points)
                {
                    if (HelperMethods.CheckTurn(l, p) == Enums.TurnType.Right)
                    {
                        double distance = calculate_distance(l.Start, l.End, p);
                        if (distance > Maxdis)
                        {
                            Maxdis = distance;
                            bestPoint = p;
                            found = true;
                        }
                    }
                }


                if (found)
                {
                    extremePoints.Add(bestPoint);
                    newEdges.Add(new Line(l.Start, bestPoint));
                    newEdges.Add(new Line(bestPoint, l.End));
                }
                else
                {
                    newEdges.Add(l);
                }
            }
            return newEdges;
        }
        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}

