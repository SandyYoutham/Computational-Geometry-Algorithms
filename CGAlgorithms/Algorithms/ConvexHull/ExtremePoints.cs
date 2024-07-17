using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms.Algorithms.ConvexHull;


namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //Removing Duplicates
            List<Point> pts = new List<Point>();
            for (int i = 0; i < points.Count(); i++)
            {
                bool flg = false;
                for (int j = 0; j < pts.Count(); j++)
                {
                    if (points[i].X == points[j].X && points[i].Y == points[j].Y) flg = true;
                    if (flg) break;
                }
                if (!flg) pts.Add(points[i]);
            }

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
            }
        }


        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }

}