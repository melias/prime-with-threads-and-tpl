using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace prime_with_threads_and_tpl
{
    class Program
    {
        static void Main()
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = PrimesInRange(200, 800000);
            sw.Stop();
            Console.WriteLine($"{result} prime numbers found in {sw.ElapsedMilliseconds / 1000} seconds ({Environment.ProcessorCount} processors).");
        }

        //kind of implemention but less performance way 
        /*
        public static long PrimesInRange(long start, long end)
        {
            long result = 0;
            for (var number = start; number < end; number++)
            {
                if (IsPrime(number))
                {
                    result++;
                }
            }
            return result;
        }
        */

        //legacy code about creating threads
        /*
       public static long PrimesInRange(long start, long end)
       {
           long result = 0;
           var lockObject = new object();

           var range = end - start;
           var numberOfThreads = (long)Environment.ProcessorCount;

           var threads = new Thread[numberOfThreads];
           var chunkSize = range / numberOfThreads;

           for (long i = 0; i < numberOfThreads; i++)
           {
               var chunkStart = start + i * chunkSize;
               var chunkEnd = (i == (numberOfThreads - 1)) ? end : chunkStart + chunkSize;
               threads[i] = new Thread(() =>
               {
                   for (var number = chunkStart; number < chunkEnd; ++number)
                   {
                       if (IsPrime(number))
                       {
                           lock (lockObject)
                           {
                               result++;
                           }
                       }
                   }
               });

               threads[i].Start();
           }

           foreach (var thread in threads)
           {
               thread.Join();
           }

           return result;
       }


        */

        //treadpool way 
        /*
        public static long PrimesInRange(long start, long end)
        {
            long result = 0;
            const long chunkSize = 100;
            var completed = 0;
            var allDone = new ManualResetEvent(initialState: false);

            var chunks = (end - start) / chunkSize;

            for (long i = 0; i < chunks; i++)
            {
                var chunkStart = (start) + i * chunkSize;
                var chunkEnd = i == (chunks - 1) ? end : chunkStart + chunkSize;
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (var number = chunkStart; number < chunkEnd; number++)
                    {
                        if (IsPrime(number))
                        {
                            Interlocked.Increment(ref result);
                        }
                    }

                    if (Interlocked.Increment(ref completed) == chunks)
                    {
                        allDone.Set();
                    }
                });

            }
            allDone.WaitOne();
            return result;
        }
        */

        //tpl way (thread tasks)
        public static long PrimesInRange(long start, long end)
        {
            long result = 0;
            Parallel.For(start, end, number =>
            {
                if (IsPrime(number))
                {
                    Interlocked.Increment(ref result);
                }
            });
            return result;
        }

        static bool IsPrime(long number)
        {
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            for (long divisor = 3; divisor < (number / 2); divisor += 2)
            {
                if (number % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
