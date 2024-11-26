using System;
using System.Runtime.Serialization;

namespace Direcionadores.Models
{
    [DataContract]
    public class Placemark
    {

        [DataMember(Order = 1)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string Description { get; set; }

        [DataMember(Order = 3)]
        public string StyleUrl { get; set; }

        [DataMember(Order = 4)]
        public PlacemarkExtendedData ExtendedData { get; set; }



        [DataContract]
        public class PlacemarkExtendedData
        {
            [DataMember(Order = 5)]
            public string RUA_CRUZAMENTO { get; set; }

            [DataMember(Order = 6)]
            public string REFERENCIA { get; set; }

            [DataMember(Order = 7)]
            public string BAIRRO { get; set; }

            [DataMember(Order = 8)]
            public string SITUACAO { get; set; }

            [DataMember(Order = 9)]
            public string CLIENTE { get; set; }

            [DataMember(Order = 10)]
            public DateTime DATA { get; set; }

            [DataMember(Order = 11)]
            public string COORDENADAS { get; set; }
        }
    }
}