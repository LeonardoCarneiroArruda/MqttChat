using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MqttChat
{
    public class RetainedMessageHandler : IMqttServerStorage
    {
        private IList<MqttApplicationMessage> Database;

        //Implementado a recuperação de mensagens ao subir o servidor
        public Task<IList<MqttApplicationMessage>> LoadRetainedMessagesAsync()
        {
            if (Database == null || !Database.Any())
                Database = new List<MqttApplicationMessage>();

            return Task.FromResult(Database);
        }

        //Aqui pode ser implementado uma inserção da mensagem no database
        public Task SaveRetainedMessagesAsync(IList<MqttApplicationMessage> messages)
        {
            foreach (var message in messages)
                Database.Add(message);

            return Task.FromResult(0);
        }
    }
}
