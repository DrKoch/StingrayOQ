using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQuant.API;

namespace finantic.OQPlugins
{
    public class SrBrokerPosition
    {
        public string Currency;
        public string Exchange;
        public InstrumentType InstrumentType;
        public double LongQty;
        public DateTime Maturity;
        public PutCall PutCall;
        public double Qty;
        public double ShortQty;
        public double Strike;
        public string Symbol;

        public Dictionary<string, string> Fields = new Dictionary<string, string>(); 

        public void AddCustomField(string name, string value)
        {
            if(value == null)
            {
                if (Fields.ContainsKey(name)) Fields.Remove(name);
                return;
            }
            if (Fields.ContainsKey(name)) Fields[name] = value;
            else Fields.Add(name, value);
        }
    }
}
