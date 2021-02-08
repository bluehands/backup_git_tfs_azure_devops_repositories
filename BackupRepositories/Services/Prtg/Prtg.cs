using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BackupRepositories.Services.Prtg
{
    [JsonObject(Title = "Prtg")]
    [XmlRoot("Prtg")]
    public class Prtg
    {
        public Prtg()
        {
        }

        [XmlElement("Result")]
        [JsonProperty("Result")]
        public List<PrtgResult> Results { get; set; } = new List<PrtgResult>();

        public string Text { get; set; } = "";
        public string Error { get; set; } = "0";
    }
}