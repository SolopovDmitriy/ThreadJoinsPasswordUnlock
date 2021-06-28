using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadJoins
{
    delegate int Method(string s);

    class Program
    {
        public static void TaskOne()
        {
            for (int i = 0; i < 500; i++)
            {
                Thread.Sleep(30);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("*");
                Console.ResetColor();
            }
        }
        public static void TaskTwo()
        {
            for (int i = 0; i < 500; i++)
            {
                Thread.Sleep(30);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("-");
                Console.ResetColor();
            }
        }
       
        
        
        static void Main(string[] args)
        {
            Thread thread3 = new Thread(() => Console.WriteLine("hello"));
            ThreadStart ts = () => Console.WriteLine("hello");
            Thread thread2 = new Thread(new ThreadStart(TaskTwo));
            Thread thread = new Thread(TaskTwo);

            thread.Priority = ThreadPriority.Highest;
            thread.Start();
            thread.Join();
            TaskOne();

            //delegate int Method(string s);
            // Method method1 = GetLengthFromString;
            Method method2 = str => str.Length;
            Method method1 = new Method(GetLengthFromString);
            TestDelegate(new Method(GetLengthFromString)); // long long
            TestDelegate(GetLengthFromString); // long
            TestDelegate(str => str.Length); // short



            Console.ReadKey();
        }

        static int GetLengthFromString(string str)
        {
            return str.Length;
        }

        static void TestDelegate(Method method)
        {
            int lenght1 = method("hello");
            Console.WriteLine(lenght1);
        }



    }
}
