using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Direcionadores.Models
{

    [DataContract]
    public class CurrentFilter
    {
        [DataMember]
        public string Cliente { get; set; }

        [DataMember]
        public string Situacao { get; set; }

        [DataMember]
        public string Bairro { get; set; }

        [DataMember]
        public string Referencia { get; set; }

        [DataMember]
        public string RuaCruzamento { get; set; }
    }


    [DataContract]
    public class AvailableFilters
    {
        // ERICK: Pensei em usar um dicionário para deixar a possibilidade de filtros mais dinâmica,
        // mas optei pela não implementação pois possivelmente seria uma complexidade desnecessária

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