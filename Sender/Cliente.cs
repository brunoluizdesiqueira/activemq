using System;
using System.Collections.Generic;
using Amqp.Serialization;


namespace Sender
{
     [AmqpContract(Encoding = EncodingType.SimpleMap)]
     public class Cliente
    {
        [AmqpMember]
         public string Crm { get; set; }

        [AmqpMember]
         public string Nome { get; set; }

        [AmqpMember]
         public bool Status { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Crm: {0},\nNome: {1},\nStatus: {2}",
                Crm,
                Nome,
                Status);
        }
    }
}