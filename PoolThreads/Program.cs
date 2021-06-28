using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PoolThreads
{
    class Program
    {
        public static void Work(object state)
        {
            Console.WriteLine($"Поток: {Thread.CurrentThread.ManagedThreadId}; состояние:{state}");
            Thread.Sleep(500);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Работа основного потока.....");
            Random random = new Random();
            for (int i = 0; i < 10; i++)
            {
                ThreadPool.QueueUserWorkItem(Work, random.Next(0, 10));
            }

            Thread.Sleep(2000);
            
            Console.WriteLine("РАбота основного потока завершена");
        }


    }
}
