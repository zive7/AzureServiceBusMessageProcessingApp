namespace Event.SenderApp
{
    using Event.SenderApp.Settings;
    using Microsoft.Azure.ServiceBus;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Sender Console!");

            TopicClient _client = new TopicClient(new ServiceBusConnectionStringBuilder(AppSettings.ConnectionString), RetryPolicy.Default);

            while (true)
            {
                string name = Console.ReadLine();

                if (name == "stop")
                {
                    break;
                }

                await SendStringAsync(_client, name);
            }

            Console.ReadKey();
        }

        private static async Task SendStringAsync(TopicClient client, string value)
        {

            List<Message> messages = new List<Message>();

            foreach (char letter in value.ToCharArray())
            {
                var message = new Message()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Body = Encoding.UTF8.GetBytes(letter.ToString()),
                    Label = AppSettings.EventName
                };

                messages.Add(message);
            }

            await client.SendAsync(messages);
        }
    }
}
