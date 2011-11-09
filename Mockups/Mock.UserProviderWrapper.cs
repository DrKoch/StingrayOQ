using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQuant.API;

namespace Mockups
{
    /// <summary>
    /// This calss allows mocking the UserProvider base class of StingrayOQ
    /// </summary>
    public class UserProviderWrapper
    {
        virtual public void EmitAccepted(Order order) { }
        virtual public void EmitCancelled(Order order) { }
        virtual public void EmitConnected() {}
        virtual public void EmitDisconnected() {}
        virtual public void EmitFilled(Order order, double lastFillPrice, int fillqty) { }
    }
}
