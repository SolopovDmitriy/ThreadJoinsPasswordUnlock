using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadInClass
{
    class Program
    {
        static int nRow = 3;
        static int nColumn = 200000000;
        static int[][] matrix;

        static int[,] matrix2;

        static long Sum()
        {
            long sum = 0;
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nColumn; j++)
                {
                    sum += matrix[i][j];
                }
            }
            return sum;
        }


        static void Main(string[] args)
        {
            matrix = new int[nRow][];
            // Create 
            for (int i = 0; i < nRow; i++) 
            {
                int[] array = matrix[i];
                matrix[i] = new int[nColumn]; // создаём рядок с номером i
                for (int j = 0; j < nColumn; j++)
                {
                    matrix[i][j] = 1;
                }
            }

            //{
            //    { 1,2,3,4,5,6,7,8},
            //    { 5,7,3,4,5,5,6,7},
            //    { 1,2,3,4,5,6,7,8}
            //}


            // Print();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Console.WriteLine(Sum());
            //600000000             Elapsed Time is 3477 ms


            // 2 thread 
            Calculation[] calculations = new Calculation[nRow];
            for (int i = 0; i < nRow; i++) // i = 0, 1, 2
            {
                calculations[i] = new Calculation(matrix[i]);
            }
            for (int i = 0; i < nRow; i++)
            {
                calculations[i].currentThread.Join();
            }


            long sum2 = 0;
            for (int i = 0; i < nRow; i++)
            {
                sum2 += calculations[i].sum;  // calculation0.sum + calculation1.sum + calculation2.sum
            }
            Console.WriteLine(sum2);



            //Calculation calculation0 = new Calculation(matrix[0]);
            //Calculation calculation1 = new Calculation(matrix[1]);
            //Calculation calculation2 = new Calculation(matrix[2]);

            //calculation0.currentThread.Join();
            //calculation1.currentThread.Join();
            //calculation2.currentThread.Join();


            //Console.WriteLine("calculation0.currentThread.IsAlive = " + calculation0.currentThread.IsAlive);
            //Console.WriteLine(calculation0.sum + calculation1.sum + calculation2.sum);
            // 600000000            Elapsed Time is 1352 ms


            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");



            Console.ReadKey();
        }
    }
}
