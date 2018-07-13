namespace Sender
{
    using System;
    using Amqp;
    using Amqp.Framing;
    using Amqp.Listener;
    using Amqp.Types;
    using Amqp.Serialization;
    class Program
    {
        static void Main(string[] args)
        {
           Send();
        }

        public static void Send()
        {
           Address address = new Address("amqp://guest:guest@localhost:5672");
           Connection connection = new Connection(address);
           Session session = new Session(connection);
           SenderLink sender = new SenderLink(session, "sender-link", "immobile");

            var serializer = new AmqpSerializer(new ContractResolver()
            {
                PrefixList = new[] { "Sender" }
            });

           try
           {
                for (int i = 0; i < 1000; i++)
                {
                    var cliente = new Cliente() { Crm = i.ToString(), Nome = "Fulano", Status = false };
                    SendMessage(sender, "Cliente", cliente, serializer);
                    Console.WriteLine("Enviado o cliente: \n{0}", cliente.ToString());
                }
           }
           finally
           {
                sender.Close();
                session.Close();
                connection.Close();
           }
        }

        static void SendMessage(SenderLink sender, string subject, object value, AmqpSerializer serializer)
        {
            var message = new Message() { BodySection = new AmqpValue<object>(value, serializer) };

            // Adicionando algumas propriedades da mensagem
            message.ApplicationProperties = new ApplicationProperties();
            message.ApplicationProperties["Message.Type.FullName"] = typeof(Cliente).FullName;

            // Adicionando o MessageId
            message.Properties = new Properties() { MessageId = Guid.NewGuid().ToString() };
            message.Properties = new Properties() { Subject = subject };

            sender.Send(message);
        }
    }
}
