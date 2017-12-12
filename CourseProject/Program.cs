using System;
using System.Collections.Generic;

namespace CourseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var k = 3;
            var t = InitializeExample(k);
            var result = ConnectedKChain.Decide(k, t, 9);
            Console.WriteLine(result);
        }

        public static List<NumeratedConnectedNode> InitializeExample(int k)
        {
            var result = new List<NumeratedConnectedNode>();

            var knotbc = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotbc);

            var knotcd = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotcd);

            var knotef = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotef);

            var knotfg = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotfg);

            var nodeJbc = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = knotbc,
                NodeY = knotcd
            };
            nodeJbc.Node.Type = Node.NodeType.J;
            result.Add(nodeJbc);

            var knotce = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotce);

            var nodeJef = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = knotef,
                NodeY = knotfg
            };
            nodeJef.Node.Type = Node.NodeType.J;
            result.Add(nodeJef);

            var knotfh = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotfh);

            var nodeSbe = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = nodeJbc,
                NodeY = knotce
            };
            nodeSbe.Node.Type = Node.NodeType.S;
            result.Add(nodeSbe);

            var knotbe = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotbe);

            var knoteh = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knoteh);

            var nodeSeh = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = nodeJef,
                NodeY = knotfh
            };
            nodeSeh.Node.Type = Node.NodeType.S;
            result.Add(nodeSeh);

            var knotab = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knotab);

            var nodePbe = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = nodeSbe,
                NodeY = knotbe
            };
            nodePbe.Node.Type = Node.NodeType.P;
            result.Add(nodePbe);

            var nodePeh = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = knoteh,
                NodeY = nodeSeh
            };
            nodePeh.Node.Type = Node.NodeType.P;
            result.Add(nodePeh);

            var knothi = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1,
                    Type = Node.NodeType.Knot
                }
            };
            result.Add(knothi);

            var nodeSae = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = knotab,
                NodeY = nodePbe
            };
            nodeSae.Node.Type = Node.NodeType.S;
            result.Add(nodeSae);

            var nodeJeh = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = nodePeh,
                NodeY = knothi
            };
            nodeJeh.Node.Type = Node.NodeType.J;
            result.Add(nodeJeh);

            var nodeSah = new NumeratedConnectedNode(k)
            {
                Node =
                {
                    WR = 1,
                    WL = 1
                },
                NodeX = nodeSae,
                NodeY = nodeJeh
            };
            nodeSah.Node.Type = Node.NodeType.S;
            result.Add(nodeJeh);

            return result;
        }
    }
}