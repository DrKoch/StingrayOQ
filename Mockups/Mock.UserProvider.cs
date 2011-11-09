using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mockups;
using finantic.OQPlugins;

namespace OpenQuant.API.Plugins
{
   

    public class UserProvider
    {
        public static UserProviderWrapper wrapper;

        public string name;
        public string description;
        public int id;
        public string url;

        #region constructor

        public UserProvider()
        {
            wrapper = Factory.GetUserProviderWrapper();
        }

        #endregion

        #region Properties

        protected virtual bool IsConnected { get { return false; } }
        #endregion

        #region public methods
        protected virtual void Connect()
        {
        }

        protected virtual void Disconnect()
        {
            
        }

        protected virtual void Send(Order ord)
        {

        }

        protected virtual void Cancel(Order ord)
        {

        }

        protected virtual void Replace(Order ord, double a, double b, double c)
        {

        }

        protected virtual BrokerInfo GetBrokerInfo()
        {

            BrokerInfo bi = new BrokerInfo();
            return bi;
        }

        #region Emit*
        protected void EmitAccepted(Order order)
        {
            wrapper.EmitAccepted(order);
        }

        protected void EmitCancelled(Order order)
        {
            wrapper.EmitCancelled(order);
        }
        protected void EmitCancelReject(
	Order order,
	OrderStatus status,
	string message
)
        {
        }

        protected void EmitConnected()
        {
            wrapper.EmitConnected();
        }

        protected void EmitDisconnected()
        {
            wrapper.EmitDisconnected();
        }


        protected void EmitError( string message )
        {
        }

        protected void EmitError(
	int id,
	int code,
	string message
)
        {}

        protected void EmitFilled(Order order, double lastFillPrice, int fillqty)
        {
            wrapper.EmitFilled(order, lastFillPrice, fillqty);
            order.LastQty = fillqty;
            order.CumQty += fillqty;            
            order.LeavesQty = order.Qty - order.CumQty;
        }

        protected void EmitPendingCancel(Order order)
        {}


        protected void EmitPendingReplace(
	                                            Order order
                                            )
        {
        }

        protected void EmitRejected(Order order, string message)
        {}

        protected void EmitReplaced(
	            Order order
            )
        {
        }

        protected void EmitReplaceReject(
	                                        Order order,
	                                        OrderStatus status,
	                                        string message
                                        )
        {
        }
        #endregion

        #endregion

    }
}
