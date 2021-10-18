using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartData
{
    public class ReadRequestData
    {
        public bool ComStart { get; set; }
        public bool ComAckn { get; set; }
        public bool ComError { get; set; }
        public byte ComErrorCode { get; set; }
        public long PartId { get; set; }
        public bool PartPresent { get; set; }
        public bool Start { get; set; }
        public bool End { get; set; }
        public bool PartOk { get; set; }
        public bool SPC { get; set; }
        public bool Master { get; set; }
        public bool Res6 { get; set; }
        public bool Res7 { get; set; }
        public byte ProductType { get; set; }
        public int NextStation { get; set; }
        public byte NokCode { get; set; }
        public string DMC { get; set; }
        public int WpcNumber { get; set; }
    }
}
