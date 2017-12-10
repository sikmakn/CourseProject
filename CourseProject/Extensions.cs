using System;

namespace CourseProject
{
    public static class Extensions
    {
        public static double Min(this double[][][][][] qz)
        {
            var min = double.PositiveInfinity;
            for (var a = 1; a < qz.Length; a++)
            {
                for (var b = a; b < qz[a].Length; b++)
                {
                    for (var c = 1; c < qz[a][b].Length; c++)
                    {
                        for (var d = c; d < qz[a][b][c].Length; d++)
                        {
                            for (var e = 1; e < qz[a][b][c][d].Length; e++)
                            {
                                if (qz[a][b][c][d][e] < min)
                                    min = qz[a][b][c][d][e];
                            }
                        }
                    }
                }
            }
            return min;
        }
    }
}
