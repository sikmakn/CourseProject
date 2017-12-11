namespace CourseProject
{
    public class Parameters
    {
        public Parameters(int k)
        {
            L = new double[k - 1];
            R = new double[k - 1];
            Q = InitializeQz(k);
        }

        public double M { get; set; }
        public double[] L { get; }

        public double[] R { get; }
        public double[][][][][] Q { get; }

        private static double[][][][][] InitializeQz(int k)
        {
            var qz = new double[k - 1][][][][];
            for (var b = 0; b < qz.Length; b++)
            {
                qz[b] = new double[k - 1 - b][][][];
                for (var c = 0; c < qz[b].Length; c++)
                {
                    qz[b][c] = new double[k - 1][][];
                    for (var d = 0; d < qz[b][c].Length; d++)
                    {
                        qz[b][c][d] = new double[k - 1 - d][];
                        for (var e = 0; e < qz[b][c][d].Length; e++)
                        {
                            qz[b][c][d][e] = new double[2 * k];
                        }
                    }
                }
            }
            return qz;
        }
    }
}