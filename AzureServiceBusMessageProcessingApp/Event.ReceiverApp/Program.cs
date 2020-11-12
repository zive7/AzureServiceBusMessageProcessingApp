namespace Event.ReceiverApp
{
    using Event.ReceiverApp.Settings;
    using Microsoft.Azure.ServiceBus;
    using System;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Receiver Console!");

            Console.ReadKey();

            var _subscriptionClient = new SubscriptionClient(new ServiceBusConnectionStringBuilder(AppSettings.ConnectionString), AppSettings.SubscriptionName);

            ReceiveString(_subscriptionClient);
            await SubscribeAsync(_subscriptionClient, AppSettings.EventName);

            Console.WriteLine("Unsubscribe from the event");
            Console.ReadKey();
            await UnsubscribeAsync(_subscriptionClient, "TestEventZive");
        }

        private static void ReceiveString(SubscriptionClient subscriptionClient)
        {

            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    Console.Write(messageData);

                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                },
               new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 1, AutoComplete = false });
        }

        private static async Task SubscribeAsync(SubscriptionClient subscriptionClient, string eventName)
        {
            await subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter()
                {
                    Label = eventName,
                },
                Name = eventName
            });
        }

        private static async Task UnsubscribeAsync(SubscriptionClient subscriptionClient, string eventName)
        {
            await subscriptionClient.RemoveRuleAsync(eventName);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine("Azure Service Bus failed", exceptionReceivedEventArgs.Exception);

            return Task.CompletedTask;
        }
    }
}
