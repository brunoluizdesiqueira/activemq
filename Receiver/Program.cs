using System;
using Amqp;

//using Amqp.Framing;
//using Amqp.Listener;
//using Amqp.Types;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Address address = new Address("amqp://guest:guest@localhost:5672");
            Connection connection = new Connection(address);
            Session session = new Session(connection);
            ReceiverLink receiver = new ReceiverLink(session, "receiver-link", "q1");

            Console.WriteLine("Receiver connected to broker.");
         
            while (true)
            { 
                Message response = receiver.Receive(TimeSpan.FromMilliseconds(2000));       
            
                if (response != null)
                {
                    receiver.Accept(response);
                    Console.WriteLine("Received response: {0} body {1}", response.Properties, response.Body);
                
                    if (string.Equals("done", response.Body))
                    {
                        break;
                    } 
                }                
            }

            receiver.Close();
            session.Close();
            connection.Close();
        }
    }
}