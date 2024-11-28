using System;
using System.Xml.Linq;

namespace Direcionadores.Utils
{

    public class CachedFile
    {
        public XDocument Document { get; set; }

        public FileOrigin Origin { get; set; }
        
        public DateTime LastAccess { get; set; }
    }



    public enum FileOrigin
    {
        Local,
        Upload
    }

}