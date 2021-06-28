using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadInClass
{
    class Calculation
    {
        public long sum;        
        int[] array;
        public Thread currentThread;

        public Calculation(int[] array)
        {          
            sum = 0;
            currentThread = new Thread(this.Run);
            this.array = array;
            currentThread.Start(); // начать поток
        }

        // Точка входа в поток.
        void Run()
        { 
            for (int j = 0; j < array.Length; j++)
            {
                sum += array[j];
            }
        }
    }


}
