namespace CourseProject
{
    public class Node
    {
        public enum NodeType { Knot, S, P, J}

        public NodeType Type { get; set; }

        public double WL { get; set; }
        public double WR { get; set; }
    }
}
