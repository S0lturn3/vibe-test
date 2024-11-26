using System.Runtime.Serialization;

namespace Direcionadores.Models
{
    [DataContract]
    public class Filters
    {

        [DataMember]
        public string CLIENTE { get; set; }
        
        [DataMember]
        public string SITUACAO { get; set; }
        
        [DataMember]
        public string BAIRRO { get; set; }
        
        [DataMember]
        public string REFERENCIA { get; set; }
        
        [DataMember]
        public string RUA_CRUZAMENTO { get; set; }

    }
}