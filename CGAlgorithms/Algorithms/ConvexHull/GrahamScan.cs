using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count() > 3)
            {
                //get vertex
                Point vertex = points.Aggregate((min, p) =>(p.Y < min.Y || (p.Y == min.Y && p.X < min.X)) ? p : min);
                
                // initialize stack of endpoints
                Stack<Point> EP = new Stack<Point>();
                EP.Push(vertex);

                // sort points based on angle related to the vertex
                Point virtualPoint = new Point(vertex.X - 4, vertex.Y);
                Line virtualLine = new Line(virtualPoint, vertex);
                List<Tuple<double, Point>> sortedPoints = new List<Tuple<double, Point>>();
                Point v1 = HelperMethods.GetVector(virtualLine);
                foreach (Point p in points)
                {
                    if (p.X != vertex.X || p.Y != vertex.Y)
                    {
                        double theta = GetAngle(v1, HelperMethods.GetVector(new Line(vertex, p)));
                        sortedPoints.Add(new Tuple<double, Point>(theta, p));
                    }
                }
                sortedPoints.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));


                // remove collinear points by saving farthest one and drop the rest
                Tuple<double, Point> init = sortedPoints.First();
                List<Point> coli_filtered_points = new List<Point>();
                foreach (Tuple<double, Point> CurrPoint in sortedPoints.Skip(1))
                {
                    if (init.Item1 == CurrPoint.Item1)
                    {
                        double dis1 = calculate_distance(vertex, init.Item2);
                        double dis2 = calculate_distance(vertex, CurrPoint.Item2);
                        if (dis1 != 0 && dis2 != 0 && dis1 < dis2)
                        {
                            init = CurrPoint;
                        }

                        if (CurrPoint == sortedPoints.Last())
                        {
                            coli_filtered_points.Add(init.Item2);
                        }
                    }
                    else
                    {
                        coli_filtered_points.Add(init.Item2);
                        if (CurrPoint == sortedPoints.Last())
                        {
                            coli_filtered_points.Add(CurrPoint.Item2);
                        }
                        init = CurrPoint;
                    }
                    
                    
                }

                // getting outer points
                foreach (Point current_point in coli_filtered_points)
                {
                    
                    if (current_point == sortedPoints.First().Item2)
                    {
                        EP.Push(current_point);
                    }
                    else
                    {
                        bool found = false;
                        while (!found && EP.Count() > 0)
                        {
                            Point top = EP.Pop();
                            Line newline = (EP.Count > 0) ? new Line(EP.Peek(), top) : new Line(virtualPoint, top);
                            Enums.TurnType turn = HelperMethods.CheckTurn(newline, current_point);
                            if (turn == Enums.TurnType.Colinear)
                            {
                                found = true;
                                double dis1 = calculate_distance(vertex, current_point);
                                double dis2 = calculate_distance(vertex, top);
                                EP.Push((dis1 != 0 && dis2 != 0 && dis1 > dis2) ? current_point : top); 
                            }
                            else if (turn == Enums.TurnType.Left)
                            {
                                found = true;
                                EP.Push(top);
                                EP.Push(current_point);
                            }
                        }
                    }
                }
                outPoints = EP.ToList();
            }
            else
            {
                outPoints = points;
            }
        }
        public double GetAngle(Point v1, Point v2)
        {
            double cos_theta = (v1.X * v2.X + v1.Y * v2.Y) / (v1.Magnitude() * v2.Magnitude());
            double RadAngle = Math.Acos(cos_theta);
            double degAngle = RadAngle * (180.0 / Math.PI);

            return degAngle;
        }
        public double calculate_distance(Point A, Point B)
        {
            //sqrt((x1-x0)^2+(y1-y0)^2)
            double diff_X = B.X - A.X;
            double diff_Y = B.Y - A.Y;
            return Math.Sqrt(diff_X * diff_X + diff_Y * diff_Y);
        }
        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}