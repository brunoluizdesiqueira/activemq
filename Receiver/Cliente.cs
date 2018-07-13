namespace Receiver
{
    using System;
     public class Cliente
    {
         public string Crm { get; set; }

         public string Nome { get; set; }

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