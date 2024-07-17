using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms.Algorithms.ConvexHull;


namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //check the lowest point
            double lowestPointY = double.MaxValue;
            double lowestPointX = double.MaxValue;
            int lowestPoint = 0;
            for (int i = 0; i < points.Count(); i++)
            {
                if (points[i].Y < lowestPointY || (points[i].Y == lowestPointY && lowestPointX > points[i].X))
                {
                    lowestPoint = i;
                    lowestPointY = points[i].Y;
                    lowestPointX = points[i].X;
                }
            }
            //outPoints.Add(points[lowestPoint]);
            int indexOfLowest = lowestPoint;
            int currentIndex = indexOfLowest, nextIndex = currentIndex + 1;

            do
            {
                outPoints.Add(points[indexOfLowest]);
                nextIndex %= points.Count;
                for (int i = 0; i < points.Count; i++)
                {
                    if (HelperMethods.CheckTurn(new Line(points[currentIndex], points[i]), points[nextIndex]) == Enums.TurnType.Left)
                    {
                        nextIndex = i;
                    }
                    else if (HelperMethods.CheckTurn(new Line(points[currentIndex], points[i]), points[nextIndex]) == Enums.TurnType.Colinear)
                    {
                        double dist1 = DistanceBetweenPoints(points[currentIndex], points[i]);
                        double dist2 = DistanceBetweenPoints(points[currentIndex], points[nextIndex]);
                        if (dist1 > dist2)
                        {
                            nextIndex = i;
                        }
                    }
                }
                indexOfLowest = nextIndex;
                currentIndex = nextIndex;
                nextIndex++;
            } while (indexOfLowest != lowestPoint);
        }

        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
