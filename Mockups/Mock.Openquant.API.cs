using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenQuant.API
{
    #region enums
    #region InstrumentType
    public enum InstrumentType
    {
        Stock = 0, // Stock  
        Futures = 1, // Futures contract  
        Option = 2, // Option  
        FutOpt = 3, // Future option  
        Bond = 4, // Bond  
        Index = 5, // Index  
        ETF = 6, // Exchange traded fund  
        FX = 7, // Foreign exchange contract  
        MultiLeg = 8, // Multi leg instrument  
    }
    #endregion

    public enum PutCall
    {
        Put,
        Call
    }

    #region TimeInForce
    public enum TimeInForce
    {
        Day = 0, // Day  
        GTC = 1, // Good Till Cancel  
        OPG = 2, // At the Opening  
        IOC = 3, // Immediate or Cancel  
        FOK = 4, // Fill or Kill  
        GTX  = 5, // Good Till Crossing  
        GTD = 6, // Good Till Date  
        ATC = 7, // At the Close  
        GFS 
    }
    #endregion

    public enum IBFaMethod
    {
         PctChange,  
         AvailableEquity,  
         NetLiq,
         EqualQuantity,  
         Undefined 
    }

    public enum OrderSide
    {
        Buy,
        Sell
    }

    #region OrderType
    public enum OrderType
    {
        Market = 0,         // Market order  
        Limit = 1,          // Limit order  
        Stop = 2,           // Stop order  
        StopLimit = 3,      // Stop limit order  
        Trail = 4,          // Trailing stop order
                            // This order type is valid for IB only. Use Order.TrailingAmt to set trailing stop amount. For more information, refer to 'Order Types' section on IB website (http://www.interactivebrokers.com/php/webhelp/webhelp.htm)  
        TrailLimit = 5,     // Trailing stop limit order
                            // This order type is valid for IB only. Use Order.TrailingAmt to set trailing stop amount, Order.StopPrice and Order.Price to set limit offset. For more information, refer to 'Order Types' section on IB website (http://www.interactivebrokers.com/php/webhelp/webhelp.htm)  
        MarketOnClose = 6,  // Market-on-close order
                            // This order type is valid for IB only     
    }
    #endregion

    #region OrderStatus
    public enum OrderStatus
    {
            PendingNew = 0, // Pending New.  
            New = 1, // New.  
            PartiallyFilled = 2, // PartiallyFilled.  
            Filled = 3, // Filled.  
            PendingCancel = 4, // PendingCancel.  
            Cancelled = 5, // Cancelled.  
            Expired = 6, // Expired.  
            PendingReplace = 7, // PendingReplace.  
            Replaced = 8, // Replaced.  
            Rejected = 9, // Rejected.  
    }
    #endregion
    #endregion

    #region IBEx
    public class IBEx
    {
        public double DisplaySize { get; set; }
        public string FaGroup { get; set; }
        public IBFaMethod FaMethod { get; set; }
        public double FaPercentage { get; set; }
        public string FaProfile { get; set; }
        public bool Hidden { get; set; }
    }
    #endregion

    #region Instrument
    public class Instrument
    {
        public string Currency { get; set; }
        public string Exchange { get; set; }
        public double Factor { get; set; }

        public DateTime Maturity { get; set; }


        public double Strike { get; set; }

        public string Symbol;
        private InstrumentType _type;
        public InstrumentType Type
        {
            get { return _type; }
            set { _type = value; } // mock has a setter           
        }
    }
    #endregion

    #region Order
    public class Order
    {
        public string Account { get; set; }
        public string ClientID { get; set; }
        private double _cumQty;

        public double CumQty
        {
            get { return _cumQty; }
            set // mock has setter  
            {
                _cumQty = value;
                if (_cumQty < 0) throw new ArgumentException("must be positive", "CumQty");
                if (_cumQty > Qty) throw new ArgumentException("must be <= Qty", "CumQty");
            }          
        }

        private IBEx _ibex;
        public IBEx IB        
        {
           get { return _ibex; }
            private set { _ibex = value; }
        }
        public Instrument Instrument { get; set; }
        private double _lastQty;
        public double LastQty
        {
            get { return _lastQty; }
            set // mock has setter
            {
                _lastQty = value;
                if (_lastQty <= 0) throw new ArgumentException("must be positive", "LastQty");

            } 
        }

        private double _leavesQty;
        public double LeavesQty
        {
            get { return _leavesQty; }
            set // mock has setter
            {
                _leavesQty = value;
                if (_leavesQty < 0) throw new ArgumentException("must not be negative", "LeavesQty");
            } 
        }

        public string OCAGroup { get; set; }

        public string OrderID;
        public double Price { get; set; }

        public OrderSide Side;
        public double StopPrice { get; set; }
        public TimeInForce TimeInForce { get; set; }
        public string Text { get; set; }

        public OrderType Type;

        private double _qty;
        public double Qty
        {
            get { return _qty; }
            set
            {
                _qty = value;
                _leavesQty = value;
            }
        }

        public Order() // mock has a constructor
        {
            IB = new IBEx();
        }
    }
    #endregion

    #region BrokerAccount
    public class BrokerAccount
    {
        private BrokerAccountFieldList _fields;
        public BrokerAccountFieldList Fields
        {
            get { return _fields; }
            
        }

        public double BuyingPower { get; set; }


        public void AddField(
	                            string name,
	                            string value
                            )
        {
        }

        public void AddField(
	string name,
	string currency,
	string value
)
        {}

        public BrokerOrder AddOrder()
        {
            return new BrokerOrder();
        }
        public BrokerPosition AddPosition()
        {            
            return new BrokerPosition();
        }

    }
    #endregion

    #region BrokerAccountFieldList
    public class BrokerAccountFieldList : ICollection
    {
        public bool Contains(
	string name
)
        {
            return false;
        }

        public bool Contains(
	string name,
	string currency
)
        {
            return false;
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region BrokerAccountList
    public class BrokerAccountList : ICollection
    {

        public BrokerAccount this[string n]
        {
            get { return new BrokerAccount(); }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region BrokerOrder
    public class BrokerOrder
    {
        public string Currency { get; set; }
        public string Exchange { get; set; }

        private BrokerOrderFieldList _fields;
        public BrokerOrderFieldList Fields
        {
            get { return _fields; }           
        }

        public InstrumentType InstrumentType { get; set; }
        public string OrderID { get; set; }
        public double Price { get; set; }
        public double Qty { get; set; }
        public OrderSide Side { get; set; }
        public OrderStatus Status { get; set; }
        public double StopPrice { get; set; }
        public string Symbol { get; set; }
        public OrderType Type { get; set; }

        public void AddCustomField(
	            string name,
	            string value
            )
        {}
    }
    #endregion

    public class BrokerOrderFieldList
    {
        
    }

    #region BrokerPosition
    public class BrokerPosition
    {
        public string Currency { get; set; }
        public string Exchange { get; set; }
        public InstrumentType InstrumentType { get; set; }
        public double LongQty { get; set; }
        public DateTime Maturity { get; set; }
        public PutCall PutCall { get; set; }
        public double Qty { get; set; }
        public double ShortQty { get; set; }
        public double Strike { get; set; }
        public string Symbol { get; set; }

        public void AddCustomField(
	        string name,
	        string value
        )
        {}
    }
    #endregion

    public class BrokerInfo
    {
        private BrokerAccountList _accounts;
        public BrokerAccountList Accounts
        {
            get { return _accounts; }
            
        }

        public BrokerAccount AddAccount(
	string name
)
        {
            return new BrokerAccount();
        }
    }
}
