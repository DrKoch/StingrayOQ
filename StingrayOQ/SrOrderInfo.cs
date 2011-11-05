using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IB = Krs.Ats.IBNet96;
using OQ = OpenQuant.API;

namespace finantic.OQPlugins
{

    public enum SrOrderInfoState
    {
        New,           // Order is not yet accepted
        Execution,     // Order is executing normally
        ReplaceCancel, // Order is replaced, cancalling
        ReplaceNew     // Order is replaced, new order being sent
    }

    // keep track of OQ Order states, IB order states during normal execution 
    // and during a Replace (Cancel+Send) sequence
    public class SrOrderInfo
    {
        public SrOrderInfoState state;
        public OQ.Order oqorder;
        public double cumQty; // already filled quantity for a replaced order
        public double newQty;       // for new order after a replace/cancel
        public double newPrice;     // for new order after a replace/cancel
        public double newStopPrice; // for new order after a replace/cancel

        #region constructor
        public SrOrderInfo(OQ.Order oqorder, SrOrderInfoState state = SrOrderInfoState.New)
        {
            this.oqorder = oqorder;
            this.state = state;
        }
        #endregion
    }
}
