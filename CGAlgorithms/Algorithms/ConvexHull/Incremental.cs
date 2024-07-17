using CGUtilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            if (points.Count <= 3)
            {
                outPoints = points;

            }
            else
            {
                //Removing duplicates
                List<Point> FilteredPoints = new List<Point>();
                foreach(Point p in points)
                {
                    if (!FilteredPoints.Contains(p))
                    {
                        FilteredPoints.Add(p);
                    }
                }

                points = FilteredPoints;

                // Sort the points based on x-coordinate first, then y-coordinate
                points.Sort((p1, p2) =>
                {
                    if (p1.X != p2.X)
                    {
                        return p1.X.CompareTo(p2.X);
                    }
                    else
                    {
                        return p1.Y.CompareTo(p2.Y);
                    }
                });
                
                //first triangle
                List<Point> currentCH = new List<Point>();

                currentCH.Add(points[0]);
                currentCH.Add(points[1]);
                currentCH.Add(points[2]);

                Point firstpoint = points[0];
                currentCH = SortPointsCounterclockwise(currentCH);

                List<Point> temp = new List<Point>();
                int index_of_First_point = currentCH.FindIndex(t => t == firstpoint);
                for (int i = index_of_First_point; i < currentCH.Count(); i++)
                {
                    temp.Add(currentCH[i]);
                }

                for (int i = 0; i < index_of_First_point; i++)
                {
                    temp.Add(currentCH[i]);
                }

                currentCH = temp;

                List<Line> currentLines = new List<Line>();

                currentLines.Add(new Line(currentCH[0], currentCH[1]));
                currentLines.Add(new Line(currentCH[1], currentCH[2]));
                currentLines.Add(new Line(currentCH[2], currentCH[0]));

                //searching for supporting points for each point in remaining points  
                for (int i = 3; i < points.Count; i++)
                {
                    bool LSP_found = false;
                    Point leftSupportPoint = null;
                    Point rightSupportPoint = null;
                    int prev = -1;

                    for (int j = 1; j <= currentCH.Count; j++)
                    {
                        if (j == currentCH.Count) { prev = j-1; j=0; } else { prev = j - 1; }
                        if (HelperMethods.CheckTurn(currentLines[prev], points[i]) == Enums.TurnType.Colinear)
                        {
                            if (LSP_found)
                            {
                                double dis1 = calculate_distance(points[i], currentLines[prev].Start);
                                double dis2 = calculate_distance(points[i], currentLines[prev].End);
                                if (dis1 > dis2)
                                {
                                    rightSupportPoint = currentLines[prev].Start;
                                }
                                else { rightSupportPoint = currentLines[prev].End; }
                            }
                            else
                            {
                                leftSupportPoint = currentLines[prev].Start;
                                LSP_found = true;
                            }

                        }
                        else if (HelperMethods.CheckTurn(currentLines[j], points[i]) == Enums.TurnType.Colinear)
                        {
                            if(prev!= currentCH.Count-1) continue;

                        }
                        else if(HelperMethods.CheckTurn(currentLines[prev], points[i])==Enums.TurnType.Left && HelperMethods.CheckTurn(currentLines[j], points[i]) == Enums.TurnType.Right)
                        {
                            leftSupportPoint = currentLines[j].Start;
                            LSP_found = true;
                        }
                        else if(HelperMethods.CheckTurn(currentLines[prev], points[i]) == Enums.TurnType.Right && HelperMethods.CheckTurn(currentLines[j], points[i]) == Enums.TurnType.Left)
                        {
                            rightSupportPoint = currentLines[j].Start;
                        }
                       
                    
                        if (prev == currentCH.Count - 1) break;
                    }

                    
                    //updating current Convex Hull
                    List<Line> filteredLines = new List<Line>();
                    bool start = false;

                    for (int j = 0; j < currentLines.Count(); j++)
                    {
                        if (currentLines[j].Start.Equals(leftSupportPoint))
                        {
                            start = true;
                        }
                        if (start)
                        {
                            if (currentLines[j].Start.Equals(leftSupportPoint))
                            {
                                filteredLines.Add(new Line(leftSupportPoint, points[i]));
                            }
                            if (currentLines[j].End.Equals(rightSupportPoint))
                            {
                                filteredLines.Add(new Line(points[i], rightSupportPoint));
                                start = false;
                            }
                        }
                        else
                        {
                            filteredLines.Add(currentLines[j]);
                        }
                    }

                    // Update tmp
                    currentCH.Clear();
                    for (int l = 0; l < filteredLines.Count; l++)
                    {
                        currentCH.Add(filteredLines[l].Start);
                    }

                    currentLines = filteredLines;
                }

                outPoints = currentCH;
            }

            
        }

        public static List<Point> SortPointsCounterclockwise(List<Point> points)
        {
            Point center = GetCenterPoint(points);
            List<Point> sortedPoints = points.OrderBy(p => GetAngle(p, center)).ToList();

            return sortedPoints;
        }
        public static double GetAngle(Point point, Point center)
        {
            double angle = Math.Atan2(point.Y - center.Y, point.X - center.X);
            if (angle < 0)
            {
                angle += 2 * Math.PI;
            }
            return angle;
        }

        public static Point GetCenterPoint(List<Point> points)
        {
            double sumX = points.Sum(p => p.X);
            double sumY = points.Sum(p => p.Y);
            double centerX = sumX / points.Count;
            double centerY = sumY / points.Count;
            return new Point(centerX, centerY);
        }

        public double calculate_distance(Point A, Point B)
        {
            //sqrt((x1-x0)^2+(y1-y0)^2)
            double deltaX = B.X - A.X;
            double deltaY = B.Y - A.Y;
            double distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }



        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }

    }
}


//https://www.ideone.com/gHycTF