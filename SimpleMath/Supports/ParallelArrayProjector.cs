using System;
using System.Threading.Tasks;
using SimpleMath.Supports;

namespace SimpleMath
{
    internal static class ParallelArrayProjector
    {
        private static readonly int _maxThreads = Environment.ProcessorCount;

        internal static void For<T>(T[] targetarray, int frompos, int topos,
            Func<int, T> projector, CalculateMode mode)
        {
            switch (mode)
            {
                case CalculateMode.Auto:
                    AutoFor(targetarray, frompos, topos, projector);
                    break;
                case CalculateMode.Parallel:
                    ForAsParallel(targetarray, frompos, topos, projector);
                    break;
                case CalculateMode.Series:
                    ForAsSeries(targetarray, frompos, topos, projector);
                    break;
            }
        }

        internal static void For<T>(T[] targetarray, Func<int, T> projector, CalculateMode mode)
        {
            switch (mode)
            {
                case CalculateMode.Auto:
                    AutoFor(targetarray, projector);
                    break;
                case CalculateMode.Parallel:
                    ForAsParallel(targetarray, projector);
                    break;
                case CalculateMode.Series:
                    ForAsSeries(targetarray, projector);
                    break;
            }
        }

        internal static void For<T>(T[,] targetarray, (int, int) frompos, (int, int) topos,
            Func<int, int, T> projector, CalculateMode mode)
        {
            switch (mode)
            {
                case CalculateMode.Auto:
                    AutoFor(targetarray, frompos, topos, projector);
                    break;
                case CalculateMode.Parallel:
                    ForAsParallel(targetarray, frompos, topos, projector);
                    break;
                case CalculateMode.Series:
                    ForAsSeries(targetarray, frompos, topos, projector);
                    break;
            }
        }

        internal static void For<T>(T[,] targetarray, Func<int, int, T> projector, CalculateMode mode)
        {
            switch (mode)
            {
                case CalculateMode.Auto:
                    AutoFor(targetarray, projector);
                    break;
                case CalculateMode.Parallel:
                    ForAsParallel(targetarray, projector);
                    break;
                case CalculateMode.Series:
                    ForAsSeries(targetarray, projector);
                    break;
            }
        }

        internal static void ForAsSeries<T>(T[] targetarray, int frompos, int topos,
            Func<int, T> projector)
        {
            for (var i = frompos; i < topos; i++)
            {
                targetarray[i] = projector.Invoke(i);
            }
        }

        internal static void ForAsSeries<T>(T[] targetarray, Func<int, T> projector)
        {
            var topos = targetarray.Length;

            ForAsSeries(targetarray, 0, topos, projector);
        }

        internal static void ForAsSeries<T>(T[,] targetarray, (int, int) frompos, (int, int) topos,
            Func<int, int, T> projector)
        {
            var fromrow = frompos.Item1;
            var fromcolumn = frompos.Item2;
            var torow = topos.Item1;
            var tocolumn = topos.Item2;

            for (var i = fromrow; i < torow; i++)
            {
                for (var j = fromcolumn; j < tocolumn; j++)
                {
                    targetarray[i, j] = projector.Invoke(i, j);
                }
            }
        }

        internal static void ForAsSeries<T>(T[,] targetarray, Func<int, int, T> projector)
        {
            var trow = targetarray.GetUpperBound(0) + 1;
            var tcolumn = targetarray.GetUpperBound(1) + 1;

            ForAsSeries(targetarray, (0, 0), (trow, tcolumn), projector);
        }

        internal static void ForAsParallel<T>(T[] targetarray, int frompos, int topos,
            Func<int, T> projector)
        {
            var cpairs = GenCounterPairs(topos - frompos, frompos);

            Parallel.ForEach(cpairs, cpair =>
            {
                for (var i = frompos; i < topos; i++)
                {
                    targetarray[i] = projector.Invoke(i);
                }
            });
        }

        internal static void ForAsParallel<T>(T[] targetarray, Func<int, T> projector)
        {
            var topos = targetarray.Length;

            ForAsParallel(targetarray, 0, topos, projector);
        }

        internal static void ForAsParallel<T>(T[,] targetarray, (int, int) frompos, (int, int) topos,
            Func<int, int, T> projector)
        {
            var fromrow = frompos.Item1;
            var fromcolumn = frompos.Item2;
            var torow = topos.Item1;
            var tocolumn = topos.Item2;

            if (torow - fromrow <= tocolumn - fromcolumn)
            {
                var cpairs = GenCounterPairs(tocolumn - fromcolumn, fromcolumn);

                Parallel.ForEach(cpairs, cpair =>
                {
                    for (var i = fromrow; i < torow; i++)
                    {
                        for (var j = cpair.Item1; j < cpair.Item2; j++)
                        {
                            targetarray[i, j] = projector.Invoke(i, j);
                        }
                    }
                });
            }
            else
            {
                var cpairs = GenCounterPairs(torow - fromrow, fromrow);

                Parallel.ForEach(cpairs, cpair =>
                {
                    for (var j = fromcolumn; j < tocolumn; j++)
                    {
                        for (var i = cpair.Item1; i < cpair.Item2; i++)
                        {
                            targetarray[i, j] = projector.Invoke(i, j);
                        }
                    }
                });
            }
        }

        internal static void ForAsParallel<T>(T[,] targetarray, Func<int, int, T> projector)
        {
            var trow = targetarray.GetUpperBound(0) + 1;
            var tcolumn = targetarray.GetUpperBound(1) + 1;

            ForAsParallel(targetarray, (0, 0), (trow, tcolumn), projector);
        }

        internal static void AutoFor<T>(T[] targetarray, int frompos, int topos,
            Func<int, T> projector, int calccost = 1)
        {
            int totalcost = (topos - frompos) * calccost;

            if (totalcost < 10000)
                ForAsSeries(targetarray, frompos, topos, projector);
                else
            ForAsParallel(targetarray, frompos, topos, projector);
        }

        internal static void AutoFor<T>(T[] targetarray, Func<int, T> projector, int calccost = 1)
        {
            var topos = targetarray.Length;

            AutoFor(targetarray, 0, topos, projector, calccost);
        }

        internal static void AutoFor<T>(T[,] targetarray, (int, int) frompos, (int, int) topos,
            Func<int, int, T> projector, int calccost = 1)
        {
            var fromrow = frompos.Item1;
            var fromcolumn = frompos.Item2;
            var torow = topos.Item1;
            var tocolumn = topos.Item2;

            int totalcost = (torow - fromrow) * (tocolumn - fromcolumn) * calccost;

            if (totalcost < 10000)
                ForAsSeries(targetarray, frompos, topos, projector);
            else
                ForAsParallel(targetarray, frompos, topos, projector);
        }
        
        internal static void AutoFor<T>(T[,] targetarray, Func<int, int, T> projector, int calccost = 1)
        {
            var trow = targetarray.GetUpperBound(0) + 1;
            var tcolumn = targetarray.GetUpperBound(1) + 1;
            
            AutoFor(targetarray, (0, 0), (trow, tcolumn), projector, calccost);
        }

        private static (int, int)[] GenCounterPairs(int totalnum, int startfrom)
        {
            if (totalnum < _maxThreads)
            {
                var pairs = new (int, int)[totalnum];

                for (var i = 0; i < totalnum; i++)
                {
                    pairs[i] = (startfrom + i, startfrom + i + 1);
                }

                return pairs;
            }
            else
            {
                var pairs = new (int, int)[_maxThreads];
                var range = totalnum / _maxThreads;

                for (var i = 0; i < _maxThreads - 1; i++)
                {
                    pairs[i] = (startfrom + i * range, startfrom + (i + 1) * range);
                }
                pairs[^1] = ((_maxThreads - 1) * range, totalnum);

                return pairs;
            }
        }
    }
}
