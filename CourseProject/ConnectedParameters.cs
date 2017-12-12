namespace CourseProject
{
    public class ConnectedParameters
    {
        public double Mc { get; set; }
        public double[] Lc { get; }
        public double[] Rc { get; }

        public double[][][][][] Qc { get; }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int E { get; set; }

        public ConnectedParameters(int k)
        {
            Lc = new double[k];
            Rc = new double[k];

            for (int i = 0; i < k; i++)
            {
                Lc[i] = double.PositiveInfinity;
                Rc[i] = double.PositiveInfinity;
            }

            Qc = new double[k][][][][];
            for (var a = 0; a < Qc.Length; a++)
            {
                Qc[a] = new double[k][][][];
                for (var b = 0; b < Qc[a].Length; b++)
                {
                    Qc[a][b] = new double[k][][];
                    for (var c = 0; c < Qc[a][b].Length; c++)
                    {
                        Qc[a][b][c] = new double[k][];
                        for (var d = 0; d < Qc[a][b][c].Length; d++)
                        {
                            Qc[a][b][c][d] = new double[k];
                            for (var e = 0; e < Qc[a][b][c][d].Length; e++)
                            {
                                Qc[a][b][c][d][e] = double.NaN;
                            }
                        }
                    }
                }
            }
        }

    }
}
