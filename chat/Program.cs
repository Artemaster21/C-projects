using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace chat
{
    class SimpleChat
    {
        static async Task Main()
        {
            Console.WriteLine("Кем вы хотите быть? (s) - Сервер, (c) - Клиент");
            var choice = Console.ReadKey(true).KeyChar;

            if (choice == 's')
                await RunServer();
            else
                await RunClient();
        }

        static async Task RunServer()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключения...");

            using TcpClient client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Собеседник подключился!");
            await HandleChat(client);
        }

        static async Task RunClient()
        {
            Console.Write("Введите IP сервера (для теста на одном ПК введите 127.0.0.1): ");
            string ip = Console.ReadLine();

            using TcpClient client = new TcpClient();
            await client.ConnectAsync(ip, 8888);
            Console.WriteLine("Подключено к серверу!");
            await HandleChat(client);
        }

        static async Task HandleChat(TcpClient client)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream) { AutoFlush = true };

            // Задача для чтения входящих сообщений в фоновом режиме
            var receiveTask = Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        string message = await reader.ReadLineAsync();
                        if (message == null) break;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"\nСобеседник: {message}");
                        Console.ResetColor();
                    }
                }
                catch { Console.WriteLine("\nСвязь разорвана."); }
            });

            // Цикл отправки сообщений (в основном потоке)
            Console.WriteLine("Вы можете писать сообщения. Нажмите Enter для отправки.");
            while (true)
            {
                string myMessage = Console.ReadLine();
                if (string.IsNullOrEmpty(myMessage)) continue;
                await writer.WriteLineAsync(myMessage);
            }
        }
    }
}
