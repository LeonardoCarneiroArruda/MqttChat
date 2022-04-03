# Estudo sobre MQTT
Este projeto foi construído com intuito de servir como objeto de estudo e referência para implementações
de um chat real-time utilizando a arquitetura do MQTT com paradigma publisher/subscriber e tópicos.
Construído após leituras de materiais sobre Message Queuing Telemetry Transport (MQTT). Desenvolvido na 
plataforma .NET e com biblioteca MQTTnet de código aberto (https://github.com/dotnet/MQTTnet), que 
disponibiliza funções para facilitar na construção de um broker e clients
 
# Estrutura
A solução é composto por somente 2 projetos. 
* MqttChat é o nosso servidor/broker. Projeto do tipo Web API 
* MqttClient é o client, que se conecta ao broker, o qual publica mensagens e se inscreve em tópicos. Projeto do tipo Aplicativo de Console.

# Guia inicial

Dentro da pasta MqttChat, execute: 
```
dotnet run
```
Isso iniciará nosso broker, ouvindo as publicações de mensagens na porta local 1883.

Agora vá na pasta MqttClient, execute: 
```
dotnet run
```
Iniciará nosso primeiro client mqtt.
Logo em seguida irá perguntar qual o `Login client`, vamos inserir um nickname, por exemplo: `leonardo`.
Agora vamos digitar para quem desejamos enviar mensagens, importante entender que nesse momento
estamos inserindo o nome que será usado como `Login client` do próximo client mqtt, estamos digitando
o tópico de destino aqui, digite por exemplo: `maria`. 
Pronto, nosso primeiro client está pronto para funcionamento. 

Agora novamente na pasta MqttClient, execute: 
```
dotnet run
```
E siga as mesmas instruções do passo anterior feito no primeiro client, porém aqui na pergunta 
`Login client` digitamos `maria` e para onde enviaremos as mensagens, digitamos `leonardo`, 
que será o tópico de destino deste segundo client.

Com o broker executando e os 2 clients, agora basta trocar mensagens entre si. 