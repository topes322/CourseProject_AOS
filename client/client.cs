using System;
using System.Net.Sockets;
using System.Text;
using Extreme.Mathematics;


namespace client
{
    #region try_TCP
    //class client
    //{

    //    private const string host = "127.0.0.1";
    //    private const int port = 8888;
    //    static TcpClient client_socket = new TcpClient();
    //    static NetworkStream stream;

    //    static void Main(string[] args)
    //    {
    //        client_socket.Connect(host, port);
    //        stream = client_socket.GetStream();
    //        Console.WriteLine("Клиент\nПодключен");
    //        try
    //        {
    //            while(true)
    //            {
    //                string data = Console.ReadLine();
    //                var buffer = Encoding.Unicode.GetBytes(data);
    //                stream.Write(buffer, 0, buffer.Length);
    //            }

    //        }
    //        catch(Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //        }
    //        finally
    //        {
    //            client_socket.Close();
    //            stream.Close();
    //        }
    //    }
    //}
    #endregion

    class Program
    {

        private const string host = "127.0.0.1";
        private const int port = 8888;
        static TcpClient client = new TcpClient();
        static NetworkStream stream;

        static void Main()
        {

            client.Connect(host, port);
            stream = client.GetStream();

            try
            {
                while (true)
                {
                    if (stream.DataAvailable)
                    {
                        StringBuilder data = new StringBuilder();
                        do
                        {
                            int len = 0;
                            byte[] buffer = new byte[128];
                            len = stream.Read(buffer, 0, buffer.Length);
                            data.Append(Encoding.Unicode.GetString(buffer, 0, len));
                        }
                        while (stream.DataAvailable);
                        Console.WriteLine("Получено число итераций: {0}", data);

                        string[] words = data.ToString().Split(new char[] { '-' }); // разделяем на слова
                        Console.WriteLine(words[0]);
                        Console.WriteLine(words[1]);
                        int a = Convert.ToInt32(words[0]);
                        int b = Convert.ToInt32(words[1]);

                        String result = Reckoning(a, b).ToString().Replace('.', ',');
                        Console.WriteLine(result);
                        var buf = Encoding.Unicode.GetBytes(result);
                        stream.Write(buf, 0, buf.Length);
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
        static private BigFloat Reckoning(int a, int b)
        {

            BigFloat e = new BigFloat(0.0);

            if (a == 0)
            {
                a = 2;
                e += (BigFloat)2.0;
            }

            for (int i = a; i <= b; i++)
            {
                e += (BigFloat)1 / (BigFloat)Fact(i);
            }

            Console.WriteLine(e.AbsoluteBinaryPrecision);
            Console.WriteLine(e.BinaryPrecision);

            return e;
        }

        static private BigInteger Fact(int x)
        {
            BigInteger f = new BigInteger(1);

            for (int i = 1; i <= x; i++)
            {
                f *= (BigInteger)i;
            }
            return f;
        }
    }
}
