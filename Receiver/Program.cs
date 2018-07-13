namespace Receiver
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using Amqp;
    using Amqp.Framing;
    using Amqp.Listener;
    using Amqp.Types;
    using Amqp.Serialization;

    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            string address = "amqp://guest:guest@localhost:5672";
            Connection connection = await Connection.Factory.CreateAsync(new Address(address));
            Session session = new Session(connection);
            ReceiverLink receiver = new ReceiverLink(session, "test-receiver", "immobile");

            receiver.Start(5, OnMessageCallback);
            Console.Read();
        }

        // Método que será chamado toda vez que uma mensagem for recebida.
        private static void OnMessageCallback(IReceiverLink receiver, Message message)
        {
            try
            {
                /*
                var serializer = new AmqpSerializer(new ContractResolver()
                {
                    PrefixList = new[] { "Receiver" }
                });

                Cliente cliente = message.GetBody<Cliente>(serializer);
                Console.WriteLine("Received {0}", cliente);
                receiver.Accept(message);
                */
                
                // Lendo a propriedade customizada
                var messageType = message.ApplicationProperties["Message.Type.FullName"];

                // Variável para salvar o corpo da mensagem
                string body = string.Empty;
                // Pega o corpo
                var rawBody = message.Body;

                  // Se o corpo é byte [] assume que foi enviado como uma BrokeredMessage
                  // desserialize-o usando um XmlDictionaryReader
                  if (rawBody is byte[])
                  {
                      using (var reader = XmlDictionaryReader.CreateBinaryReader(
                          new MemoryStream(rawBody as byte[]),
                          null,
                          XmlDictionaryReaderQuotas.Max))
                      {
                          var doc = new XmlDocument();
                          doc.Load(reader);
                          body = doc.InnerText;
                      }
                  }
                  else // Se o corpo for uma string
                  {
                      body = rawBody.ToString();
                  }

                  // Escrevendo o corpo no console
                  Console.WriteLine(body);

                  // Aceitando a mensagem
                  receiver.Accept(message);
              }
              catch (Exception ex)
              {
                  receiver.Reject(message);
                  Console.WriteLine(ex);
              }
        }
    }
}