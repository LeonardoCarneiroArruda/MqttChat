using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            var optionsBuilder = new MqttServerOptionsBuilder()
                     .WithConnectionValidator(c =>
                     {
                         if (c.Username == "username" && c.Password == "password")
                         {
                            Console.WriteLine($"Client {c.ClientId}. Conexão validada para c.Endpoint: {c.Endpoint}");
                            c.ReasonCode = MqttConnectReasonCode.Success;
                         }
                         else
                         {
                             c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                         }
                     })
                    .WithApplicationMessageInterceptor(context =>
                    {
                        Console.WriteLine("=====Mensagem Recebida no Broker======");
                        Console.WriteLine($"ClientId: {context.ClientId} no Tópico: {context.ApplicationMessage.Topic}");
                        string inPayload = Encoding.UTF8.GetString(context.ApplicationMessage.Payload, 0, context.ApplicationMessage.Payload.Length);
                        Console.WriteLine($"Payload: {inPayload}");
                        Console.WriteLine();
                    })
                    .WithSubscriptionInterceptor(context => 
                    {
                        Console.WriteLine($"ClientId: {context.ClientId} inscrito no tópico: {context.TopicFilter.Topic}");
                    })
                    .WithStorage(new RetainedMessageHandler())
                    .WithPersistentSessions();

            services.AddHostedMqttServer(optionsBuilder.Build())
                    .AddMqttConnectionHandler()
                    .AddConnections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapConnectionHandler<MqttConnectionHandler>(
                "/mqtt",
                httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                                                       protocolList =>
                                                           protocolList.FirstOrDefault() ?? string.Empty);

            });

            app.UseMqttServer(server => {});
        }
    }
}
