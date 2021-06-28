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
        static string realPassword = ""; // найденный пароль
        static bool isFound;
        static string alphabet;


        public static void CheckPasword(string password)
        {            
            try
            {
                SecureString securePwd = new SecureString();
                foreach (var item in password)
                {
                    securePwd.AppendChar(item);//AppendChar(item) - добавляем букву  - item в securePwd
                }
                Process.Start("calc.exe", "Lusa2", securePwd, ""); // если открывает калькулятор, то код выполняется дальше
                                                                   // если ошибка, то идём в  catch (Win32Exception ex), пропуская realPassword = password;
             
                realPassword = password;               
                isFound = true;
            }
            catch (Win32Exception ex)
            {
                //Console.WriteLine("exception: " + ex);
            }
        }


        static void FindPass(object alphabetForFirstCharObject)
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



        static void Main(string[] args)
        {
            isFound = false;
            //alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Console.WriteLine("alphabet.Length = " + alphabet.Length);
            //alphabet = s.ToCharArray();//массив символов

            // 0, 1, 2, ... 15      //  i = 0;   startIndex =  i * blockSize =  0 * 16 = 0;    осталось количество символов = alphabet.Length - startIndex = 62 - 0 = 62
            // 16, 16, ... 31       //  i = 1;   startIndex =  i * blockSize =  1 * 16 = 16;   осталось количество символов = alphabet.Length - startIndex = 62 - 16 = 48
            // 32, 31, ... 47,      //  i = 2;   startIndex =  i * blockSize =  2 * 16 = 32;    осталось количество символов = alphabet.Length - startIndex = 62 - 32 = 30
            // else:
            // 48, 46, до конца     //  i = 3;   startIndex =  i * blockSize =  3 * 16 = 48;    осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // FindPassOneThread(); //для тестирования в однопоточном режиме   8 сек для пароля из двух букв         

            int countTreads = 4; //колличество потоков: для 2 потоков - 15 sec // для 4 потоков - 9 sec// для 8 потоков долго //для 16 потоков долго
            List<Thread> threads = new List<Thread>(countTreads);   // создаем список потоков, на countTreads ячеек         
            int blockSize = (int) Math.Ceiling((double) alphabet.Length / countTreads); // длину алфавита делим на колличество потоков (62 символов делим на 4 - ре потока и округляет в большую сторону = 16), для первой буквы пароля
            Console.WriteLine("blockSize = " + blockSize);
            for (int i = 0; i < countTreads; i++)
            {
                Thread thread = new Thread(FindPass); // создаём поток и говорим потоку, что он будет выполнять метод FindPass
                threads.Add(thread); // добавляем поток в список потоков
                int startIndex = i * blockSize; // startIndex -- это номер символа, с которого начинаем копировать строку
                if (alphabet.Length - startIndex < blockSize) // если мы подошли близко к концу строки, осталось количество символов = alphabet.Length - startIndex = 62 - 48
                {             
                    
                    thread.Start(alphabet.Substring(startIndex, alphabet.Length - startIndex)); 
                    Console.WriteLine("symbols for thread №" + i + ": " +   alphabet.Substring(startIndex, alphabet.Length - startIndex)); //  осталось количество символов = alphabet.Length - startIndex = 62 - 48 = 14
                    break;
                }
                else
                {
                    thread.Start(alphabet.Substring(startIndex, blockSize)); // запускаем поток и передаём параметр методу FindPass  // параметр = alphabet.Substring(startIndex, blockSize)) = "abcdefghijklmno"
                    Console.WriteLine("symbols for thread №" + i + ": " + alphabet.Substring(startIndex, blockSize));
                }                 
            }
            for (int i = 0; i < threads.Count; i++)
            {
                threads[i].Join();
            }


            stopwatch.Stop();
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");



            //CheckPasword("12S");
            Console.WriteLine("realPassword=" + realPassword);
            Console.Read();
        }

    }
}









//threads[countTreads - 1] = new Thread(FindPass); //последний поток = 4-1 = 3 //  создаём поток и говорим потоку, что он будет выполнять метод FindPass
//int startIndex = (countTreads - 1) * blockSize; // startIndex -- это номер символа, с которого начинаем копировать строку for last thread //  
//                                                // startIndex =  (countTreads - 1]) * blockSize =  (4-1) * 15 = 45;
//
//Console.WriteLine(alphabet.Substring(startIndex, alphabet.Length - startIndex));