using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MqttClient
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;
        private static string loginChatClient;
        private static string publisher;
        private static string _topic = "chat/client";

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Client....");
            Console.Write("Login client: ");
            loginChatClient = Console.ReadLine();

            Console.Write("Para quem você deseja enviar mensagens: ");
            publisher = Console.ReadLine();

            try
            {
                // Cria uma nova instância MQTT client.
                var factory = new MqttFactory();
                _client = factory.CreateMqttClient();

                //configure options
                _options = new MqttClientOptionsBuilder()
                    .WithClientId(loginChatClient)
                    .WithTcpServer("localhost", 1883)
                    .WithCredentials("username", "password")
                    /*Injetado WithCleanSession valor false para que não exclusa a sessão em caso de reconexão,
                    consequentemente manterá as mensagens*/
                    .WithCleanSession(false)
                    .Build();

                //handlers
                _client.UseConnectedHandler(e =>
                {
                    Console.WriteLine("Conectado com sucesso com MQTT Broker.");

                    string _topicSubscriber = $"{_topic}/{loginChatClient}";
                    _client.SubscribeAsync(new MqttTopicFilterBuilder()
                                            .WithTopic(_topicSubscriber).Build()
                                           ).Wait();
                });
                _client.UseDisconnectedHandler(e =>
                {
                    Console.WriteLine("Desconectado from MQTT Brokers.");
                });
                _client.UseApplicationMessageReceivedHandler(e =>
                {
                    try
                    {
                        Console.WriteLine();
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        Console.WriteLine($">> {publisher}: {payload}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message, ex);
                    }
                });

                //conectar
                _client.ConnectAsync(_options).Wait();

                SimulatePublish();

                _client.DisconnectAsync().Wait();
                Console.WriteLine("Simulacao finalizada! Press any key to exit.");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro na inicalização do client MQTT");
                Console.WriteLine(e);
                throw;
            }
        }

        //Método que envia mensagens para o tópico
        static void SimulatePublish()
        {
            string _topicPublisher = $"{_topic}/{publisher}";
            string messageToPublish = null;
            Console.WriteLine("Enviar mensagem ou digite quit para parar de publicar mensagens...");

            do
            {
                Console.Write($">> {loginChatClient}: ");
                messageToPublish = Console.ReadLine();

                var messageMqtt = new MqttApplicationMessageBuilder()
                    .WithTopic(_topicPublisher)
                    .WithPayload(messageToPublish)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                if (_client.IsConnected)
                {
                    _client.PublishAsync(messageMqtt);
                }

            } while (messageToPublish != "quit");
        }
    }
}
