using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OQ = OpenQuant.API;

namespace finantic.OQPlugins
{
    /// <summary>
    /// reorder with submit/Acknowledge information
    /// </summary>
    public class SrOrder
    {
        public OQ.Order oqorder;
        public DateTime PlaceDate = DateTime.MaxValue; // form StingRay to TWS/IB
        public DateTime SubmittedDate = DateTime.MaxValue; // form IB to Exchange
        public bool acknowledged = false; // arrived at exchange

        #region constructor
        public SrOrder(OQ.Order oqorder)
        {
            this.oqorder = oqorder;
        }
        #endregion

        #region public methods
        public override string ToString()
        {
            return oqorder.Side.ToString() + " "  // Buy/Sell 
                + oqorder.Qty.ToString() + " "
                + oqorder.Instrument.Symbol + " "
                + oqorder.Type.ToString() // Market, Limit, ...
                + " (Id=" + oqorder.OrderID + ")";
        }
        #endregion

    }
}
