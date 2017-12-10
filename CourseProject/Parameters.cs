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
            for (var i = 0; i < qz.Length; i++)
            {
                qz[i] = new double[k - 1 - i][][][];
                for (var i1 = 0; i1 < qz[i].Length; i1++)
                {
                    qz[i][i1] = new double[k - 1][][];
                    for (var i2 = 0; i2 < qz[i][i1].Length; i2++)
                    {
                        qz[i][i1][i2] = new double[k - 1 - i2][];
                        for (var i3 = 0; i3 < qz[i][i1][i2].Length; i3++)
                        {
                            qz[i][i1][i2][i3] = new double[2 * k];
                        }
                    }
                }
            }
            return qz;
        }
    }
}