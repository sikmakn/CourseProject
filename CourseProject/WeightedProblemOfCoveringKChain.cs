using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

namespace CourseProject
{
    public class WeightedProblemOfCoveringKChain
    {
        public double Decide(int k, int v, List<NumeratedNode> T)
        {
            if (v <= k)
                return 0;

            foreach (var z in T)
            {
                var node = z.Node;
                switch (node.Type)
                {
                    case Node.NodeType.Knot:
                        DecideKnot(z, k);
                        break;
                    case Node.NodeType.S:
                        DecideS(z, k);
                        break;
                    case Node.NodeType.P:
                        DecideP(z, k);
                        break;
                    case Node.NodeType.J:
                        DecideJ(z, k);
                        break;
                }
            }
            var s = new double[4];
            s[0] = T[T.Count - 1].Parameters.L.Min();
            s[1] = T[T.Count - 1].Parameters.R.Min();
            s[2] = T[T.Count - 1].Parameters.Q.Min();
            return s.Min();
        }


        private static void DecideKnot(NumeratedNode node, int k)
        {
            var parameters = node.Parameters;
            var knot = node.Node;
            parameters.L[0] = knot.WL;
            parameters.R[0] = knot.WR;
            for (var i = 0; i < k; i++)
            {
                parameters.L[i] = double.PositiveInfinity;
                parameters.R[i] = double.PositiveInfinity;
            }
            for (var a = 1; a < parameters.Q.Length; a++)
            {
                for (var b = a; b < parameters.Q[a].Length; b++)
                {
                    for (var c = 1; c < parameters.Q[a][b].Length; c++)
                    {
                        for (var d = c; d < parameters.Q[a][b][c].Length; d++)
                        {
                            for (var e = 0; e < parameters.Q[a][b][c][d].Length; e++)
                            {
                                parameters.Q[a][b][c][d][e] = DecideKnotQ(a, b, c, d, e, k);
                            }
                        }
                    }
                }
            }
            node.Parameters.M = knot.WL + knot.WR;
        }

        private static double DecideKnotQ(int a, int b, int c, int d, int e, int k)
        {
            if (a == 1 && c == 1 && b == 2 && d == 2 && e == 2 + k + 1)
                return 0;

            return double.PositiveInfinity;
        }

        private static void DecideS(NumeratedNode node, int k)
        {
            var nodeX = node.NodeX;
            var nodeY = node.NodeY;
            var s = new double[2];
            s[0] = nodeX.Parameters.M + nodeY.Parameters.M - node.Node.WR;
            s[1] = MinLxRy(nodeX.Parameters.L, nodeY.Parameters.R, k);
            node.Parameters.M = s.Min();

            for (var a = 1; a < k; a++)
            {
                s[0] = nodeX.Parameters.M + nodeY.Parameters.L[a] - node.Node.WR;
                s[1] = MinLxQy(nodeX.Parameters.L, nodeY.Parameters.Q, k, a);
                node.Parameters.L[a] = s.Min();

                s[0] = nodeX.Parameters.R[a] + nodeX.Parameters.M - node.Node.WR;
                s[1] = MinQxRy(nodeX.Parameters.Q, nodeY.Parameters.R, k, a);
                node.Parameters.R[a] = s.Min();

                for (var c = a; c < k; c++)
                {
                    s[0] = nodeX.Parameters.R[a] + nodeY.Parameters.L[c] - node.Node.WR;
                    s[1] = MinQzQy(node.Parameters.Q, nodeY.Parameters.Q, k, a, c);
                    node.Parameters.Q[a][a][c][c][0] = s.Min();

                    for (var b = 1; b <= k-1; b++)
                    {
                        for (var d = c; d <= k-1; d++)
                        {
                            node.Parameters.Q[a][b][c][d][1] = double.PositiveInfinity;
                            for (var e = 2; e <= k - 1; e++)
                            {
                                node.Parameters.Q[a][b][c][d][e] = MinQxQy(nodeX.Parameters.Q, nodeY.Parameters.Q, k, a,
                                    b, c, d, e);
                            }
                        }
                    }
                }
            }
        }

        private static double MinQxQy(double[][][][][] Qx, double[][][][][] Qy, int k, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;

            var minE1 = a < e - 1 ? a : e - 1;
            var minA2 = a < c ? a : c;
            var minB2 = b < d ? b : d;
            var minE2 = c < e - 1 ? c : e - 1;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var b1 = a1; b1 <= a; b1++)
                {
                    for (var c1 = 1; c1 <= c; c1++)
                    {
                        for (var d1 = c1; d1 <= c; d1++)
                        {
                            for (var e1 = 1; e1 < minE1; e1++)
                            {
                                for (var a2 = 1; a2 <= minA2; a2++)
                                {
                                    var flag1 = b1 > e1 + a2 - 1 ? b1 : e1 + a2 - 1;
                                    if (flag1 != a)
                                        continue;

                                    for (var b2 = a2; b2 <= minB2; b2++)
                                    {
                                        var flag2 = b1 > e1 + b2 - 1 ? b1 : e1 + b2 - 1;
                                        if (flag2 != b)
                                            continue;

                                        if (d1 + b2 > k)
                                            continue;

                                        for (var c2 = 1; c2 <= c; c2++)
                                        {
                                            for (var d2 = c2; d2 <= c; d2++)
                                            {
                                                for (var e2 = 0; e2 <= minE2; e2++)
                                                {
                                                    var flag3 = d2 > e2 + c1 - 1 ? d2 : e2 + c1 - 1;
                                                    if (flag3 != c)
                                                        continue;

                                                    var flag4 = d2 > e2 + d1 - 1 ? d2 : e2 + d1 - 1;
                                                    if (flag4 != d)
                                                        continue;
                                                    if (e1 + e2 - 1 != e)
                                                        continue;

                                                    var value = Qx[a1][b1][c1][d1][e1] +
                                                                Qy[a2][b2][c2][d2][e2];
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

        private static double MinQzQy(double[][][][][] Qz, double[][][][][] Qy, int k, int a, int c)
        {
            var min = double.PositiveInfinity;

            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var b1 = a1; b1 <= a; b1++)
                {
                    for (var c1 = 1; c1 <= k-1; c1++)
                    {
                        for (var d1 = c1; d1 <= k-1; d1++)
                        {
                            for (var a2 = 1; a2 <= k-1; a2++)
                            {
                                for (var b2 = a2; b2 <= k-1; b2++)
                                {
                                    for (var c2 = 1; c2 <= c; c2++)
                                    {
                                        for (var d2 = c2; d2 <= c; d2++)
                                        {
                                            if (d1 + b2 > k)
                                                continue;

                                            for (var e1 = -k; e1 <= a; e1++)
                                            {
                                                for (var e2 = -k; e2 <= c; e2++)
                                                {
                                                    if (e1 != -k && e2 != -k)
                                                        continue;

                                                    var flag1 = b1 > e1 + a2 - 1 ? b1 : e1 + a2 - 1;
                                                    if (flag1 != a)
                                                        continue;

                                                    var flag2 = d2 > e2 + c1 - 1 ? d2 : e2 + c1 - 1;
                                                    if (flag2 != c)
                                                        continue;

                                                    var value = Qz[a1][b1][c1][d1][e1 + k + 1] +
                                                                Qy[a2][b2][c2][d2][e2 + k + 1];
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

        private static double MinQxRy(double[][][][][] Qx, double[] Ry, int k, int a)
        {
            var min = double.PositiveInfinity;
            for (var d1 = 1; d1 < k; d1++)
            {
                for (var c1 = 1; c1 < k && c1 <= d1; c1++)
                {
                    for (var a2 = 1; a2 < k; a2++)
                    {
                        if (d1 + a2 > k)
                            continue;

                        for (var b1 = 1; b1 <= a; b1++)
                        {
                            for (var a1 = 1; a1 <= a && a1 <= b1; a1++)
                            {
                                for (var e = -k; e <= a; e++)
                                {
                                    var flag = b1 > e + a2 - 1 ? b1 : e + a2 - 1;
                                    if (flag != a)
                                        continue;

                                    var value = Qx[a1][b1][c1][d1][e + k + 1] + Ry[a2];
                                    if (value < min)
                                        min = value;
                                }
                            }
                        }

                    }
                }
            }
            return min;
        } 

        private static double MinLxQy(double[] Lx, double[][][][][] Qy, int k, int a)
        {
            var min = double.PositiveInfinity;
            for (var c1 = 1; c1 < k; c1++)
            {
                for (var b2 = 1; b2 < k; b2++)
                {
                    if (c1 + b2 > k)
                        continue;

                    for (var a2 = 1; a2 < k && a2 <= b2; a2++)
                    {
                        for (var e = -k; e <= a; e++)
                        {
                            for (var d2 = 1; d2 <= a; d2++)
                            {
                                for (var c2 = 1; c2 <= d2 && c1 <= a; c2++)
                                {
                                    var flag = d2 > e + c1 ? d2 : e + c1;
                                    if (flag != a)
                                        continue;

                                    var value = Lx[c1] + Qy[a2][b2][c2][d2][e + k + 1];
                                    if (value < min)
                                        min = value;

                                }
                            }
                        }

                    }
                }
            }
            return min;
        }

        private static double MinLxRy(double[] Lx, double[] Ry, int k)
        {
            var min = double.PositiveInfinity;

            for (var c = 1; c < k; c++)
            {
                for (var a = 1; a < k; a++)
                {
                    if (c + a > k)
                        continue;

                    var value = Lx[c] + Ry[a];
                    if (value < min)
                        min = value;
                }
            }
            return min;
        }

        private void DecideP(NumeratedNode node, int k)
        {
            var nodeX = node.NodeX;
            var nodeY = node.NodeY;
            var nodeZ = node.Node;
            node.Parameters.M = nodeX.Parameters.M + nodeY.Parameters.M - nodeZ.WL - nodeZ.WR;
            
            for (var a = 1; a <= k-1; a++)
            {
                node.Parameters.L[a] = MinLRPNode(nodeX.Parameters.L, nodeY.Parameters.L, k, a) - nodeZ.WL;
                node.Parameters.R[a] = MinLRPNode(nodeX.Parameters.R, nodeY.Parameters.R, k, a) - nodeZ.WR;

                for (var b = 1; b <=  k-1; b++)
                {
                    for (var c = 1; c <= k-1; c++)
                    {
                        for (var d = 1; d <= k - 1; d++)
                        {
                            for (var e = -k; e <= k-1; e++)
                            {
                                node.Parameters.Q[a][b][c][d][e] = MinQxQyPNode(nodeX.Parameters.Q, nodeY.Parameters.Q,
                                    k, a, b, c, d, e);
                            }
                        }
                    }
                }
            }
        }

        private static double MinQxQyPNode(double[][][][][] Qx, double[][][][][] Qy, int k, int a, int b, int c, int d,
            int e)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var a2 = 1; a2 <= a; a2++)
                {
                    var maxA = a1 > a2 ? a1 : a2;
                    if(maxA != a)
                        continue;
        
                    for (var c1 = 1; c1 <= c; c1++)
                    {
                        for (var c2 = 1; c2 <= c; c2++)
                        {
                            var maxC = c1 > c2 ? c1 : c2;
                            if (maxC != c)
                                continue;

                            for (var e1 = -k; e1 <= e; e1++)
                            {
                                for (var e2 = -k; e2 <= e; e2++)
                                {
                                    var maxE = e1 > e2 ? e1 : e2;
                                    if (maxE != e)
                                        continue;

                                    for (var b1 = 1; b1 <= b; b1++)
                                    {
                                        for (var b2 = 1; b2 < b; b2++)
                                        {
                                            var flag1 = b1 > b2 ? b1 : b2;
                                            var flag2 = e1 + c2 - 1 > e2 + c1 - 1 ? e1 + c2 - 1 : e2 + c1 - 1;
                                            var flag = flag1 > flag2 ? flag1 : flag2;
                                            if (flag != b)
                                                continue;

                                            for (var d1 = 1; d1 <= d; d1++)
                                            {
                                                for (var d2 = 1; d2 <= d; d2++)
                                                {
                                                    flag1 = d1 > d2 ? d1 : d2;
                                                    flag2 = e1 + a2 - 1 > e2 + a1 - 1 ? e1 + a2 - 1 : e2 + a1 - 1;
                                                    flag = flag1 > flag2 ? flag1 : flag2;
                                                    if (flag != d)
                                                        continue;

                                                    var value = Qx[a1][b1][c1][d1][e1 + k + 1] +
                                                                Qy[a2][b2][c2][d2][e2 + k + 1];
                                                    if (value < min)
                                                    {
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
            }

            return min;
        }

        private static double MinLRPNode(double[] x, double[] y, int k, int a)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var a2 = 1; a2 <= a; a2++)
                {
                    if (a1 + a2 > k)
                        continue;

                    var value = x[a1] + y[a2];
                    if (value < min)
                        min = value;
                }
            }
            return min;
        }

        private void DecideJ(NumeratedNode nodeZ, int k)
        {
            var nodeX = nodeZ.NodeX;
            var nodeY = nodeZ.NodeY;

            var minLy = MinLJNode(nodeY.Parameters.L, k);
            var minMy = nodeY.Parameters.M < minLy ? nodeY.Parameters.M : minLy;
            nodeZ.Parameters.M = nodeX.Parameters.M + minMy - nodeX.Node.WR;

            var s = new double[2];

            for (var a = 1; a <= k-1; a++)
            {
                //s[0] = MinLxRyJNode(nodeX.Parameters.L, nodeY.Parameters.R, k, a);//todo
                //s[1] = ;
                //nodeZ.Parameters.L[a] = s.Min();

                
                nodeZ.Parameters.R[a] = nodeX.Parameters.R[a] - nodeX.Node.WR + minMy;
                for (var b = a; b <= k-1; b++)
                {
                    for (var c = 1; c <= k - 1; c++)
                    {
                        for (var d = c; d <= k - 1; d++)
                        {
                            for (var e = -k; e <= k - 1; e++)
                            {
                                s[0] = MinQxRyJNode(nodeX.Parameters.Q, nodeY.Parameters.R, k, a, b, c, d, e);
                                s[1] = MinQxQyJNode(nodeX.Parameters.Q, nodeY.Parameters.Q, k, a, b, c, d, e);
                                nodeZ.Parameters.Q[a][b][c][d][e] = s.Min();
                            }
                        }
                    }
                }
            }
        }

        private static double MinQxQyJNode(double[][][][][] Qx, double[][][][][] Qy, int k, int a, int b,
            int c, int d, int e)
        {
            var min = double.PositiveInfinity;

            for (var b1 = 1; b1 <= b; b1++)
            {
                for (var c1 = 1; c1 <= c; c1++)
                {
                    for (var d1 = c1; d1 <= d; d1++)
                    {
                        for (var b2 = 1; b2 <= c; b2++)
                        {
                            var maxD = d1 > b2 ? d1 : b2;

                            var maxC = c1 > b2 ? c1 : b2;
                            if(maxC != c || maxD != d || d1 + b2 > k)
                                continue;
                            
                            for (var a2 = 0; a2 <= b2 && a2 <= c; a2++)
                            {
                                for (var d2 = 1; d2 <= k-1; d2++)
                                {
                                    var maxB = b1 > e + d2 - 1? b1 : e + d2 - 1;
                                    if(maxB != b)
                                        continue;
                                    
                                    for (var c2 = 1; c2 <= d2 && c2 <= k-1; c2++)
                                    {
                                        for (var e2 = -k; e2 <= d; e2++)
                                        {
                                            var value = Qx[a][b1][c1][d1][e] + Qy[a2][b2][c2][d2][e2];
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
            return min;
        }

        private static double MinQxRyJNode(double[][][][][] Qx, double[] Ry, int k, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;

            for (var b1 = 1; b1 <= b; b1++)
            {
                for (var c1 = 1; c1 <= c; c1++)
                {
                    for (var c2 = 2; c2 <= c; c2++)
                    {
                        var maxC = c1 > c2 ? c1 : c2;
                        if(maxC != c)
                            continue;
                        
                        for (var d1 = c1; d1 <= d; d1++)
                        {
                            var maxD = d1 > c2 ? d1 : c2;
                            if (maxD != d || d1 + c2 > k)
                                continue;
                            var maxB = b1 > e + c2 - 1 ? b1 : e + c2 - 1;
                            if(maxB != b)
                                continue;

                            var value = Qx[a][b1][c1][d1][e + k + 1] + Ry[c2];
                            if (value < min)
                                min = value;
                        }
                    }
                }
            }
            return min;
        }

        private static double MinLJNode(double[] L, int k)
        {
            var min = double.PositiveInfinity;

            for (var a = 1; a <= k - 1; a++)
            {
                if (L[a] < min)
                {
                    min = L[a];
                }
            }
            return min;
        }

        private static double MinLxRyJNode(double[] Lx, double[] Ry, int k, int a)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                for (var a2 = 1; a2 <= a; a2++)
                {
                    var maxA = a1 > a2 ? a1 : a2;
                    if (maxA != a || a1 + a2 > k)
                        continue;

                    var value = Lx[a1] + Ry[a2];
                    if (value < min)
                    {
                        min = value;
                    }
                }
            }
            return min;
        }

        private static double MinLxQyJNode(double[] Lx, double[][][][][] Qy, int k, int a, int b, int c, int d, int e)
        {
            var min = double.PositiveInfinity;
            for (var a1 = 1; a1 <= a; a1++)
            {
                if (a1 + b > k)
                    continue;

                for (var a2 = 1; a2 <= a && a2 <= b; a2++)
                {
                    var max = a1 > b ? a1 : b;
                    if (max == a)
                    {
                        var value = Lx[a1] + Qy[a2][b][c][d][e + k + 1];
                        if (value < min)
                            min = value;
                    }
                }
            }
            return min;
        }

    }
}