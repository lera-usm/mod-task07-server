using System;
using System.Threading;
using System.IO;

namespace server
{
    public class sobit : EventArgs
    {
        public int id { get; set; }
    }
    struct zapros
    {
        public Thread thread;
        public bool isp;
        public int ozh;
        public int vrrab;
    }
    class server
    {
        public int kolzap;
        public int kolvip;
        public int otmzap;
        public int kolo;

        public zapros[] zapr;
        object b;

        public server(int kol, zapros[] zap)
        {
            kolzap = 0;
            kolvip = 0;
            kolzap = 0;
            this.zapr = zap;
            this.kolo = kol;
            b = new object();
            for (int i = 0; i < kolo; i++)
                zap[i].isp = false;
        }
        void otvet(object e)
        {
            int ojidanie = 10;
            Thread.Sleep(ojidanie);
            for (int i = 0; i < kolo; i++)
            {
                if (zapr[i].thread == Thread.CurrentThread)
                {
                    zapr[i].isp = false;
                    zapr[i].thread = null;
                    break;
                }
            }
        }
        public void pr(object otpr, sobit e)
        {
            lock (b)
            {
                kolzap++;
                for (int i = 0; i < kolo; i++)
                {
                    if (!zapr[i].isp)
                        zapr[i].ozh++;
                }
                for (int i = 0; i < kolo; i++)
                {
                    if (!zapr[i].isp)
                    {
                        zapr[i].vrrab++;
                        zapr[i].isp = true;
                        zapr[i].thread = new Thread(new ParameterizedThreadStart(otvet));
                        zapr[i].thread.Start(e.id);
                        kolvip++;
                        return;
                    }
                }
                otmzap++;
            }
        }
    }
    class client
    {
        public event EventHandler<sobit> otv;
        server server;

        int id = 0;

        public client(server server)
        {
            this.server = server;
            this.otv += server.pr;
        }
        protected virtual void vipl(sobit e)
        {
            EventHandler<sobit> sob = otv;
            if (sob != null)
                sob(this, e);
        }
        public void work()
        {
            sobit e = new sobit();
            id++;
            e.id = id;
            this.vipl(e);
        }
    }
    class Program
    {
        static int fakt(int n)
        {
            if (n == 0)
                return 1;
            else
                return n * fakt(n - 1);
        }

        static int kolpot = 7;
        static int kolzap = 60;
        static zapros[] zap = new zapros[kolpot];

        static void Main(string[] args)
        {
            server server = new server(kolpot, zap);
            client client = new client(server);
            for (int i = 0; i < kolzap; i++)
                client.work();
            Thread.Sleep(1500);
            for (int i = 0; i < kolpot; ++i)
            {
                Console.Write("Поток № ");
                Console.Write(i);
                Console.Write(" выполнил заявок ");
                Console.Write(server.zapr[i].vrrab);
                Console.Write("\n");
            }
            double p = server.kolzap / server.kolo;
            Console.WriteLine($"Интенсивность потока заявок: {p}");
            double p0 = 0;
            for (int i = 0; i < server.kolo; i++)
            {
                p0 += Math.Pow(p, i) / fakt(i);
                if (i == server.kolo - 1)
                    p0 = Math.Pow(p0, -1);
            }
            Console.WriteLine($"Вероятность простоя системы: {p0}");
            double pn = Math.Pow(p, server.kolo) * p0 / fakt(server.kolo);
            Console.WriteLine($"Вероятность отказа системы: {pn}");
            double Q = 1 - pn;
            Console.WriteLine($"Относительная пропускная способность: {Q}");
            double A = server.kolzap * (1 - pn);
            Console.WriteLine($"Абсолютная пропускная способность: {A}");
            double k = A / server.kolo;
            Console.WriteLine($"Среднее число занятых каналов: {k}");
            Console.ReadKey();
        }
    }
}