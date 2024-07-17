using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGUtilities;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<Line> pts = new List<Line>();
            #region initialization & setting the polygon with clockwise direction
            List<Point> vertices = new List<CGUtilities.Point>();
            List<Point> temp = new List<Point>();
            //Initialization Loops 
            for (int i = 0; i < lines.Count; i++) //put our vertices in a list
            {
                temp.Add(lines[i].Start);
            }
            if (temp.Count == 3) return;

            bool isClockWise = IsClockwiseOrder(temp);
            if (isClockWise == false)
            {
                for (int i = temp.Count - 1; i >= 0; i--)
                {
                    vertices.Add(temp[i]);
                }
            }
            else vertices = temp;
            #endregion
            //sorting the vertices according to its y value
            List<Point> sortedVertices = vertices.OrderByDescending(p => p.Y).ToList();
            Point topMostPoint = sortedVertices[0];
            Point bottomMostPoint = sortedVertices[sortedVertices.Count - 1];
            //finding the index of the topmost & bottomMost vertices in the original list
            int topMostIndex = -1;
            int bottomMostIndex = -1;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].X == topMostPoint.X && vertices[i].Y == topMostPoint.Y) topMostIndex = i;
                if (vertices[i].X == bottomMostPoint.X && vertices[i].Y == bottomMostPoint.Y) bottomMostIndex = i;
            }
            // Setting the side of each point in the polygon
            Dictionary<Point, int> dic = new Dictionary<Point, int>();
            for (int i = topMostIndex + 1; i != bottomMostIndex; i++)
            {
                i = i % vertices.Count;
                if (i == bottomMostIndex) break;
                dic.Add(vertices[i], 1);
            }
            for (int i = bottomMostIndex + 1; i != topMostIndex; i++) //redundancy, can be improved.
            {
                i = i % vertices.Count;
                if (i == bottomMostIndex) break;
                if (i == topMostIndex) break;
                dic.Add(vertices[i], 0);
            }
            dic.Add(vertices[bottomMostIndex], 0);
            dic.Add(vertices[topMostIndex], 0);
            //to remove any resulted line that forms the polygon
            Dictionary<Tuple<Tuple<double, double>, Tuple<double, double>>, int> line = new Dictionary<Tuple<Tuple<double, double>, Tuple<double, double>>, int>();
            for (int i = 0; i < lines.Count; i++)
            {
                Tuple<double, double> firstPoint = new Tuple<double, double>(lines[i].Start.X, lines[i].Start.Y);
                Tuple<double, double> secondPoint = new Tuple<double, double>(lines[i].End.X, lines[i].End.Y);
                line.Add(new Tuple<Tuple<double, double>, Tuple<double, double>>(firstPoint, secondPoint), 1);
                line.Add(new Tuple<Tuple<double, double>, Tuple<double, double>>(secondPoint, firstPoint), 1);


            }

            //The Real Work
            Stack<Point> q = new Stack<Point>();
            Stack<int> pos = new Stack<int>();
            int headPos, preHeadPos = -1;
            int currentSide = -1;
            for (int i = 0; i < sortedVertices.Count; i++)
            {
                if (sortedVertices[i].X == vertices[bottomMostIndex].X && sortedVertices[i].Y == vertices[bottomMostIndex].Y) break;
                if (i == 0) { q.Push(sortedVertices[i]); pos.Push(i + 1); continue; }
                else if (i == 1)
                {
                    q.Push(sortedVertices[i]);
                    pos.Push(i + 1);
                    currentSide = dic[sortedVertices[i]];
                    continue;
                }
                Point head = q.Peek();
                headPos = pos.Peek();
                q.Pop();
                pos.Pop();
                Point preHead = q.Peek();
                preHeadPos = pos.Peek();
                q.Push(head);
                pos.Push(headPos);
                if (dic[sortedVertices[i]] == currentSide)
                {
                    Enums.TurnType turnType = currentSide == 1 ? Enums.TurnType.Right : Enums.TurnType.Left; //the convex side 
                    bool tmp = false; //determine whether we should consider the head || bottom in the right or left side
                    if (preHead.X == vertices[bottomMostIndex].X && preHead.Y == vertices[bottomMostIndex].Y ||
                       head.X == vertices[bottomMostIndex].X && head.Y == vertices[bottomMostIndex].Y) dic[vertices[bottomMostIndex]] = currentSide;
                    if (preHead.X == vertices[topMostIndex].X && preHead.Y == vertices[topMostIndex].Y ||
                       head.X == vertices[topMostIndex].X && head.Y == vertices[topMostIndex].Y) dic[vertices[topMostIndex]] = currentSide;
                    /* if (dic[preHead] != dic[head])
                     {
                         if (turnType == Enums.TurnType.Right) turnType = Enums.TurnType.Left;
                         else turnType = Enums.TurnType.Right;
                     }*/
                    bool isReflex = true;
                    do //stops when we find a reflex turn
                    {
                        isReflex = true;
                        Enums.TurnType tt = HelperMethods.CheckTurn(new Line(sortedVertices[i], preHead), head);
                        if (tt == turnType)
                        {
                            isReflex = false;
                            Tuple<double, double> firstPoint = new Tuple<double, double>(sortedVertices[i].X, sortedVertices[i].Y);
                            Tuple<double, double> secondPoint = new Tuple<double, double>(preHead.X, preHead.Y);
                            if (!line.ContainsKey(new Tuple<Tuple<double, double>, Tuple<double, double>>(firstPoint, secondPoint)))
                            {
                                if (!((sortedVertices[i].X == vertices[vertices.Count - 1].X && sortedVertices[i].Y == vertices[vertices.Count - 1].Y)
                                    && (preHead.X == vertices[0].X && preHead.Y == vertices[0].Y)))
                                    outLines.Add(new Line(sortedVertices[i], preHead));
                            }
                            q.Pop();
                            pos.Pop();
                            preHead = q.Peek();
                            head = sortedVertices[i];
                            headPos = i + 1;
                            preHeadPos = i;

                        }
                        if (sortedVertices[i].X != q.Peek().X || sortedVertices[i].Y != q.Peek().Y) q.Push(sortedVertices[i]);
                        if (i + 1 != pos.Peek()) pos.Push(i + 1);


                    } while (!isReflex);
                }
                else
                {
                    while (q.Count > 0)
                    {
                        Tuple<double, double> firstPoint = new Tuple<double, double>(q.Peek().X, q.Peek().Y);
                        Tuple<double, double> secondPoint = new Tuple<double, double>(sortedVertices[i].X, sortedVertices[i].Y);
                        if (!line.ContainsKey(new Tuple<Tuple<double, double>, Tuple<double, double>>(firstPoint, secondPoint)))
                        {
                            if (!((sortedVertices[i].X == vertices[vertices.Count - 1].X && sortedVertices[i].Y == vertices[vertices.Count - 1].Y)
                                && (q.Peek().X == vertices[0].X && q.Peek().Y == vertices[0].Y)))
                                outLines.Add(new Line(q.Peek(), sortedVertices[i]));
                        }
                        q.Pop();
                        pos.Pop();
                    }
                    q.Push(head);
                    q.Push(sortedVertices[i]);
                    pos.Push(headPos);
                    pos.Push(i + 1);
                    currentSide = dic[sortedVertices[i]];
                }
            }

            Console.WriteLine("HI");
        }

        public override string ToString()
        {
            return "Monotone Triangulation";
        }
        bool IsClockwiseOrder(List<Point> points)
        {
            double area = 0;

            for (int i = 0; i < points.Count; i++)
            {
                int nextIndex = (i + 1) % points.Count;
                area += (points[nextIndex].X + points[i].X) * (points[nextIndex].Y - points[i].Y);
            }

            return area < 0;
        }
    }
}