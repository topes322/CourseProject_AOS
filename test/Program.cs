using System;
using Extreme.Mathematics;

namespace test
{
    class Program
    {
        static BigInteger fact(int a)
        {
            BigInteger f = new BigInteger(1);

            for(int i = a; i > 1; i--)
            {
                f *= i;
            }
            return f;
        }

        static void Main(string[] args)
        {
            BigFloat e = new BigFloat(2.0);

            for(int i = 2; i < 500; i++)
            {
                e += (BigFloat)1 / (BigFloat)fact(i);
                Console.WriteLine(e.ToString());
                Console.WriteLine(i);
                Console.ReadLine();
                Console.Clear();
            }
        }
    }
}
