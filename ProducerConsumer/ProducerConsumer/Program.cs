using System;
using System.Collections.Generic;
using System.Threading;

namespace ProducerConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Starter(2, 10, 2, 2);

            Console.ReadKey();
        }


        private void Starter(int storageSize, int itemNumbers, int producersCount, int consumersCount)
        {
            Semaphore access = new Semaphore(1, 1);
            Semaphore full = new Semaphore(storageSize, storageSize);
            Semaphore empty = new Semaphore(0, storageSize);

            List<Thread> producerThreads = new List<Thread>();
            List<Thread> consumerThreads = new List<Thread>();

            for (int i = 0; i < producersCount; i++)
            {
                int producerNumber = i + 1;
                Thread threadProducer = new Thread(() => Producer(itemNumbers, full, empty, access, producerNumber));
                producerThreads.Add(threadProducer);
                threadProducer.Start();
            }

            for (int i = 0; i < consumersCount; i++)
            {
                int consumerNumber = i + 1;
                Thread threadConsumer = new Thread(() => Consumer(itemNumbers, full, empty, access, consumerNumber));
                consumerThreads.Add(threadConsumer);
                threadConsumer.Start();
            }

            foreach (var thread in producerThreads)
            {
                thread.Join();
            }

            foreach (var thread in consumerThreads)
            {
                thread.Join();
            }
        }

        private readonly List<string> storage = new List<string>();

        private void Producer(int itemNumbers, Semaphore full, Semaphore empty, Semaphore access, int producerNumber)
        {
            for (int i = 0; i < itemNumbers; i++)
            {
                full.WaitOne();
                access.WaitOne();

                storage.Add("item " + i);
                Console.WriteLine($"Producer {producerNumber} added item " + i);

                access.Release();
                empty.Release();
            }
        }

        private void Consumer(int itemNumbers, Semaphore full, Semaphore empty, Semaphore access, int consumerNumber)
        {
            for (int i = 0; i < itemNumbers; i++)
            {
                empty.WaitOne();
                Thread.Sleep(1000);
                access.WaitOne();

                string item = storage[0];
                storage.RemoveAt(0);

                full.Release();
                access.Release();

                Console.WriteLine($"Consumer {consumerNumber} took " + item);
            }
        }
    }
}
