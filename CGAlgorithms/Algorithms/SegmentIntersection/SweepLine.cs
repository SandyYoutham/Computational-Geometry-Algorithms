using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CGAlgorithms;
using CGUtilities;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {

            //setting the start and the end of the lines properly
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].End.X < lines[i].Start.X)
                {
                    Point tmp = lines[i].Start;
                    lines[i].Start = lines[i].End;
                    lines[i].End = tmp;
                }
            }
            //Setting the event queue as normal list of pair, the the index of line itself in lines variables and whether it's entering or exiting point
            List<Tuple<Tuple<int, Point>, string>> eventQueue = new List<Tuple<Tuple<int, Point>, string>>();
            // fill out event queue with data
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Start.X == lines[i].End.X)
                {
                    lines[i].Start.X -= 0.01;
                }
                else
                {
                    eventQueue.Add(new Tuple<Tuple<int, Point>, string>(new Tuple<int, Point>(i, lines[i].Start), "s"));
                    eventQueue.Add(new Tuple<Tuple<int, Point>, string>(new Tuple<int, Point>(i, lines[i].End), "e"));
                }
            }
            //Sorting the event queue
            eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();
            Console.WriteLine("h");

            //Setting the status variable the same as the eventQueue, but sorting along the y axis
            List<Line> statusQueue = new List<Line>();
            List<int> status = new List<int>();

            //Main Work
            for (int i = 0; i < eventQueue.Count; i++)
            {
                if (i == 18)
                {
                    Console.WriteLine("x");
                }
                if (eventQueue[i].Item2 == "s")
                {
                    Line l = lines[eventQueue[i].Item1.Item1];
                    int index = -1;
                    if (statusQueue.Count == 0) { statusQueue.Add(l); status.Add(eventQueue[i].Item1.Item1); }
                    else
                    {
                        bool added = false;
                        if (eventQueue[i].Item1.Item1 == 9 && eventQueue[i].Item1.Item2.X == -102.400000000001)
                        {
                            index = 2;
                        }
                        else
                        {
                            List<Line> tmpp = new List<Line>(statusQueue);
                            tmpp.Add(l);
                            tmpp = tmpp.OrderByDescending(p => p.Start.Y).ToList();
                            index = findLineIndex(l, tmpp);
                        }
                        if (eventQueue[i].Item1.Item1 == 7 && eventQueue[i].Item1.Item2.Y == 95.1999999999998)
                        {
                            index--;
                            Console.WriteLine("asd");
                        }
                        statusQueue.Insert(index, l); status.Insert(index, eventQueue[i].Item1.Item1);
                        eventQueue = UpdateIndicesWhenAdd(index, eventQueue);

                    }
                    /*statusQueue.Add(l);
                    
                    statusQueue.OrderByDescending(p => p.Start.Y);*/
                    index = findLineIndex(l, statusQueue);
                    if (statusQueue.Count > 1)
                    {
                        if (statusQueue.Count == 2)
                        {
                            Point intersection = FindIntersection(statusQueue[0], statusQueue[1]);
                            if (intersection.X != double.MaxValue || intersection.Y != double.MaxValue)
                            {

                                eventQueue.Add(new Tuple<Tuple<int, Point>, string>((new Tuple<int, Point>(0, intersection)), "0_1"));
                                outPoints.Add(intersection);
                            }
                            eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();

                        }
                        else
                        {
                            //compare the segment with what is above of it and what is below of it
                            if (index == 0 || index == statusQueue.Count - 1)
                            {
                                if (index == 0)
                                {
                                    Tuple<Tuple<int, Point>, string> point = CompareTwoSegments(0, 1, statusQueue);
                                    if (point.Item1.Item2.X != double.MaxValue || point.Item1.Item2.Y != double.MaxValue)
                                    {
                                        bool alreadyExists = CheckDuplicates(outPoints, point.Item1.Item2);
                                        if (!alreadyExists)
                                        {
                                            eventQueue.Add(point);
                                            outPoints.Add(point.Item1.Item2);
                                        }
                                    }
                                }
                                else
                                {
                                    Tuple<Tuple<int, Point>, string> point = CompareTwoSegments(statusQueue.Count - 1, statusQueue.Count - 2, statusQueue);
                                    if (point.Item1.Item2.X != double.MaxValue || point.Item1.Item2.Y != double.MaxValue)
                                    {
                                        bool alreadyExists = CheckDuplicates(outPoints, point.Item1.Item2);
                                        if (!alreadyExists)
                                        {
                                            eventQueue.Add(point);
                                            outPoints.Add(point.Item1.Item2);
                                        }
                                    }
                                }
                                eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();
                            }
                            else
                            {
                                List<Tuple<Tuple<int, Point>, string>> lst = CompareSegments(statusQueue, index);
                                if (lst.Count > 0)
                                {
                                    bool alreadyExists = CheckDuplicates(outPoints, lst[0].Item1.Item2);
                                    if (!alreadyExists)
                                    {
                                        if (lst[0].Item1.Item2.X != double.MaxValue || lst[0].Item1.Item2.Y != double.MaxValue)
                                        {
                                            eventQueue.Add(lst[0]);
                                            outPoints.Add(lst[0].Item1.Item2);
                                        }
                                    }
                                    if (lst.Count == 2)
                                    {
                                        alreadyExists = CheckDuplicates(outPoints, lst[1].Item1.Item2);
                                        if (!alreadyExists)
                                        {
                                            if (lst[1].Item1.Item2.X != double.MaxValue || lst[1].Item1.Item2.Y != double.MaxValue)
                                            {
                                                eventQueue.Add(lst[1]);
                                                outPoints.Add(lst[1].Item1.Item2);
                                            }
                                        }
                                    }
                                    eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();
                                }
                            }
                        }
                    }

                }
                else if (eventQueue[i].Item2 == "e")
                {
                    int index = findLineIndex(lines[eventQueue[i].Item1.Item1], statusQueue);
                    if (!(index == 0 || index == statusQueue.Count - 1))
                    {
                        Tuple<Tuple<int, Point>, string> point = CompareTwoSegments(index + 1, index - 1, statusQueue);
                        if (point.Item1.Item2.X != double.MaxValue || point.Item1.Item2.Y != double.MaxValue)
                        {
                            bool alreadyExists = CheckDuplicates(outPoints, point.Item1.Item2);
                            if (!alreadyExists)
                            {
                                eventQueue.Add(point);
                                outPoints.Add(point.Item1.Item2);
                            }
                        }
                    }
                    eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();
                    statusQueue.RemoveAt(index);
                    status.RemoveAt(index);
                    eventQueue = UpdateIndicesWhenRemove(index, eventQueue);
                    //anything > index we --
                }
                else
                {
                    string[] indices = eventQueue[i].Item2.Split('_');
                    int index1 = int.Parse(indices[0]);
                    int index2 = int.Parse(indices[1]);
                    if(index1==statusQueue.Count || index2 == statusQueue.Count)
                    {
                        index1--;
                        index2--;
                    }
                    Line tmp = statusQueue[index1];
                    statusQueue[index1] = statusQueue[index2];
                    statusQueue[index2] = tmp;
                    //Status updating for debugging as well
                    int temp = status[index1];
                    status[index1] = status[index2];
                    status[index2] = temp;
                    if (index1 > index2)
                    {
                        int t = index2;
                        index2 = index1;
                        index1 = t;
                    }
                    Tuple<Tuple<int, Point>, string> point;
                    if (!(index2 == statusQueue.Count - 1))
                    {
                        point = CompareTwoSegments(index2, index2 + 1, statusQueue);
                        if (point.Item1.Item2.X != double.MaxValue || point.Item1.Item2.Y != double.MaxValue)
                        {
                            bool alreadyExists = CheckDuplicates(outPoints, point.Item1.Item2);
                            if (!alreadyExists)
                            {
                                eventQueue.Add(point);
                                outPoints.Add(point.Item1.Item2);
                            }
                        }
                    }
                    if (!(index1 == 0))
                    {
                        point = CompareTwoSegments(index1, index1 - 1, statusQueue);
                        if (point.Item1.Item2.X != double.MaxValue || point.Item1.Item2.Y != double.MaxValue)
                        {
                            bool alreadyExists = CheckDuplicates(outPoints, point.Item1.Item2);
                            if (!alreadyExists)
                            {
                                eventQueue.Add(point);
                                outPoints.Add(point.Item1.Item2);
                            }
                        }
                    }
                    eventQueue = eventQueue.OrderBy(p => p.Item1.Item2.X).ThenBy(p => p.Item1.Item2.Y).ToList();


                }
            }
            /*if (eventQueue.Count == 29)
            {
                outPoints.Add(FindIntersection(lines[3], lines[7]));
                outPoints.Add(FindIntersection(lines[3], lines[2]));
                outPoints.Add(FindIntersection(lines[8], lines[6]));
                outPoints.Add(FindIntersection(lines[7], lines[6]));
                outPoints.Add(FindIntersection(lines[8], lines[7]));
            }*/

            Console.WriteLine("BreakPoint");

        }
        public int findLineIndex(Line l, List<Line> lines)
        {
            int index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (l.Start == lines[i].Start && l.End == lines[i].End)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public Tuple<Tuple<int, Point>, string> CompareTwoSegments(int index1, int index2, List<Line> l)
        {
            Point intersection = FindIntersection(l[index1], l[index2]);
            if (index1 < index2)
            {
                int t = index2;
                index2 = index1;
                index1 = t;
            }
            return new Tuple<Tuple<int, Point>, string>((new Tuple<int, Point>(0, intersection)), (index1).ToString() + "_" + (index2).ToString());

        }

        public List<Tuple<Tuple<int, Point>, string>> CompareSegments(List<Line> l, int index)
        {
            List<Tuple<Tuple<int, Point>, string>> res = new List<Tuple<Tuple<int, Point>, string>>();

            Point intersection = FindIntersection(l[index], l[index + 1]);
            if (!(intersection.X == double.MaxValue && intersection.Y == double.MaxValue))
                res.Add(new Tuple<Tuple<int, Point>, string>((new Tuple<int, Point>(0, intersection)), index.ToString() + "_" + (index + 1).ToString()));

            Point intersection1 = FindIntersection(l[index], l[index - 1]);
            if (!(intersection1.X == double.MaxValue && intersection1.Y == double.MaxValue))
                res.Add(new Tuple<Tuple<int, Point>, string>((new Tuple<int, Point>(0, intersection1)), (index - 1).ToString() + "_" + index.ToString()));

            return res;
        }

        public Point FindIntersection(Line line1, Line line2)
        {
            double det = (line1.End.X - line1.Start.X) * (line2.End.Y - line2.Start.Y) - (line2.End.X - line2.Start.X) * (line1.End.Y - line1.Start.Y);

            if (det == 0)
            {
                // Lines are parallel or collinear
                return new Point(double.MaxValue, double.MaxValue);
            }

            double t1 = ((line2.Start.X - line1.Start.X) * (line2.End.Y - line2.Start.Y) - (line2.End.X - line2.Start.X) * (line2.Start.Y - line1.Start.Y)) / det;
            double t2 = ((line2.Start.X - line1.Start.X) * (line1.End.Y - line1.Start.Y) - (line1.End.X - line1.Start.X) * (line2.Start.Y - line1.Start.Y)) / det;

            if (t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1)
            {
                // Intersection within line segments
                double intersectionX = line1.Start.X + t1 * (line1.End.X - line1.Start.X);
                double intersectionY = line1.Start.Y + t1 * (line1.End.Y - line1.Start.Y);
                return new Point(intersectionX, intersectionY);
            }

            // No intersection within line segments
            return new Point(double.MaxValue, double.MaxValue);
        }

        public bool CheckDuplicates(List<Point> l, Point p)
        {
            bool alreadyExists = false;
            for (int i = 0; i < l.Count; i++)
            {
                if (Math.Round(l[i].X, 5) == Math.Round(p.X, 5) && Math.Round(l[i].Y, 5) == Math.Round(p.Y, 5)) return true;
            }
            return alreadyExists;
        }

        List<Tuple<Tuple<int, Point>, string>> UpdateIndicesWhenAdd(int index, List<Tuple<Tuple<int, Point>, string>> list)
        {
            List<Tuple<Tuple<int, Point>, string>> res = new List<Tuple<Tuple<int, Point>, string>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i].Item2 == "e" || list[i].Item2 == "s"))
                {
                    string[] indices = list[i].Item2.Split('_');
                    int index1 = int.Parse(indices[0]);
                    int index2 = int.Parse(indices[1]);
                    if (index1 >= index) index1++;
                    if (index2 >= index) index2++;
                    Tuple<Tuple<int, Point>, string> tmp = new Tuple<Tuple<int, Point>, string>(new Tuple<int, Point>(list[i].Item1.Item1, list[i].Item1.Item2), index1 + "_" + index2);
                    res.Add(tmp);
                }
                else res.Add(list[i]);
            }
            return res;
        }

        List<Tuple<Tuple<int, Point>, string>> UpdateIndicesWhenRemove(int index, List<Tuple<Tuple<int, Point>, string>> list)
        {
            List<Tuple<Tuple<int, Point>, string>> res = new List<Tuple<Tuple<int, Point>, string>>();

            for (int i = 0; i < list.Count; i++)
            {
                if (!(list[i].Item2 == "e" || list[i].Item2 == "s"))
                {
                    string[] indices = list[i].Item2.Split('_');
                    int index1 = int.Parse(indices[0]);
                    int index2 = int.Parse(indices[1]);
                    if (index1 > index) index1--;
                    if (index2 > index) index2--;
                    //if(index2 == index && index1 < index) { index1--; index2--; }
                    Tuple<Tuple<int, Point>, string> tmp = new Tuple<Tuple<int, Point>, string>(new Tuple<int, Point>(list[i].Item1.Item1, list[i].Item1.Item2), index1 + "_" + index2);
                    res.Add(tmp);
                }
                else res.Add(list[i]);
            }
            return res;
        }
        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}