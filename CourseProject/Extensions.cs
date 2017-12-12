using System;

namespace CourseProject
{
    public static class Extensions
    {
        public static double Min(this double[][][][][] qz, int k)
        {
            var min = double.PositiveInfinity;
            for (var i = 1; i <= k-1; i++)
            {
                for (var j = i; j <= k-1; j++)
                {
                    for (var a = 1; a <= k-1; a++)
                    {
                        for (var b = i; b <= k-1; b++)
                        {
                            for (var e = 0; e <=k-1; e++)
                            {
                                if (qz[i][j][a][b][e] < min)
                                    min = qz[i][j][a][b][e];
                            }
                        }
                    }
                }
            }
            return min;
        }
    }
}
