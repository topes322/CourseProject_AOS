using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Extreme.Mathematics;


namespace server
{
    #region try_TCP
    //class server
    //{

    //    static void Main(string[] args)
    //    {
    //        TcpListener listener = new TcpListener(IPAddress.Any, 8888);
    //        listener.Start();
    //        TcpClient client = listener.AcceptTcpClient();
    //        NetworkStream stream = client.GetStream();
    //        Console.WriteLine("Сервер");


    //        try
    //        {

    //            while (true)
    //            {

    //                StringBuilder str = new StringBuilder();
    //                do
    //                {
    //                    int len = 0;
    //                    byte[] buffer = new byte[256];
    //                    len = stream.Read(buffer, 0, buffer.Length);
    //                    str.Append(Encoding.Unicode.GetString(buffer,0,len));
    //                }
    //                while (stream.DataAvailable);
    //                    Console.WriteLine(str);

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //        finally
    //        {
    //            client.Close();
    //            stream.Close();
    //        }
    //    }
    //}
    #endregion



    class Program
    {
        static ServerObject server; // сервер
        static Thread listenThread; // потока для прослушивания


        static int x = 0; // число итераций
        public static int check = 0;
        public static BigFloat e = new BigFloat(0.0, AccuracyGoal.InheritAbsolute);

        static void Main(string[] args)
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
            }
            catch (Exception ex)
            {
                server.Disconnect();
                Console.WriteLine(ex.Message);
            }
            Thread.Sleep(20);


            Console.WriteLine("Введите число знаков после запятой:");
            try
            {
                x = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            server.CallSend(x);

            while (true)
            {
                if (check == ServerObject.Count_client)
                {
                    Console.WriteLine(e.ToString());


                    x = 0;
                    e = 0.0;
                    check = 0;
                    break;
                }
            }

        }
    }

    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        List<ClientObject> clients = new List<ClientObject>(); // все подключения
        public static int Count_client = 0;
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }

        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine("Новое подключение, кол-во клиентов: {0}", ++Count_client);
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void CallSend(int x)
        {
            if (clients.Count > 0)
            {
                // 1500 / 2 ; 30 000 / 3 ; ...
                // 2500 итерации = 7700 знаков
                // 10 000 итерации = 35000 знаков
                if (x <= 12)
                {
                    x *= 2;
                }
                else if (x >= 550)
                {
                    x /= 2;
                }
                else if (x > 30000)
                {
                    x /= 3;
                }

                x /= clients.Count;

                for (int i = clients.Count - 1; i >= 0; i--)
                {
                    StringBuilder data = new StringBuilder();

                    if (i == 0)
                    {
                        data.Append(0);
                        data.Append("-");
                        data.Append(x * (i + 1));
                    }
                    else
                    {
                        data.Append(x * i + 1);
                        data.Append("-");
                        data.Append(x * (i + 1));
                    }
                    clients[i].SendMes(data.ToString());
                    while (Program.check == 0)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
        }

        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }


            Environment.Exit(0); //завершение процесса
        }
    }


    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        TcpClient client;
        ServerObject server; // объект сервера

        bool res = true;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }


        public void Process()
        {
            try
            {
                Stream = client.GetStream();

                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    bool f = false;
                    StringBuilder data = new StringBuilder();
                    try
                    {
                        do
                        {
                            int len = 0;
                            byte[] buf = new byte[128];
                            len = Stream.Read(buf, 0, buf.Length);
                            data.Append(Encoding.Unicode.GetString(buf, 0, len));
                            f = true;
                        }
                        while (Stream.DataAvailable);

                        if (f)
                            while (true)
                            {
                                if (res)
                                {
                                    res = false;
                                    BigFloat temp;
                                    BigFloat.TryParse(data.ToString(), out temp);


                                    Console.Write("AbsoluteBinaryPrecision temp = ");
                                    Console.WriteLine(temp.AbsoluteBinaryPrecision);

                                    //    преобразовать в строку temp и Program.e
                                    //    убрать 2 первых знака
                                    //    преобразовать в BigInteger 
                                    //    сложить
                                    //    преобразовать резульатат в строку
                                    //    добавить "2," в начало строки
                                    //    преобразовать в BigFloat

                                    Program.e += (BigFloat)temp;
                                   // Program.e = BigFloat.Add(Program.e, temp, AccuracyGoal.DoublePrecision, RoundingMode.TowardsZero);
                                    Console.Write("AbsoluteBinaryPrecision e = ");
                                    Console.WriteLine(Program.e.AbsoluteBinaryPrecision);

                                    Program.check++;
                                    res = true;
                                    break;
                                }
                            }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("Клиент отключился, кол-во клиентов: {0}", --ServerObject.Count_client);
                        Console.Write(ex.Message);
                        break;
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }


        public void SendMes(string data)
        {
            var buffer = Encoding.Unicode.GetBytes(data);
            Stream.Write(buffer, 0, buffer.Length);
            Thread.Sleep(20);
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }

}
