using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            points = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToList(); // O(Nlog(N))

            List<Point> pts = new List<Point>();
            for (int i = 0; i < points.Count(); i++) //Removing Duplicates (O(N))
            {
                if ((i != points.Count() - 1) && (points[i].X != points[i + 1].X || points[i].Y != points[i + 1].Y)) pts.Add(points[i]);
                if (i == points.Count() - 1) pts.Add(points[i]);
            }
            outPoints = Hull(pts);
            //Removing Duplicates
            HashSet<Point> uniquePoints = new HashSet<Point>(outPoints);
            outPoints.Clear();
            outPoints.AddRange(uniquePoints);

        }


        private List<Point> Hull(List<Point> points) //The main recursive function
        {
            //Base
            if (points.Count == 1)
                return points;

            //Divide
            int mid = points.Count / 2;
            List<Point> leftPoints = points.GetRange(0, mid);
            List<Point> rightPoints = points.GetRange(mid, points.Count - mid);

            //Conquer
            List<Point> leftHull = Hull(leftPoints);
            List<Point> rightHull = Hull(rightPoints);

            //Combine
            return MergeHulls(leftHull, rightHull);
        }

        private List<Point> MergeHulls(List<Point> leftHull, List<Point> rightHull)
        {
            if (leftHull.Count == 0)
                return rightHull;

            if (rightHull.Count == 0)
                return leftHull;

            int rightMostIndex = GetRightmostPointIndex(leftHull);
            int leftMostIndex = GetLeftmostPointIndex(rightHull);

            List<int> upperTangent = FetchUpperTangent(leftHull, rightHull, rightMostIndex, leftMostIndex);
            List<int> lowerTangent = FetchLowerTangent(leftHull, rightHull, rightMostIndex, leftMostIndex);

            List<Point> mergedHull = new List<Point>();
            for (int i = 0; i < Math.Max(leftHull.Count, rightHull.Count); i++)
            {
                if (i < leftHull.Count)
                {
                    if (i != upperTangent[0] && i != lowerTangent[0])
                    {
                        if (HelperMethods.CheckTurn(new Line(leftHull[upperTangent[0]], leftHull[lowerTangent[0]]), leftHull[i]) == Enums.TurnType.Right) mergedHull.Add(leftHull[i]);
                    }
                }
                if (i < rightHull.Count)
                {
                    if (i != upperTangent[1] || i != lowerTangent[1])
                    {
                        if (HelperMethods.CheckTurn(new Line(rightHull[upperTangent[1]], rightHull[lowerTangent[1]]), rightHull[i]) == Enums.TurnType.Left) mergedHull.Add(rightHull[i]);
                    }
                }
            }
            mergedHull.Add(leftHull[lowerTangent[0]]);
            mergedHull.Add(rightHull[lowerTangent[1]]);
            mergedHull.Add(leftHull[upperTangent[0]]);
            mergedHull.Add(rightHull[upperTangent[1]]);
            return mergedHull;

        }
        private int GetRightmostPointIndex(List<Point> points) //pseudoCode to get the rightMostPoint of the leftHull
        {
            int rightMostIndex = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X > points[rightMostIndex].X)
                    rightMostIndex = i;
                else if (points[i].X == points[rightMostIndex].X && points[i].Y > points[rightMostIndex].Y)
                    rightMostIndex = i;
            }
            return rightMostIndex;
        }

        private int GetLeftmostPointIndex(List<Point> points) //pseudoCode to get the leftMostPoint of the rightHull
        {
            int leftMostIndex = 0;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < points[leftMostIndex].X)
                    leftMostIndex = i;
                else if (points[i].X == points[leftMostIndex].X && points[i].Y < points[leftMostIndex].Y)
                    leftMostIndex = i;
            }
            return leftMostIndex;
        }

        private double DistanceBetweenPoints(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

        private List<int> FetchUpperTangent(List<Point> leftHull, List<Point> rightHull, int rightMostIndex, int leftMostIndex) //Getting one point from each hull that constructs the tangent 
        {
            List<int> upperTangent = new List<int>();

            int uppera = rightMostIndex, upperb = leftMostIndex;

            int tmp1 = rightMostIndex, tmp2 = leftMostIndex;
            do
            {
                rightMostIndex = tmp1;
                leftMostIndex = tmp2;
                tmp2 = findClockWise(tmp1, tmp2, leftHull, rightHull, 0);
                tmp1 = findAClockWise(tmp1, tmp2, leftHull, rightHull, 0);
            } while (rightMostIndex != tmp1 || tmp2 != leftMostIndex);


            upperTangent.Add(rightMostIndex);
            upperTangent.Add(leftMostIndex);
            return upperTangent;

        }

        private List<int> FetchLowerTangent(List<Point> leftHull, List<Point> rightHull, int rightMostIndex, int leftMostIndex) //Getting one point from each hull that constructs the tangent 
        {
            List<int> lowerTangent = new List<int>();
            int tmp1 = rightMostIndex, tmp2 = leftMostIndex;
            do
            {
                rightMostIndex = tmp1;
                leftMostIndex = tmp2;
                tmp1 = findClockWise(tmp1, tmp2, leftHull, rightHull, 1);
                tmp2 = findAClockWise(tmp1, tmp2, leftHull, rightHull, 1);
            } while (rightMostIndex != tmp1 || tmp2 != leftMostIndex);

            lowerTangent.Add(rightMostIndex);
            lowerTangent.Add(leftMostIndex);
            return lowerTangent; //O(n)
        }

        private int findClockWise(int indexA, int indexB, List<Point> leftHull, List<Point> rightHull, int type) //type is like flag to know if we are working lower or upper tangent
        {
            if (type == 1) //lower tangent else work as upper tangent
            {
                List<Point> tmp = leftHull;
                leftHull = rightHull;
                rightHull = tmp;
                int tmp2;
                tmp2 = indexA;
                indexA = indexB;
                indexB = tmp2;
            }
            for (int i = 0; i < rightHull.Count; i++)
            {
                if (i == indexB) continue;
                if (HelperMethods.CheckTurn(new Line(leftHull[indexA], rightHull[indexB]), rightHull[i]) == Enums.TurnType.Left)
                {
                    indexB = i;
                }
                else if (HelperMethods.CheckTurn(new Line(leftHull[indexA], rightHull[indexB]), rightHull[i]) == Enums.TurnType.Colinear)
                {
                    double dist1 = DistanceBetweenPoints(leftHull[indexA], rightHull[indexB]);
                    double dist2 = DistanceBetweenPoints(leftHull[indexA], rightHull[i]);
                    if (dist2 > dist1)
                    {
                        indexB = i;
                    }
                }
            }
            return indexB; //O(n)
        }
        private int findAClockWise(int indexA, int indexB, List<Point> leftHull, List<Point> rightHull, int type) //like the psuedo code
        {
            if (type == 1) //lower tangent else work as upper tangent
            {
                List<Point> tmp = leftHull;
                leftHull = rightHull;
                rightHull = tmp;
                int tmp2;
                tmp2 = indexA;
                indexA = indexB;
                indexB = tmp2;
            }
            for (int i = 0; i < leftHull.Count; i++)
            {
                if (i == indexA) continue;
                if (HelperMethods.CheckTurn(new Line(rightHull[indexB], leftHull[indexA]), leftHull[i]) == Enums.TurnType.Right)
                {
                    indexA = i;
                }
                else if (HelperMethods.CheckTurn(new Line(rightHull[indexB], leftHull[indexA]), leftHull[i]) == Enums.TurnType.Colinear)
                {
                    double dist1 = DistanceBetweenPoints(rightHull[indexB], leftHull[indexA]);
                    double dist2 = DistanceBetweenPoints(rightHull[indexB], leftHull[i]);
                    if (dist2 > dist1)
                    {
                        indexA = i;
                    }
                }
            }
            return indexA;
        }

        #region myPreferableImplementationByFazlaka (fails with 4 tests unfortunately so I had to follow the lecture's approach
        private bool CompareWithTangent(Point a, Point b, char c, char d)
        {
            //my approach for getting tangents always returns 4 points.
            //But what if one point is inside of the others?
            //so I return 4 points, with indication that if the point I'm currently working on has maxValue of double that means that this point is not with us
            //so I return true to ignore this point in the conditions of checking whether the point is inside the tangents or not.
            if (b.X == double.MaxValue) return true;
            else if (d == 'x')
            {
                if (c == '<') return (a.X <= b.X);
                else return (a.X >= b.X);
            }
            else
            {
                if (c == '<') return (a.Y <= b.Y);
                else return (a.Y >= b.Y);
            }
        }

        // If one of the tangent points are inside the others (i.e. the extreme points don't form square but triangle), I ignore it by setting its value to double.MaxValue
        // the complexity of this function is always o(1); because I always pass 4 points to it, with analysis of the recursive function, this function doesn't affect
        // the total complexity 
        private static List<Point> CheckIfInTriangle(List<Point> points)
        {
            List<Point> pts = points;
            List<Point> outPoints = new List<Point>();
            for (int i = 0; i < pts.Count(); i++)
            {
                Point tmp = pts[i];
                bool flag = true;
                for (int j = 0; j < pts.Count(); j++)
                {
                    if (j == i) continue;
                    for (int k = 0; k < pts.Count(); k++)
                    {
                        if (k == i) continue;
                        for (int l = 0; l < pts.Count(); l++)
                        {
                            if (l == i) continue;
                            Enums.PointInPolygon f = HelperMethods.PointInTriangle(tmp, pts[j], pts[k], pts[l]);
                            if (f == Enums.PointInPolygon.Inside || f == Enums.PointInPolygon.OnEdge)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag) break;
                    }
                    if (!flag) break;
                }
                if (flag) outPoints.Add(tmp);
                else
                {
                    outPoints.Add(new Point(double.MaxValue, double.MaxValue));
                }
            }
            return outPoints;
        }
        //Thus, total complexity of the code is O(nlog(n))

        private List<Point> HF(List<Point> points)
        {
            List<Point> pts = new List<Point>();
            List<Point> outPoints = new List<Point>();
            pts = points;
            //check the lowest point
            double lowestPointY = double.MaxValue;
            double lowestPointX = double.MaxValue;
            int lowestPoint = 0;
            for (int i = 0; i < pts.Count(); i++)
            {
                if (pts[i].Y < lowestPointY || (pts[i].Y == lowestPointY && lowestPointX > pts[i].X))
                {
                    lowestPoint = i;
                    lowestPointY = pts[i].Y;
                    lowestPointX = pts[i].X;
                }
            }
            outPoints.Add(pts[lowestPoint]);
            int indexOfLowest = lowestPoint;

            do
            {
                for (int i = 0; i < pts.Count(); i++)
                {
                    int rights = 0;
                    if (indexOfLowest == i) continue;
                    Line l = new Line(pts[indexOfLowest], pts[i]);
                    for (int j = 0; j < pts.Count(); j++)
                    {
                        Enums.TurnType tt = HelperMethods.CheckTurn(l, pts[j]);
                        if (tt == Enums.TurnType.Right)
                        {
                            rights++;
                            break;
                        }
                        else if (tt == Enums.TurnType.Colinear)
                        {
                            double dist1 = DistanceBetweenPoints(pts[indexOfLowest], pts[i]);
                            double dist2 = DistanceBetweenPoints(pts[indexOfLowest], pts[j]);
                            if (dist2 > dist1)
                            {
                                rights++;
                                break;
                            }

                        }
                    }
                    if (rights == 0)
                    {
                        indexOfLowest = i;
                        if (indexOfLowest == lowestPoint) break;
                        outPoints.Add(pts[indexOfLowest]);
                        break;
                    }
                }

            } while (lowestPoint != indexOfLowest);
            return outPoints;
        }
        #endregion
    }

}

