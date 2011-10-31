using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQuant.API;

namespace finantic.OQPlugins
{
    public class SrBrokerOrder
    {
        public string Currency;
        public string Exchange;
        public Dictionary<string,string> Fields = new Dictionary<string, string>();
        public InstrumentType InstrumentType;
        public string OrderId;
        public double Price;
        public double Qty;
        public OrderSide Side;
        public OrderStatus Status;
        public double StopPrice;
        public string Symbol;
        public OrderType Type;

        public void AddCustomField(string name, string value)
        {
            if(value == null)
            {
                if (Fields.ContainsKey(name)) Fields.Remove(name);
                return;

            }
            Fields[name] = value;
        }
    }
}
