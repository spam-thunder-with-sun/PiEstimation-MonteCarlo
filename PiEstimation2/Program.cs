using SharpNeatLib.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PiEstimation
{
    class Program
    {
        static long inside = 0;

        static async Task Main(string[] args)
        {
            long toCompute = (long)2 << 33;
            long tot = 0;
            Task[] task = new Task[Environment.ProcessorCount - 1];
            long numXtask = toCompute / task.Length;
            toCompute = numXtask * task.Length;
            string filePath = "pi_interactions.txt";

            var timer = new Stopwatch();
            TimeSpan timeTaken;

            Console.WriteLine("Compute Pi with MonteCarlo 2.0");

            while (true)
            {
                timer.Reset();
                timer.Start();

                for (int i = 0; i < task.Length; i++)
                    task[i] = Task.Factory.StartNew(() => calcPI(new FastRandom(), numXtask));

                Task.WaitAll(task);
                timer.Stop();
                timeTaken = timer.Elapsed;

                tot = toCompute;

                if (File.Exists(filePath))
                {
                    string[] toRead = File.ReadAllLines(filePath);
                    tot += long.Parse(toRead[0]);
                    inside += long.Parse(toRead[1]);
                }

                Console.Write($"Time:{timeTaken.ToString(@"m\:ss\.fff")} ");
                Console.Write($"Interactions:{tot} ");
                Console.Write($"Inside:{inside} ");
                Console.WriteLine($"Pi:{(decimal)inside / (tot >> 2)}");

                string[] toWrite = { tot.ToString(), inside.ToString() };
                File.WriteAllLines(filePath, toWrite);

                tot = 0;
                inside = 0;
            }
        }

        static void calcPI(FastRandom rnd, long totNum)
        {
            double num1, num2;
            long _inside = 0;
            for (long i = 0; i < totNum; i++)
            {
                num1 = rnd.NextDouble();
                num2 = rnd.NextDouble();

                if (num1 * num1 + num2 * num2 < 1)
                    _inside++;
            }

            Interlocked.Add(ref inside, _inside);
        }

    }
}

