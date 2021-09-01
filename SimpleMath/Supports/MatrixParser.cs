using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleMath.Supports
{
    internal static class MatrixParser
    {
        internal static Matrix<T> StringToMatrix<T>(string matrixstr,
            ParseFromString parserule, bool isnumeric)
        {
            Matrix<T> matrix = null;

            try
            {
                if (!IsStartAndEndWith(matrixstr, '[', ']')) throw new Exception();

                var rowsstr = RemoveHeadAndTail(matrixstr).Split("\n ");

                Func<string, T> parsefunc = isnumeric ?
                    input => (T)parserule[typeof(T)].Invoke(input) :
                    input => 
                    {
                        if (!IsStartAndEndWith(input, '\"', '\"')) throw new Exception();

                        var content = RemoveHeadAndTail(input);
                        return (T)parserule[typeof(T)].Invoke(Unescape(content));
                    };

                foreach (var rowstr in rowsstr)
                {
                    if (!IsStartAndEndWith(rowstr, '[', ']')) throw new Exception();

                    var strarray = RemoveHeadAndTail(rowstr).Split(", ");
                    var tarray = strarray.Select(item => parsefunc.Invoke(item)).ToArray();

                    if (matrix == null)
                        matrix = new Matrix<T>(tarray);
                    else
                        matrix |= new Matrix<T>(tarray);
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Matrix string is not valid.");
            }

            return matrix;
        }

        private static bool IsStartAndEndWith(string input, char sch, char ech)
            => input[0] == sch && input[^1] == ech;

        private static string RemoveHeadAndTail(string input)
            => input[1..^1];

        internal static string MatrixToString<T>(Matrix<T> matrix, ParseToString parserule)
        {
            Func<T, string> parsefunc = matrix.IsNumeric() ?
                input => parserule[typeof(T)].Invoke(input) :
                input => "\"" + Escape(parserule[typeof(T)].Invoke(input)) + "\"";

            var sb = new StringBuilder();
            sb.Append('[');

            matrix.Iterateii((t, i, j) =>
            {
                var str = parsefunc.Invoke(t);

                if (j == 0)
                {
                    if (j == matrix.ColumnsNum - 1)
                        sb.Append(i == matrix.RowsNum - 1 ? $"[{str}]" : $"[{str}]\n ");
                    else
                        sb.Append($"[{str}, ");
                }
                else if (j == matrix.ColumnsNum - 1)
                {
                    sb.Append(i == matrix.RowsNum - 1 ? $"{str}]" : $"{str}]\n ");
                }
                else
                {
                    sb.Append($"{str}, ");
                }
            });

            sb.Append(']');

            return sb.ToString();
        }
            
        private static bool IsEscapechar(char ch)
            => ch switch
            {
                '\n' => true,
                '\r' => true,
                '\t' => true,
                '\f' => true,
                '\\' => true,
                ',' => true,
                ' ' => true,
                _ => false
            };

        private static string Escape(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (IsEscapechar(input[i]))
                {
                    var sb = new StringBuilder();
                    sb.Append(input, 0, i);

                    do
                    {
                        sb.Append('\\');
                        var ch = input[i];
                        ch = ch switch
                        {
                            '\n' => 'n',
                            '\r' => 'r',
                            '\t' => 't',
                            '\f' => 'f',
                            _ => ch
                        };
                        sb.Append(ch);

                        i++;

                        var lastpos = i;
                        while (i < input.Length)
                        {
                            ch = input[i];
                            if (IsEscapechar(ch)) break;

                            i++;
                        }
                        sb.Append(input, lastpos, i - lastpos);

                    } while (i < input.Length);

                    return sb.ToString();
                }
            }

            return input;
        }

        private static string Unescape(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    var sb = new StringBuilder();
                    sb.Append(input, 0, i);

                    do
                    {
                        i++;

                        if (i < input.Length)
                        {
                            var ch = input[i];
                            ch = ch switch
                            {
                                'n' => '\n',
                                'r' => '\r',
                                't' => '\t',
                                'f' => '\f',
                                _ => ch
                            };
                            sb.Append(ch);

                            i++;
                        }

                        var lastpos = i;
                        while (i < input.Length && input[i] != '\\')
                        {
                            i++;
                        }
                        sb.Append(input, lastpos, i - lastpos);

                    } while (i < input.Length);

                    return sb.ToString();
                }
            }

            return input;
        }
    }
}
