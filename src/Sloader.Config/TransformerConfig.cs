using System.Collections.Generic;

namespace Sloader.Config
{
    public class TransformerConfig
    {
        public string Name { get; set; }
        public IList<string> Sources { get; set; } 
    }
}