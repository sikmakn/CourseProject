namespace CourseProject
{
    public class NumeratedConnectedNode
    {

        public Node Node { get; set; }
        public ConnectedParameters Parameters { get; set; }

        public NumeratedConnectedNode NodeX { get; set; }
        public NumeratedConnectedNode NodeY { get; set; }

        public NumeratedConnectedNode(int k)
        {
            Node = new Node();
            Parameters = new ConnectedParameters(k);
        }
    }
}
