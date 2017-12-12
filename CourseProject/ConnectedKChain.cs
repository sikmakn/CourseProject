using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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
            nodes[nodes.Count - 1].Parameters.Lc[0] = double.PositiveInfinity;
            s[0] = nodes[nodes.Count - 1].Parameters.Lc.Min();
            nodes[nodes.Count - 1].Parameters.Rc[0] = double.PositiveInfinity;
            s[1] = nodes[nodes.Count - 1].Parameters.Rc.Min();
            s[2] = nodes[nodes.Count - 1].Parameters.Qc.Min(k);
            s[3] = nodes[nodes.Count - 1].Parameters.Mc;
            return s.Min();
        }

        private static void DecideS(NumeratedConnectedNode nodeZ)
        {
            var nodeX = nodeZ.NodeX;
            var nodeY = nodeZ.NodeY;
            var ex = nodeX.Parameters.E == 0 ? -_k : nodeX.Parameters.E;
            var exIndex = nodeX.Parameters.E == -_k ? 0 : nodeX.Parameters.E;
            var ey = nodeY.Parameters.E == 0 ? -_k : nodeY.Parameters.E;
            var eyIndex = nodeY.Parameters.E == -_k ? 0 : nodeY.Parameters.E;
            nodeZ.Parameters.A = nodeX.Parameters.A > ex + nodeY.Parameters.A - 1
                ? nodeX.Parameters.A
                : ex + nodeY.Parameters.A - 1;
            nodeZ.Parameters.B = nodeX.Parameters.B > ex + nodeY.Parameters.B - 1
                ? nodeX.Parameters.B
                : ex + nodeY.Parameters.B - 1;
            nodeZ.Parameters.C = nodeY.Parameters.C > ey + nodeX.Parameters.C - 1
                ? nodeY.Parameters.C
                : ey + nodeX.Parameters.C - 1;
            nodeZ.Parameters.D = nodeY.Parameters.D > ey + nodeX.Parameters.D - 1
                ? nodeY.Parameters.D
                : ey + nodeX.Parameters.D - 1;
            nodeZ.Parameters.E = ex + ey - 1;
            var s = new double[3];
            s[0] = nodeX.Parameters.Mc + nodeY.Parameters.Mc - nodeX.Node.WR;
            s[1] = double.PositiveInfinity;
            if (Math.Abs(nodeY.Parameters.Rc[nodeY.Parameters.A]) < 0.0001)
            {
                s[1] = MinLCxSNode(nodeX.Parameters.Lc);
            }
            s[2] = double.PositiveInfinity;
            if (Math.Abs(nodeX.Parameters.Lc[nodeX.Parameters.C]) < 0.001)
            {
                s[2] = MinLCxSNode(nodeY.Parameters.Rc);
            }
            nodeZ.Parameters.Mc = s.Min();

            s[2] = double.PositiveInfinity;

            var s2 = new double[2];
            var s4 = new double[4];
            for (var a = 1; a < _k - 1; a++)
            {
                s2[0] = nodeX.Parameters.Mc + nodeY.Parameters.Lc[a] - nodeX.Node.WR;
                s2[1] = double.PositiveInfinity;
                if (Math.Abs(nodeY.Parameters.Qc[nodeY.Parameters.A][nodeY.Parameters.B][nodeY.Parameters.C]
                        [nodeY.Parameters.D][eyIndex]) < 0.001)
                {
                    s2[1] = MinLCxSNode(nodeX.Parameters.Lc, nodeY.Parameters, a);
                }
                nodeZ.Parameters.Lc[a] = s2.Min();

                s2[0] = nodeX.Parameters.Rc[a] + nodeY.Parameters.Mc - nodeX.Node.WR;
                s2[1] = double.PositiveInfinity;
                if (Math.Abs(nodeX.Parameters.Qc[nodeX.Parameters.A][nodeX.Parameters.B][nodeX.Parameters.C]
                        [nodeX.Parameters.D][exIndex]) < 0.001)
                {
                    s2[1] = MinRySNode(nodeX.Parameters, nodeY.Parameters.Rc, a);
                }
                nodeZ.Parameters.Rc[a] = s2.Min();

                for (var b = a; b <= _k-1; b++)
                {
                    s4[1] = double.PositiveInfinity;

                    if (Math.Abs(nodeY.Parameters.Qc[nodeY.Parameters.A][nodeY.Parameters.B][nodeY.Parameters.C]
                            [nodeY.Parameters.D][eyIndex]) < 0.001)
                    {
                        s4[1] = MinQcxSNode(nodeX.Parameters.Qc, a, b);
                    }

                    for (var c = 1; c <= _k-1; c++)
                    {
                        s4[0] = nodeX.Parameters.Rc[a] + nodeY.Parameters.Lc[c] - nodeX.Node.WR;
                        for (var d = c; d <= _k-1; d++)
                        {
                            s4[2] = double.PositiveInfinity;
                            if (Math.Abs(nodeX.Parameters.Qc[nodeX.Parameters.A][nodeX.Parameters.B][nodeX.Parameters.C]
                                    [nodeX.Parameters.D][exIndex]) < 0.001)
                            {
                                s4[2] = MinQcySNode(nodeY.Parameters.Qc, c, d);
                            }
                            s4[3] = MinS4SNode(nodeX.Parameters.Qc, nodeY.Parameters.Qc, a, b, c, d);
                            nodeZ.Parameters.Qc[a][b][c][d][0] = s4.Min();
                            nodeZ.Parameters.Qc[a][b][c][d][1] = double.PositiveInfinity;
                            for (var e = 2; e <= _k-1; e++)
                            {
                                nodeZ.Parameters.Qc[a][b][c][d][e] = MinQczSNode(nodeX.Parameters.Qc,
                                    nodeY.Parameters.Qc, a, b, c, d, e);
                            }
                        }
                    }
                }

            }
        }

        private static double MinQczSNode(double[][][][][] qCx, double[][][][][] qCy, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;

            var _e = e == 0 ? -_k : e;

            var minAC = a < c ? a : c;
            var minBD = b < d ? b : d;
            var minAE = a < _e ? a : _e;
            var minCE = c > _e ? c : _e;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var b1 = 1; b1 <= minAC; b1++)
                {
                    for (var c1 = 1; c1 <= c; c1++)
                    {
                        for (var d1 = c1; d1 <= d; d1++)
                        {
                            for (var a2 = 1; a2 <= minAC; a2++)
                            {
                                for (var b2 = a2; b2 <= minBD; b2++)
                                {
                                    for (var c2 = 1; c2 <= c; c2++)
                                    {
                                        for (var d2 = c2; d2 <= d; d2++)
                                        {
                                            if(d1 + b2 > _k)
                                                continue;

                                            for (var e1 = 1; e1 <= minAE; e1++)
                                            {
                                                var maxA = a1 > e1 + a2 - 1 ? a1 : e1 + a2 - 1;
                                                if(maxA != a)
                                                    continue;

                                                var maxB = b1 > e1 + b2 - 1 ? b1 : e1 + b2 - 1;
                                                if(maxB != b)
                                                    continue;

                                                for (var e2 = 1; e2 <= minCE; e2++)
                                                {
                                                    var maxC = c2 > e2 + c1 - 1 ? c2 : e2 + c1 - 1;
                                                    if(maxC != c)
                                                        continue;

                                                    var maxD = d2 > e2 + d1 - 1 ? d2 : e2 + d1 - 1;
                                                    if (maxD != d)
                                                        continue;

                                                    var value = qCx[a1][b1][c1][d1][e1] + qCy[a2][b2][c2][d2][e2];

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

        private static double MinS4SNode(double[][][][][] qCx, double[][][][][] qCy, int a, int b, int c, int d)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= b; a1++)
            {
                for (var b1 = a1; a1 <= b1 && b1 <= b; b1++)
                {
                    for (var c1 = 1; c1 <= _k-1; c1++)
                    {
                        for (var d1 = c1; c1 <= d1 && d1 <= _k-1; d1++)
                        {
                            for (var a2 = 1; a2 <= _k - 1; a2++)
                            {
                                for (var b2 = a2; a2 <= b2 && b2 <= _k - 1; b2++)
                                {
                                    if(d1 + b2 >= _k)
                                        continue;
                                    
                                    for (var c2 = 1; c2 <= c; c2++)
                                    {
                                        for (var d2 = c2; d2 <= d; d2++)
                                        {
                                            for (var e1 = 0; e1 <= b; e1++)
                                            {
                                                var _e1 = e1 == 0 ? -_k : e1;

                                                var maxA = a1 > _e1 + a2 - 1 ? a1 : _e1 + a2 - 1;
                                                if(maxA != a)
                                                    continue;

                                                var maxB = b1 > _e1 + b2 - 1 ? b1 : _e1 + b2 - 1;
                                                if (maxB != b)
                                                    continue;
                                                
                                                for (var e2 = 0; e2 <= d; e2++)
                                                {
                                                    var _e2 = e2 == 0 ? -_k : e2;

                                                    var maxC = c2 > _e2 + c1 - 1 ? c2 : _e2 + c1 - 1;
                                                    if (maxC != c)
                                                        continue;

                                                    var maxD = d2 > _e2 + d1 - 1 ? d2 : _e2 + d1 - 1;
                                                    if(maxD != d)
                                                        continue;

                                                    if (_e1 != -_k && _e2 != -_k)
                                                        continue;

                                                    var value = qCx[a1][b1][c1][d1][e1] + qCy[a2][b2][c2][d2][e2];

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

        private static double MinQcySNode(double[][][][][] QCy, int c, int d)
        {
            var min = double.PositiveInfinity;
            for (var a2 = 1; a2 <= _k-1; a2++)
            {
                for (var b2 = a2; a2<=b2 && b2 <= _k-1; b2++)
                {
                    if (QCy[a2][b2][c][d][0] < min)
                        min = QCy[a2][b2][c][d][0];
                }
            }
            return min;
        }

        private static double MinQcxSNode(double[][][][][] QCx, int a, int b)
        {
            var min = double.PositiveInfinity;
            for (var c1 = 1; c1 <= _k-1; c1++)
            {
                for (int d1 = c1; d1 <= _k-1 && c1 <= d1; d1++)
                {

                    if (QCx[a][b][c1][d1][0] < min)
                        min = QCx[a][b][c1][d1][0];
                }
            }
            return min;
        }
        private static double MinRySNode(ConnectedParameters parametersX, double[] Ry, int a)
        {
            var min = double.PositiveInfinity;

            var ex = parametersX.E == 0 ? -_k : parametersX.E;
            for (var a2 = 1; a2 <= _k-1; a2++)
            {
                if (parametersX.D + a2 > _k)
                    continue;

                var maxA = parametersX.A > ex + a2 - 1 ? parametersX.A : ex + a2 - 1;
                if (maxA != a)
                    continue;

                if (Ry[a2] < min)
                    min = Ry[a2];
            }
            return min;
        }

        private static double MinLCxSNode(double[] LC, ConnectedParameters parametersY, int a)
        {
            var min = double.PositiveInfinity;
            var ey = parametersY.E == 0 ? -_k : parametersY.E;
            for (var c1 = 1; c1 <= _k-1; c1++)
            {
                if(c1 + parametersY.B > _k)
                    continue;

                var maxA = parametersY.C > ey + c1 ? parametersY.C : ey + c1;
                if (maxA != a)
                    continue;

                if (LC[c1] < min)
                {
                    min = LC[c1];
                }
            }
            return min;
        }

        private static double MinLCxSNode(double[] LC)
        {
            var min = double.PositiveInfinity;
            for (var c = 1; c <= _k-1; c++)
            {
                if (LC[c] < min)
                {
                    min = LC[c];
                }
            }
            return min;
        }

        private static void DecideP(NumeratedConnectedNode nodeZ)
        {
            var nodeX = nodeZ.NodeX;
            var nodeY = nodeZ.NodeY;

            var ex = nodeX.Parameters.E == 0 ? -_k : nodeX.Parameters.E;
            var ey = nodeY.Parameters.E == 0 ? -_k : nodeY.Parameters.E;
            nodeZ.Parameters.A = nodeX.Parameters.A > nodeY.Parameters.A 
                ? nodeX.Parameters.A : nodeY.Parameters.A;
            nodeZ.Parameters.C = nodeY.Parameters.C > nodeX.Parameters.C 
                ? nodeY.Parameters.C : nodeX.Parameters.C;
            nodeZ.Parameters.E = ey > ex
                ? ey : ex;

            var s = new int[4];
            s[0] = nodeY.Parameters.B;
            s[1] = nodeX.Parameters.B;
            s[2] = ex + nodeY.Parameters.C;
            s[3] = ey + nodeX.Parameters.C;
            nodeZ.Parameters.B = s.Max();

            s[0] = nodeY.Parameters.D;
            s[1] = nodeX.Parameters.D;
            s[2] = ex + nodeY.Parameters.A;
            s[3] = ey + nodeX.Parameters.A;
            nodeZ.Parameters.D = s.Max();

            nodeZ.Parameters.Mc = nodeY.Parameters.Mc + nodeX.Parameters.Mc 
                - nodeZ.Node.WL - nodeZ.Node.WR;

            for (var a = 1; a <= _k - 1; a++)
            {
                nodeZ.Parameters.Lc[a] = MinLrpNode(nodeX.Parameters.Lc, nodeY.Parameters.Lc, a)
                    - nodeX.Node.WL;
                nodeZ.Parameters.Rc[a] = MinLrpNode(nodeX.Parameters.Rc, nodeY.Parameters.Rc, a)
                    - nodeX.Node.WR;

                for (var b = 1; b < _k-1; b++)
                {
                    for (var c = 1; c < _k - 1; c++)
                    {
                        for (var d = 1; d < _k - 1; d++)
                        {
                            for (var e = 0; e < _k - 1; e++)
                            {
                                nodeZ.Parameters.Qc[a][b][c][d][e] = MinQcxQcyPNode(nodeX.Parameters.Qc,
                                    nodeY.Parameters.Qc, a, b, c, d, e);
                            }
                        }
                    }
                }
            }
        }

        private static double MinQcxQcyPNode(double[][][][][] QCx, double[][][][][] QCy, int a, int b, int c, int d, int e)
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

                                    for (var d1 = 1; d1 <= d; d1++)
                                    {
                                        for (var d2 = 1; d2 <= d; d2++)
                                        {
                                            for (var e1 = 0; e1 <= e; e1++)
                                            {
                                                for (var e2 = 0; e2 <= e; e2++)
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

        private static double MinLrpNode(double[] first, double[] second, int a)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var a2 = 1; a2 <= a; a2++)
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
                                  MinMcyLcyJNode(nodeY.Parameters.Mc, nodeY.Parameters.Lc, _k - 1);
            var s = new double[3];

            var ey = nodeY.Parameters.E == 0 ? -_k : nodeY.Parameters.E;
            var eyIndex = nodeY.Parameters.E == -_k ? 0 : nodeY.Parameters.E;
            var ex = nodeX.Parameters.E == 0 ? -_k : nodeX.Parameters.E;
            var exIndex = nodeX.Parameters.E == -_k ? 0 : nodeX.Parameters.E;
            for (var a = 1; a <= _k-1; a++)
            {
                nodeZ.Parameters.Lc[a] = double.PositiveInfinity;

                if (nodeY.Parameters.B <= a && 
                    Math.Abs(nodeY.Parameters.Qc[nodeY.Parameters.A][nodeY.Parameters.B][nodeY.Parameters.C][nodeY.Parameters.D][eyIndex]) < 0.001)
                {
                    nodeZ.Parameters.Lc[a] = MinLcxJNode(nodeX.Parameters.Lc, a, nodeY.Parameters.B);
                }
                nodeZ.Parameters.Rc[a] = nodeX.Parameters.Rc[a] - nodeZ.Node.WR +
                                         MinMcyLcyJNode(nodeY.Parameters.Mc, nodeY.Parameters.Lc, a);

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
                                if (nodeX.Parameters.A == a && nodeX.Parameters.B <= b && nodeX.Parameters.C <= c
                                    && nodeX.Parameters.D <= d && exIndex == e
                                    && Math.Abs(nodeX.Parameters.Qc[nodeX.Parameters.A][nodeX.Parameters.B][nodeX.Parameters.C][
                                        nodeX.Parameters.D][exIndex]) < 0.001)
                                {
                                    s[0] = MinRyJNode(nodeY.Parameters.Rc, b, c, d, nodeX);
                                    s[1] = MinQyJNode(nodeY.Parameters.Qc, nodeX, b, c, d);
                                }

                                s[2] = double.PositiveInfinity;

                                if (nodeY.Parameters.A <= b && nodeY.Parameters.B <= d && nodeY.Parameters.C <= b 
                                    && nodeY.Parameters.D <= d && eyIndex <= b 
                                    && Math.Abs(nodeY.Parameters.Qc[nodeY.Parameters.A][nodeY.Parameters.B][nodeY.Parameters.C][nodeY.Parameters.D][eyIndex]) < 0.001)
                                {
                                    s[2] = MinQxJNode(nodeX.Parameters.Qc, nodeY, a, b, c, d, e);
                                }
                                nodeZ.Parameters.Qc[a][b][c][d][e] = s.Min();
                            }
                        }
                    }
                }
            }
        }

        private static double MinQxJNode(double[][][][][] Qx, NumeratedConnectedNode nodeY, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;
            var _e = e == 0 ? -_k : e;
            for (var b1 = a; b1 <= b; b1++)
            {
                var maxB = b1 > _e + nodeY.Parameters.D - 1 ? b1 : _e + nodeY.Parameters.D - 1;
                if(maxB != b)
                    continue;

                for (var c1 = 1; c1 <= c; c1++)
                {
                    var maxC = c1 > nodeY.Parameters.B ? c1 : nodeY.Parameters.B;
                    if (maxC != c)
                        continue;

                    for (var d1 = c1; d1 <= d; d1++)
                    {
                        var maxD = d1 > nodeY.Parameters.B ? d1 : nodeY.Parameters.B;
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
            var ex = nodeX.Parameters.E == 0 ? -_k : nodeX.Parameters.E;
            for (var a2 = 1; a2 <= d; a2++)
            {
                for (var b2 = a2; a2 <= b2 && b2 <= d; b2++)
                {
                    var maxD = nodeX.Parameters.D > b2 ? nodeX.Parameters.D : b2;
                    if (maxD != d)
                        continue;

                    var maxC = nodeX.Parameters.C > b2 ? nodeX.Parameters.C : b2;
                    if (maxC != c)
                        continue;

                    var maxB = nodeX.Parameters.B > ex + b2 - 1
                        ? nodeX.Parameters.B
                        : ex + b2 - 1;
                    if (maxB != b)
                        continue;

                    for (var c2 = 1; c2 <= d; c2++)
                    {
                        for (var d2 = c2; c2 <= d2 && d2 <= d; d2++)
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
                var _ex = nodeX.Parameters.E == 0 ? -_k : nodeX.Parameters.E;
                var maxB = nodeX.Parameters.B > _ex + c2 - 1 ? nodeX.Parameters.B : _ex + c2 - 1;
                if(maxB != b)
                    continue;

                var maxC = nodeX.Parameters.C > c2 ? nodeX.Parameters.C : c2;
                if(maxC != c)
                    continue;

                var maxD = nodeX.Parameters.D > c2 ? nodeX.Parameters.D : c2;
                if(maxD != d)
                    continue;

                if (Ry[c2] < min)
                    min = Ry[c2];
            }
            return min;
        }

        private static double MinLcxJNode(double[] LCx, int a, int by)
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

        private static double MinMcyLcyJNode(double MCy, double[] LCy, int limit )
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

            for (var a = 0; a <= _k - 1; a++)
            {
                nodeZ.Parameters.Lc[a] = double.PositiveInfinity;
                nodeZ.Parameters.Rc[a] = double.PositiveInfinity;
            }
            nodeZ.Parameters.Lc[1] = nodeZ.Node.WL;
            nodeZ.Parameters.Rc[1] = nodeZ.Node.WR;

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
                                nodeZ.Parameters.Qc[a][b][c][d][e] = double.PositiveInfinity;
                            }
                        }
                    }
                }
            }
            if (_k > 2)
                nodeZ.Parameters.Qc[1][2][1][2][2] = 0;
        }

    }
}
