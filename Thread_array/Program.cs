using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_array
{
    class Program
    {
        static int[] array;
        static int x;
        static long sum0;
        static long sum1;
        static int nRow = 2;
        static int nColumn = 400000000;

        static int[][] matrix;

       


        static void IncrementInt()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Interlocked.Increment(ref x);               
                //x++; // not atomic operation                
            }
        }

        static void IncrementArray()
        {
            for (int i = 0; i < 1000000; i++)
            {
                array[0]++;
                //lock (array)
                //{
                //    array[0]++;
                //}
                // unlock(array)
            }
        }

        static void SumRow0()
        {            
            sum0 = 0;
            int[] row0 = matrix[0];
            for (int j = 0; j < nColumn; j++)
            {
                sum0 += row0[j];
            }           
        }

        static void SumRow1()
        {         
            sum1 = 0;
            int[] row1 = matrix[1]; 
            for (int j = 0; j < nColumn; j++)
            {
                sum1 += row1[j];
            }           
        }



        static void Main(string[] args)
        {
            //x = 0;
            //array = new int[10];
            //Thread thread1 = new Thread(IncrementArray);            
            //Thread thread2 = new Thread(IncrementArray);
            //thread1.Start();
            //thread1.Join();
            ////while (x != 1000000);
            //// while (thread.IsAlive);
            //thread2.Start();
            //thread2.Join();
            //Console.WriteLine(array[0]);

            matrix = new int[nRow][];
            // Create 
            for (int i = 0; i < nRow; i++) // 1615098112
            {
                matrix[i] = new int[nColumn];
                for (int j = 0; j < nColumn; j++)
                {
                    matrix[i][j] = i + j;
                }
            }
            
            // Print();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            //Console.WriteLine(Sum());

            // 160000000000000000
            // Elapsed Time is 3981 ms


            // 2 thread sum = 160000000000000000  
            // Elapsed Time is 8118 ms

            Thread thread3 = new Thread(SumRow0);
            Thread thread4 = new Thread(SumRow1);
            thread3.Start();
            thread4.Start();

            thread3.Join();
            thread4.Join();
            Console.WriteLine(sum0 + sum1);

            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");



            Console.ReadKey();
        }



        static void Print()
        {            
            for (int i = 0; i < nRow; i++)
            {
                for (int j = 0; j < nColumn; j++)
                {
                    Console.Write(matrix[i][j] + "\t");
                }
                Console.WriteLine();
            }
        }

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



    }
}
