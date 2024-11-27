using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Direcionadores.Models
{   
    [DataContract]
    public class AvailableFilters
    {

        [DataMember]
        public List<string> CLIENTES { get; set; }
        
        [DataMember]
        public List<string> SITUACOES { get; set; }
        
        [DataMember]
        public List<string> BAIRROS { get; set; }


        public AvailableFilters()
        {
            CLIENTES = new List<string>();
            SITUACOES = new List<string>();
            BAIRROS = new List<string>();
        }

    }
}