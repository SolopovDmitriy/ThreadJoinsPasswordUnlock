using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsLock
{
    class Program
    {
        static object block = new object();
        public static void TaskOne(object symb)
        {
            for (int i = 0; i < 18000; i++)
            {
                Thread.Sleep(0);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(symb.ToString());
                Console.ResetColor();
            }
            Console.WriteLine("Задача выполнена" + symb.ToString());
        }


        public static void ThreadNotBlocked()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Старт незаблокированного потока");
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(200);
                Console.WriteLine(i);
            }    
            Console.WriteLine("Финиш незаблокированного потока");
        }
        public static void ThreadBlocked()
        {
            
            lock (block)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Старт заблокированного  потока");
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(200);
                    Console.WriteLine(i);
                }
                Console.WriteLine("Финиш заблокированного потока");
            }
        }
        static void Main(string[] args)
        {
            /* Thread thread_1 = new Thread(ThreadNotBlocked);
             Thread thread_2 = new Thread(ThreadNotBlocked);
             thread_1.Start();
             thread_2.Start();*/

            //Thread thread_3 = new Thread(ThreadBlocked);
            //Thread thread_4 = new Thread(ThreadBlocked);
            //thread_3.Start(); //ставится блокировка этим потоком - и пока она не снимется, другой поток ждет
            //thread_4.Start();



            //Thread thread1 = new Thread(new ParameterizedThreadStart(TaskOne));
            Thread thread5 = new Thread(obj => obj.ToString());
            Thread thread1 = new Thread(TaskOne);
            thread1.Priority = ThreadPriority.Lowest;

            Thread thread2 = new Thread(new ParameterizedThreadStart(TaskOne));
            thread2.Priority = ThreadPriority.Normal;

            Thread thread3 = new Thread(new ParameterizedThreadStart(TaskOne));
            thread3.Priority = ThreadPriority.Highest;

            thread1.Start("*");
            thread2.Start("-");
            thread3.Start("#");







        }
    }
}
