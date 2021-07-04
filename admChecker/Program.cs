using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace admChecker
{
    
    class Program
    {
        static string realPassword; // найденный пароль
        static bool isFound; //создаем булевую переменную  isFound
        static string alphabet; //создаем строковую переменную  alphabet


        public static void CheckPasword(string password) // string password == alphabetForFirstChar[k].ToString()
        {            
            try
            {
                SecureString securePwd = new SecureString();
                foreach (var item in password) //
                {
                    securePwd.AppendChar(item);//AppendChar(item) - добавляем букву  - item в securePwd
                }
                Process.Start("calc.exe", "Lusa", securePwd, ""); // если открывает калькулятор, то код выполняется дальше
                                                                   // если ошибка, то идём в  catch (Win32Exception ex), пропуская realPassword = password;
             
                realPassword = password;               
                isFound = true;
            }
            catch (Win32Exception ex)
            {
                //Console.WriteLine("exception: " + ex);
            }
        }

        //alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        static void FindPass(object alphabetForFirstCharObject) //object alphabetForFirstCharObject == alphabet.Substring(startIndex, alphabet.Length - startIndex) - 
        {
            string alphabetForFirstChar = (string)alphabetForFirstCharObject;
            // password with one letter
            for (int k = 0; k < alphabetForFirstChar.Length; k++)
            {
                CheckPasword(alphabetForFirstChar[k].ToString()); //  "S"
                if (isFound) // если пароль найдет, то выход из метода
                {
                    return;
                }
            }
            // password with two letters
            for (int j = 0; j < alphabetForFirstChar.Length; j++)
            {
                for (int k = 0; k < alphabet.Length; k++)
                {                   
                    CheckPasword(alphabetForFirstChar[j].ToString() + alphabet[k].ToString()); //  '1' -> "1";  "1" + "S" = "1S"
                    if (isFound)
                    {
                        return;
                    }
                }                
            }
            // password with three letters
            for (int i = 0; i < alphabetForFirstChar.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    for (int k = 0; k < alphabet.Length; k++)
                    {
                        CheckPasword(alphabetForFirstChar[i].ToString() + alphabet[j].ToString() + alphabet[k].ToString()); //  "12S"
                        if (isFound)
                        {
                            return;
                        }
                    }
                    // Console.WriteLine("" + alphabetForFirstChar[i] + alphabet[j] + "?");
                }               
            }
        }



        // для однопоточной работы
        static void FindPassOneThread()
        {
            // password with one letter
            for (int k = 0; k < alphabet.Length; k++)
            {
                CheckPasword(alphabet[k].ToString()); //  "1"
                if (isFound)
                {
                    return;
                }
            }
            // password with two letters
            for (int j = 0; j < alphabet.Length; j++)
            {
                for (int k = 0; k < alphabet.Length; k++)
                {
                    CheckPasword(alphabet[j].ToString() + alphabet[k].ToString()); //  "1S"
                    if (isFound)
                    {
                        return;
                    }
                }
            }
            // password with three letters
            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    for (int k = 0; k < alphabet.Length; k++)
                    {
                        CheckPasword(alphabet[i].ToString() + alphabet[j].ToString() + alphabet[k].ToString()); //  "12S"
                        if (isFound)
                        {
                            return;
                        }
                    }
                }
                Console.WriteLine("" + alphabet[i] + "??");
            }
        }


       static void FindByThread(int countTreads)  // countTreads - колличество потоков: для 2 потоков - 15 sec // для 4 потоков - 9 sec// для 8 потоков долго //для 16 потоков долго
        {
            isFound = false;
            realPassword = "";
            //alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Console.WriteLine("alphabet.Length = " + alphabet.Length);// показываем длину алфафита
            //alphabet = s.ToCharArray();//массив символов

            // 0, 1, 2, ... 15      //  i = 0;   startIndex =  i * blockSize =  0 * 16 = 0;    осталось количество символов = alphabet.Length - startIndex = 62 - 0 = 62
            // 16, 16, ... 31       //  i = 1;   startIndex =  i * blockSize =  1 * 16 = 16;   осталось количество символов = alphabet.Length - startIndex = 62 - 16 = 48
            // 32, 31, ... 47,      //  i = 2;   startIndex =  i * blockSize =  2 * 16 = 32;    осталось количество символов = alphabet.Length - startIndex = 62 - 32 = 30
            // else:
            // 48, 46, до конца     //  i = 3;   startIndex =  i * blockSize =  3 * 16 = 48;    осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // FindPassOneThread(); //для тестирования в однопоточном режиме   8 сек для пароля из двух букв         
                        
            List<Thread> threads = new List<Thread>(countTreads);   // создаем список потоков, на countTreads ячеек         
            int blockSize = (int)Math.Ceiling((double)alphabet.Length / countTreads); // длину алфавита делим на колличество потоков (62 символов делим на 4 - ре потока и округляет в большую сторону = 16), для первой буквы пароля
            Console.WriteLine("blockSize = " + blockSize);// blockSize -колличество символов в потоке
            for (int i = 0; i < countTreads; i++) // countTreads - колличество потоков
            {
                Thread thread = new Thread(FindPass); // создаём поток и говорим потоку, что он будет выполнять метод FindPass
                threads.Add(thread); // добавляем поток в список потоков
                int startIndex = i * blockSize; // startIndex -- это номер символа, с которого начинаем копировать строку
                if (alphabet.Length - startIndex < blockSize) // если мы подошли близко к концу строки, осталось количество символов = alphabet.Length - startIndex = 62 - 48
                { //alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                    thread.Start(alphabet.Substring(startIndex, alphabet.Length - startIndex));
                   // Console.WriteLine("symbols for thread №" + i + ": " + alphabet.Substring(startIndex, alphabet.Length - startIndex)); //  осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14
                    break;
                }
                else
                {
                    thread.Start(alphabet.Substring(startIndex, blockSize)); // запускаем поток и передаём параметр методу FindPass  // параметр = alphabet.Substring(startIndex, blockSize)) = "abcdefghijklmno"
                    //Console.WriteLine("symbols for thread №" + i + ": " + alphabet.Substring(startIndex, blockSize));
                }
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }


            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");



            //CheckPasword("12S");
            Console.WriteLine("realPassword FindByThread = " + realPassword);
       }





        static void FindByTask(int countTasks) //колличество задач: для 2 потоков - 15 sec // для 4 потоков - 9 sec// для 8 потоков долго //для 16 потоков долго
        {
            isFound = false;
            realPassword = "";
            //alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Console.WriteLine("alphabet.Length = " + alphabet.Length); 
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
                             

            
            List<Task> tasks = new List<Task>(countTasks);   // создаем список задач, на countTasks ячеек         
            int blockSize = (int)Math.Ceiling((double)alphabet.Length / countTasks); // длину алфавита делим на колличество задач (62 символов делим на 4 - ре потока и округляет в большую сторону = 16), для первой буквы пароля
            Console.WriteLine("blockSize = " + blockSize);
            for (int i = 0; i < countTasks; i++)
            {
               
                int startIndex = i * blockSize; // startIndex -- это номер символа, с которого начинаем копировать строку
                if (alphabet.Length - startIndex < blockSize) // если мы подошли близко к концу строки, осталось количество символов = alphabet.Length - startIndex = 62 - 48
                {

                    //Thread thread = new Thread(FindPass);                   
                    //thread.Start(alphabet.Substring(startIndex, alphabet.Length - startIndex));

                    Task task = new Task(() => FindPass(alphabet.Substring(startIndex, alphabet.Length - startIndex))); // создаём task и говорим task, что он будет выполнять метод FindPass
                    tasks.Add(task); // добавляем поток в список потоков                    
                    task.Start();
                    //Console.WriteLine("symbols for task №" + i + ": " + alphabet.Substring(startIndex, alphabet.Length - startIndex)); //  осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14
                    break;
                }
                else
                {
                    // Task task = new Task(Method);
                    // Action action = () => FindPass(alphabet.Substring(startIndex, blockSize));
                    //action = Method;
                    // Task task = new Task(action); // создаём task, передаём параметр методу FindPass  
                    Task task = new Task(() => FindPass(alphabet.Substring(startIndex, blockSize)));
                    // параметр = alphabet.Substring(startIndex, blockSize)) = "abcdefghijklmno"
                    // и говорим потоку, что он будет выполнять метод FindPass
                    tasks.Add(task); // добавляем task в список task
                    task.Start(); // запускаем task 
                    //Console.WriteLine("symbols for task №" + i + ": " + alphabet.Substring(startIndex, blockSize));
                }
            }
            //for (int i = 0; i < tasks.Count; i++)
            //{
            //    tasks[i].Wait();
            //}
            Task.WaitAll(tasks.ToArray());
            for (int i = 0; i < tasks.Count; i++)
            {
                tasks[i].Dispose();
            }

            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");
            //CheckPasword("12S");
            Console.WriteLine("realPassword in FindByTask = " + realPassword);
           
        }


        //static void Method()
        //{
        //    int startIndex = 0;
        //    int blockSize = 16;
        //    FindPass(alphabet.Substring(startIndex, blockSize));
        //}

        static async Task FindByAsync(int countTasks) //колличество задач: для 2 потоков - 15 sec // для 4 потоков - 9 sec// для 8 потоков долго //для 16 потоков долго
        {
            isFound = false;
            realPassword = "";
            //alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Console.WriteLine("alphabet.Length = " + alphabet.Length);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();



            List<Task> tasks = new List<Task>(countTasks);   // создаем список задач, на countTasks ячеек         
            int blockSize = (int)Math.Ceiling((double)alphabet.Length / countTasks); // длину алфавита делим на колличество задач (62 символов делим на 4 - ре потока и округляет в большую сторону = 16), для первой буквы пароля
            Console.WriteLine("blockSize = " + blockSize);
            for (int i = 0; i < countTasks; i++)
            {

                int startIndex = i * blockSize; // startIndex -- это номер символа, с которого начинаем копировать строку
                if (alphabet.Length - startIndex < blockSize) // если мы подошли близко к концу строки, осталось количество символов = alphabet.Length - startIndex = 62 - 48
                {

                    //Thread thread = new Thread(FindPass);                   
                    //thread.Start(alphabet.Substring(startIndex, alphabet.Length - startIndex));

                    Task task = new Task(() => FindPass(alphabet.Substring(startIndex, alphabet.Length - startIndex))); // создаём task и говорим task, что он будет выполнять метод FindPass
                    tasks.Add(task); // добавляем поток в список потоков                    
                    task.Start();
                    //Console.WriteLine("symbols for task №" + i + ": " + alphabet.Substring(startIndex, alphabet.Length - startIndex)); //  осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14
                    break;
                }
                else
                {
                    // Task task = new Task(Method);
                    // Action action = () => FindPass(alphabet.Substring(startIndex, blockSize));
                    //action = Method;
                    // Task task = new Task(action); // создаём task, передаём параметр методу FindPass  
                    Task task = new Task(() => FindPass(alphabet.Substring(startIndex, blockSize)));
                    // параметр = alphabet.Substring(startIndex, blockSize)) = "abcdefghijklmno"
                    // и говорим потоку, что он будет выполнять метод FindPass
                    tasks.Add(task); // добавляем task в список task
                    task.Start(); // запускаем task 
                   // Console.WriteLine("symbols for task №" + i + ": " + alphabet.Substring(startIndex, blockSize));
                }
            }
           
            // Task.WaitAll(tasks.ToArray()); // old row
            await Task.WhenAll(tasks.ToArray()); // new row
          
            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");
            //CheckPasword("12S");
            Console.WriteLine("realPassword in FindByAsync = " + realPassword);

        }



        static async Task Main(string[] args)
        {
            FindByTask(62); // 9 сек
            Console.WriteLine("========================================");
            Thread.Sleep(2000);
            FindByThread(62); // 9 sec

            Console.WriteLine("========================================");
            await FindByAsync(62);
            Console.WriteLine("end");
            Console.ReadKey();
            
        }

    }
}


