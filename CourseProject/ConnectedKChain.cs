using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseProject
{
    public class ConnectedKChain
    {
        private static int _k;

        public static double Decide(int k, List<NumeratedConnectedNode> nodes, int v)
        {
            if (v <= k)
                return 0;

            _k = k;

            foreach (var nodeZ in nodes)
            {
                switch (nodeZ.Node.Type)
                {
                    case Node.NodeType.Knot:
                        DecideKnote(nodeZ);
                        break;
                    case Node.NodeType.S:
                        DecideS(nodeZ);
                        break;
                    case Node.NodeType.P:
                        DecideP(nodeZ);
                        break;
                    case Node.NodeType.J:
                        DecideJ(nodeZ);
                        break;
                }
            }
            var s = new double[4];
            nodes[nodes.Count - 1].Parameters.LC[0] = double.PositiveInfinity;
            s[0] = nodes[nodes.Count - 1].Parameters.LC.Min();
            nodes[nodes.Count - 1].Parameters.RC[0] = double.PositiveInfinity;
            s[1] = nodes[nodes.Count - 1].Parameters.RC.Min();
            s[2] = nodes[nodes.Count - 1].Parameters.QC.Min();
            s[3] = nodes[nodes.Count - 1].Parameters.Mc;
            return s.Min();
        }

        private static void DecideS(NumeratedConnectedNode nodeZ)
        {
            
        }

        private static void DecideP(NumeratedConnectedNode nodeZ)
        {
            var nodeX = nodeZ.NodeX;
            var nodeY = nodeZ.NodeY;
            nodeZ.Parameters.a = nodeX.Parameters.a > nodeY.Parameters.a 
                ? nodeX.Parameters.a : nodeY.Parameters.a;
            nodeZ.Parameters.c = nodeY.Parameters.c > nodeX.Parameters.c 
                ? nodeY.Parameters.c : nodeX.Parameters.c;
            nodeZ.Parameters.e = nodeY.Parameters.e > nodeX.Parameters.e
                ? nodeY.Parameters.e : nodeX.Parameters.e;

            var s = new int[4];
            s[0] = nodeY.Parameters.b;
            s[1] = nodeX.Parameters.b;
            s[2] = nodeX.Parameters.e + nodeY.Parameters.c;
            s[3] = nodeY.Parameters.e + nodeX.Parameters.c;
            nodeZ.Parameters.b = s.Max();

            s[0] = nodeY.Parameters.d;
            s[1] = nodeX.Parameters.d;
            s[3] = nodeX.Parameters.e + nodeY.Parameters.a;
            s[4] = nodeY.Parameters.e + nodeX.Parameters.a;
            nodeZ.Parameters.d = s.Max();

            nodeZ.Parameters.Mc = nodeY.Parameters.Mc + nodeX.Parameters.Mc 
                - nodeZ.Node.WL - nodeZ.Node.WR;

            for (var a = 1; a <= _k - 1; a++)
            {
                nodeZ.Parameters.LC[a] = MinLRPNode(nodeX.Parameters.LC, nodeY.Parameters.LC, a)
                    - nodeX.Node.WL;
                nodeZ.Parameters.RC[a] = MinLRPNode(nodeX.Parameters.RC, nodeY.Parameters.RC, a)
                    - nodeX.Node.WR;

                for (var b = 1; b < _k-1; b++)
                {
                    for (var c = 1; c < _k - 1; c++)
                    {
                        for (var d = 1; d < _k - 1; d++)
                        {
                            for (var e = 0; e < _k - 1; e++)
                            {
                                nodeZ.Parameters.QC[a][b][c][d][e] = MinQCxQCyPNode(nodeX.Parameters.QC,
                                    nodeY.Parameters.QC, a, b, c, d, e);
                            }
                        }
                    }
                }
            }
        }

        private static double MinQCxQCyPNode(double[][][][][] QCx, double[][][][][] QCy, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var a2 = 1; a2 <= a; a2++)
                {
                    var maxA = a1 > a2 ? a1 : a2;
                    if(maxA != a)
                        continue;

                    for (var b1 = 1; b1 <= b; b1++)
                    {
                        for (var b2 = 1; b2 <= b; b2++)
                        {
                            for (var c1 = 1; c1 <= c; c1++)
                            {
                                for (var c2 = 1; c2 <= c; c2++)
                                {
                                    var maxC = c1 > c2 ? c1 : c2;
                                    if(maxC != c)
                                        continue;

                                    for (var d1 = 1; d1 < d; d1++)
                                    {
                                        for (var d2 = 1; d2 < d; d2++)
                                        {
                                            for (var e1 = 0; e1 < e; e1++)
                                            {
                                                for (var e2 = 0; e2 < e; e2++)
                                                {
                                                    var maxE = e1 > e2 ? e1 : e2;
                                                    if(maxE != e)
                                                        continue;

                                                    var _e1 = e1 == 0 ? -_k : 0;
                                                    var _e2 = e2 == 0 ? -_k : 0;
                                                    var maxB = b1 > b2 ? b1 : b2;
                                                    maxB = maxB > _e1 + c2 - 1 ? maxB : _e1 + c2 - 1;
                                                    maxB = maxB > _e2 + c1 - 1 ? maxB : _e2 + c1 - 1;

                                                    if(maxB != b)
                                                        continue;

                                                    var maxD = d1 > d2 ? d1 : d2;
                                                    maxD = maxD > _e1 + a2 - 1 ? maxD : _e1 + a2 - 1;
                                                    maxD = maxD > _e2 + a1 - 1 ? maxD : _e2 + a1 - 1;

                                                    if(maxD != d)
                                                        continue;

                                                    var value = QCx[a1][b1][c1][d1][e1] + QCy[a2][b2][c2][d2][e2];
                                                    if (value < min)
                                                        min = value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return min;
        }

        private static double MinLRPNode(double[] first, double[] second, int a)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 0; a1 <= a; a1++)
            {
                for (var a2 = 0; a2 <= a; a2++)
                {
                    var maxA = a1 > a2 ? a1 : a2;
                    if(maxA != a || a1 + a2 > _k)
                        continue;

                    var value = first[a1] + second[a2];
                    if (value < min)
                        min = value;
                }
            }
            return min;
        }

        private static void DecideJ(NumeratedConnectedNode nodeZ)
        {
            var nodeX = nodeZ.NodeX;
            var nodeY = nodeZ.NodeY;


            nodeZ.Parameters.Mc = nodeX.Parameters.Mc - nodeX.Node.WR +
                                  MinMCyLCyJNode(nodeY.Parameters.Mc, nodeY.Parameters.LC, _k - 1);
            var s = new double[3];
            for (var a = 1; a <= _k-1; a++)
            {
                nodeZ.Parameters.LC[a] = double.PositiveInfinity;

                if (nodeY.Parameters.b <= a && 
                    nodeY.Parameters.QC[nodeY.Parameters.a][nodeY.Parameters.b][nodeY.Parameters.c][nodeY.Parameters.d][nodeY.Parameters.e] == 0)
                {
                    nodeZ.Parameters.LC[a] = MinLCxJNode(nodeX.Parameters.LC, a, nodeY.Parameters.b);
                }
                nodeZ.Parameters.RC[a] = nodeX.Parameters.RC[a] - nodeZ.Node.WR +
                                         MinMCyLCyJNode(nodeY.Parameters.Mc, nodeY.Parameters.LC, a);

                for (var b = a; b <= _k-1; b++)
                {
                    for (var c = 1; c <= _k - 1; c++)
                    {
                        for (var d = c; d <= _k - 1; d++)
                        {
                            for (var e = 0; e <= _k - 1; e++)
                            {
                                s[0] = double.PositiveInfinity;
                                s[1] = double.PositiveInfinity;
                                if (nodeX.Parameters.a == a && nodeX.Parameters.b <= b && nodeX.Parameters.c <= c
                                    && nodeX.Parameters.d <= d && nodeX.Parameters.e == e
                                    && nodeX.Parameters.QC[nodeX.Parameters.a][nodeX.Parameters.b][nodeX.Parameters.c][
                                        nodeX.Parameters.d][nodeX.Parameters.e] == 0)
                                {
                                    s[0] = MinRyJNode(nodeY.Parameters.RC, b, c, d, nodeX);
                                    s[1] = MinQyJNode(nodeY.Parameters.QC, nodeX, b, c, d);
                                }

                                s[2] = double.PositiveInfinity;

                                if (nodeY.Parameters.a <= b && nodeY.Parameters.b <= d && nodeY.Parameters.c <= b 
                                    && nodeY.Parameters.d <= d && nodeY.Parameters.e <= b 
                                    && nodeY.Parameters.QC[nodeY.Parameters.a][nodeY.Parameters.b][nodeY.Parameters.c][nodeY.Parameters.d][nodeY.Parameters.e] == 0)
                                {
                                    s[2] = MinQxJNode(nodeX.Parameters.QC, nodeY, a, b, c, d, e);
                                }
                                nodeZ.Parameters.QC[a][b][c][d][e] = s.Min();
                            }
                        }
                    }
                }
            }
        }

        private static double MinQxJNode(double[][][][][] Qx, NumeratedConnectedNode nodeY, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;
            for (var b1 = a; b1 <= b; b1++)
            {
                var maxB = b1 > e + nodeY.Parameters.d - 1 ? b1 : e + nodeY.Parameters.d - 1;
                if(maxB != b)
                    continue;

                for (var c1 = 1; c1 <= c; c1++)
                {
                    var maxC = c1 > nodeY.Parameters.b ? c1 : nodeY.Parameters.b;
                    if (maxC != c)
                        continue;

                    for (var d1 = c1; d1 <= d; d1++)
                    {
                        var maxD = d1 > nodeY.Parameters.b ? d1 : nodeY.Parameters.b;
                        if(maxD != d)
                            continue;

                        if (Qx[a][b1][c1][d1][e] < min)
                            min = Qx[a][b1][c1][d1][e];
                    }
                }
            }
            return min;
        }

        private static double MinQyJNode(double[][][][][] Qy, NumeratedConnectedNode nodeX, int b, int c, int d)
        {
            var min = double.PositiveInfinity;
            for (var b2 = 1; b2 <= d; b2++)
            {
                var maxD = nodeX.Parameters.d > b2 ? nodeX.Parameters.d : b2;
                if(maxD != d)
                    continue;

                var maxC = nodeX.Parameters.c > b2 ? nodeX.Parameters.c : b2;
                if(maxC != c)
                    continue;

                var maxB = nodeX.Parameters.b > nodeX.Parameters.e + b2 - 1
                    ? nodeX.Parameters.b
                    : nodeX.Parameters.e + b2 - 1;
                if(maxB != b)
                    continue;

                for (var a2 = 1; a2 <= b2 && a2 <= d; a2++)
                {
                    for (var d2 = 1; d2 <= d; d2++)
                    {
                        for (var c2 = 1; c2 <= d2 && c2 <= d; c2++)
                        {
                            for (var e2 = 0; e2 <= c; e2++)
                            {
                                if (Qy[a2][b2][c2][d2][e2] < min)
                                    min = Qy[a2][b2][c2][d2][e2];
                            }
                        }
                    }
                }
            }
            return min;
        }

        private static double MinRyJNode(double[] Ry, int b, int c, int d, NumeratedConnectedNode nodeX)
        {
            var min = double.PositiveInfinity;
            for (var c2 = 1; c2 <= d; c2++)
            {
                var _ex = nodeX.Parameters.e == 0 ? -_k : nodeX.Parameters.e;
                var maxB = nodeX.Parameters.b > _ex + c2 - 1 ? nodeX.Parameters.b : _ex + c2 - 1;
                if(maxB != b)
                    continue;

                var maxC = nodeX.Parameters.c > c2 ? nodeX.Parameters.c : c2;
                if(maxC != c)
                    continue;

                var maxD = nodeX.Parameters.d > c2 ? nodeX.Parameters.d : c2;
                if(maxD != d)
                    continue;

                if (Ry[c2] < min)
                    min = Ry[c2];
            }
            return min;
        }

        private static double MinLCxJNode(double[] LCx, int a, int by)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                var maxA = a1 > by ? a1 : by;
                if(maxA != a)
                    continue;

                if (LCx[a1] < min)
                    min = LCx[a1];
            }
            return min;
        }

        private static double MinMCyLCyJNode(double MCy, double[] LCy, int limit )
        {
            var min = MCy;

            for (var a = 1; a <= limit; a++)
            {
                if (LCy[a] < min)
                    min = LCy[a];
            }
            return min;
        }


        private static void DecideKnote(NumeratedConnectedNode nodeZ)
        {
            nodeZ.Parameters.Mc = nodeZ.Node.WL + nodeZ.Node.WR;
            nodeZ.Parameters.LC[1] = nodeZ.Node.WL;
            nodeZ.Parameters.RC[1] = nodeZ.Node.WR;

            for (var a = 2; a <= _k - 1; a++)
            {
                nodeZ.Parameters.LC[a] = double.PositiveInfinity;
                nodeZ.Parameters.RC[a] = double.PositiveInfinity;
            }

            for (var a = 1; a <= _k - 1; a++)
            {
                for (var b = a; b <= _k - 1; b++)
                {
                    for (var c = 1; c <= _k - 1; c++)
                    {
                        for (var d = c; d <= _k - 1; d++)
                        {
                            for (var e = 0; e <= _k - 1; e++)
                            {
                                nodeZ.Parameters.QC[a][b][c][d][e] =
                                    a == 1 && c == 1 && b == 2 && d == 2 && e == 2 
                                    ? 0 : double.PositiveInfinity;
                            }
                        }
                    }
                }
            }
        }
    }
}
