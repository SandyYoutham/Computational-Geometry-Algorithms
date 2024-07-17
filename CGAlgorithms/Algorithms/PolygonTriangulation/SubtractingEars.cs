using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class SubtractingEars : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {

            List<Point> vertices = new List<Point>();
            List<Point> transitionList = new List<Point>();
            List<Line> newLines = new List<Line>();
            bool IsConvex = false;
            bool IsEar = false;

            for (int i = 0; i < polygons[0].lines.Count; i++)
            {
                vertices.Add(polygons[0].lines[i].Start);
            }
            if (vertices.Count <= 3) return;






            for (int i = 0; i < vertices.Count; i++)
            {
                Point prev;
                Point next;
                if (i == 0) prev = vertices[vertices.Count - 1];
                else prev = vertices[i - 1];

                if (i == vertices.Count - 1) next = vertices[0];
                else next = vertices[i + 1];


                if (HelperMethods.CheckTurn(new Line(prev, vertices[i]), next) == Enums.TurnType.Right) IsConvex = true;
                else IsConvex = false;

                if (IsConvex == true)
                {
                    bool ear = true;
                    for (int j = 0; j < vertices.Count; j++)
                    {
                        if (vertices[j] == vertices[i] || vertices[j] == prev || vertices[j] == next) continue;
                        Enums.PointInPolygon check = HelperMethods.PointInTriangle(vertices[j], vertices[i], prev, next);
                        if (check == Enums.PointInPolygon.Inside || check == Enums.PointInPolygon.OnEdge) ear = false;
                    }
                    if (ear) IsEar = true;
                    else IsEar = false;
                }
                if (IsEar == true)
                {
                    outLines.Add(new Line(prev, next));
                    vertices.RemoveAt(i);
                    for (int j = 0; j < vertices.Count - 1; j++)
                    {
                        newLines.Add(new Line(vertices[j], vertices[j + 1]));
                    }
                    newLines.Add(new Line(vertices[vertices.Count - 1], vertices[0]));
                    break;
                }
            }
            List<Polygon> p = new List<Polygon>();
            p.Add(new Polygon(newLines));
            Run(points, lines, p, ref outPoints, ref outLines, ref outPolygons);



        }


        public override string ToString()
        {
            return "Subtracting Ears";
        }





    }
}
