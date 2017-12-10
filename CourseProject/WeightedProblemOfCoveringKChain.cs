using System;
using System.Collections.Generic;
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
                            for (var e = 1; e < parameters.Q[a][b][c][d].Length; e++)
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
                s[1] = MinLxQy(nodeX.Parameters.L, nodeY.Parameters.Q, k);
                node.Parameters.L[a] = s.Min();

                s[0] = nodeX.Parameters.R[a] + nodeX.Parameters.M - node.Node.WR;
                s[1] =
                node.Parameters.R[a] = s.Min();
                //
            }
        }

        private static double MinLxQy(double[] Lx, double[][][][][] Qy, int k)
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
                        for (var a = -k; a < k; a++)
                        {
                            for (var e = -k; e < k; e++)
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

        }

        private void DecideJ(NumeratedNode node, int k)
        {

        }

    }
}