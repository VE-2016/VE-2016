using System;
using System.Collections;
using System.Text;

namespace WinExplorer
{
    public class Finder
    {
        public int MAX(int x, int y)
        {
            return x > y ? x : y;
        }

        public int MIN(int x, int y)
        {
            return x < y ? x : y;
        }

        //const int XSIZE = 100;

        public ArrayList found { get; set; }

        private void OUTPUT(int s)
        {
            //Console.WriteLine(s);

            found.Add(s);
        }

        /* Computing of the maximal suffix for <= */

        private int maxSuf(char[] x, int m, ref int p)
        {
            int ms, j, k;
            char a, b;

            ms = -1;
            j = 0;
            k = p = 1;
            while (j + k < m)
            {
                a = x[j + k];
                b = x[ms + k];
                if (a < b)
                {
                    j += k;
                    k = 1;
                    p = j - ms;
                }
                else
                    if (a == b)
                    if (k != p)
                        ++k;
                    else
                    {
                        j += p;
                        k = 1;
                    }
                else
                { /* a > b */
                    ms = j;
                    j = ms + 1;
                    k = p = 1;
                }
            }
            return (ms);
        }

        /* Computing of the maximal suffix for >= */

        private int maxSufTilde(char[] x, int m, ref int p)
        {
            int ms, j, k;
            char a, b;

            ms = -1;
            j = 0;
            k = p = 1;
            while (j + k < m)
            {
                a = x[j + k];
                b = x[ms + k];
                if (a > b)
                {
                    j += k;
                    k = 1;
                    p = j - ms;
                }
                else
                    if (a == b)
                    if (k != p)
                        ++k;
                    else
                    {
                        j += p;
                        k = 1;
                    }
                else
                { /* a < b */
                    ms = j;
                    j = ms + 1;
                    k = p = 1;
                }
            }
            return (ms);
        }

        public bool Compare(byte[] b1, byte[] b2)
        {
            return Encoding.ASCII.GetString(b1) == Encoding.ASCII.GetString(b2);
        }

        public byte[] GetBytes(char[] b, int offset, int length)
        {
            byte[] bb = new byte[length];

            for (int i = 0; i < length; i++)
                bb[i] = Convert.ToByte(b[i + offset]);

            return bb;
        }

        /* Two Way string matching algorithm. */

        public ArrayList TW(char[] x, int m, char[] y, int n)
        {
            found = new ArrayList();

            int i, j, ell, memory, per;

            int p = 0;
            int q = 0;

            /* Preprocessing */
            i = maxSuf(x, m, ref p);
            j = maxSufTilde(x, m, ref q);
            if (i > j)
            {
                ell = i;
                per = p;
            }
            else
            {
                ell = j;
                per = q;
            }

            /* Searching */
            //if (memcmp(x, x + per, ell + 1) == 0)
            byte[] b = GetBytes(x, 0, ell + 1);
            byte[] c = GetBytes(x, per, ell + 1);
            if (Compare(b, c) == true)
            {
                j = 0;
                memory = -1;
                while (j <= n - m)
                {
                    i = MAX(ell, memory) + 1;
                    while (i < m && x[i] == y[i + j])
                        ++i;
                    if (i >= m)
                    {
                        i = ell;
                        while (i > memory && x[i] == y[i + j])
                            --i;
                        if (i <= memory)
                            OUTPUT(j);
                        j += per;
                        memory = m - per - 1;
                    }
                    else
                    {
                        j += (i - ell);
                        memory = -1;
                    }
                }
            }
            else
            {
                per = MAX(ell + 1, m - ell - 1) + 1;
                j = 0;
                while (j <= n - m)
                {
                    i = ell + 1;
                    while (i < m && x[i] == y[i + j])
                        ++i;
                    if (i >= m)
                    {
                        i = ell;
                        while (i >= 0 && x[i] == y[i + j])
                            --i;
                        if (i < 0)
                            OUTPUT(j);
                        j += per;
                    }
                    else
                        j += (i - ell);
                }
            }

            return found;
        }

        private int mains()
        {
            return 0;
        }
    }

    public class SearchEngine
    {
    }
}