using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet96;
using OpenQuant.API;
using IB = Krs.Ats.IBNet96;
using OQ = OpenQuant.API;

namespace finantic.OQPlugins
{
    public class Helpers
    {
        #region Converters
        // These converters convert OQ enums to IB enums and back
        // all follow the same pattern
        // if a conversion is not possible they return false.
        // the caller is expected to create an error message and ignore
        // the class containing the enum which is not convertible

        public static bool ActionSideToOrderSide(ActionSide action, out OrderSide side)
        {
            side = OrderSide.Buy;
            switch (action)
            {
                case ActionSide.Buy:
                    side = OrderSide.Buy;
                    break;
                case ActionSide.Sell:
                    side = OrderSide.Sell;
                    break;
                case ActionSide.SShort:
                    side = OrderSide.Sell;
                    break;
                case ActionSide.Undefined:
                default:
                    return false;
            }
            return true;
        }

        public static bool OrderStateToOrderStatus(IB.OrderStatus ibstatus, out OQ.OrderStatus status)
        {
            status = OQ.OrderStatus.New;
            switch (ibstatus)
            {
                case IB.OrderStatus.ApiCancelled:
                case IB.OrderStatus.Canceled:
                    status = OQ.OrderStatus.Cancelled;
                    break;
                case IB.OrderStatus.Inactive:
                case IB.OrderStatus.ApiPending:
                case IB.OrderStatus.PendingSubmit:
                case IB.OrderStatus.PreSubmitted:
                case IB.OrderStatus.Submitted:
                    status = OQ.OrderStatus.PendingNew;
                    break;
                case IB.OrderStatus.Filled:
                    status = OQ.OrderStatus.Filled;
                    break;
                case IB.OrderStatus.PartiallyFilled:
                    status = OQ.OrderStatus.PartiallyFilled;
                    break;
                case IB.OrderStatus.PendingCancel:
                    status = OQ.OrderStatus.PendingCancel;
                    break;
                default:
                case IB.OrderStatus.Unknown:
                    return false;
            }
            return true;
        }

        public static bool OrderTypeToOrderType(IB.OrderType ibordertype, out OQ.OrderType oqordertype)
        {
            oqordertype = OQ.OrderType.Market;
            switch (ibordertype)
            {
                case IB.OrderType.Limit:
                case IB.OrderType.LimitOnClose:
                    oqordertype = OQ.OrderType.Limit;
                    break;
                case IB.OrderType.Market:
                    oqordertype = OQ.OrderType.Market;
                    break;
                case IB.OrderType.MarketOnClose:
                    oqordertype = OQ.OrderType.MarketOnClose;
                    break;
                case IB.OrderType.Stop:
                    oqordertype = OQ.OrderType.Stop;
                    break;
                case IB.OrderType.StopLimit:
                    oqordertype = OQ.OrderType.StopLimit;
                    break;
                case IB.OrderType.TrailingStop:
                    oqordertype = OQ.OrderType.Trail;
                    break;
                case IB.OrderType.TrailingStopLimit:
                    oqordertype = OQ.OrderType.TrailLimit;
                    break;
                default:
                case IB.OrderType.Volatility:
                case IB.OrderType.VolumeWeightedAveragePrice:
                case IB.OrderType.Scale:
                case IB.OrderType.Relative:
                case IB.OrderType.PeggedToMarket:
                case IB.OrderType.Default:
                case IB.OrderType.Empty:
                    return false;
            }
            return true;
        }

        public static bool SecurityTypeToInstrumentType(SecurityType secType, out InstrumentType result)
        {
            result = InstrumentType.Stock; // default
            switch (secType)
            {
                case SecurityType.Bond:
                    result = InstrumentType.Bond;
                    break;
                case SecurityType.Cash:
                    result = InstrumentType.FX;
                    break;
                case SecurityType.Future:
                    result = InstrumentType.Futures;
                    break;
                case SecurityType.FutureOption:
                case SecurityType.Index:
                    result = InstrumentType.Index;
                    break;
                case SecurityType.Option:
                    result = InstrumentType.FutOpt;
                    break;
                case SecurityType.Stock:
                    result = InstrumentType.Stock;
                    break;
                case SecurityType.Bag:
                case SecurityType.Undefined:
                    return false;
            }
            return true;
        }


        public static bool OrderSideToActionSide(OrderSide orderside, out ActionSide action)
        {
            action = ActionSide.Undefined;
            switch (orderside)
            {
                case OrderSide.Buy:
                    action = ActionSide.Buy;
                    break;
                case OrderSide.Sell:
                    action = ActionSide.Sell;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static bool OQOrderTypeToIBOrderType(OQ.OrderType oqtype, out IB.OrderType ibtype)
        {
            ibtype = IB.OrderType.Empty;
            switch (oqtype)
            {
                case OQ.OrderType.Limit:
                    ibtype = IB.OrderType.Limit;
                    break;
                case OQ.OrderType.Market:
                    ibtype = IB.OrderType.Market;
                    break;
                case OQ.OrderType.MarketOnClose:
                    ibtype = IB.OrderType.MarketOnClose;
                    break;
                case OQ.OrderType.Stop:
                    ibtype = IB.OrderType.Stop;
                    break;
                case OQ.OrderType.StopLimit:
                    ibtype = IB.OrderType.StopLimit;
                    break;
                case OQ.OrderType.Trail:
                    ibtype = IB.OrderType.TrailingStop;
                    break;
                case OQ.OrderType.TrailLimit:
                    ibtype = IB.OrderType.TrailingStopLimit;
                    break;
                default:
                    return false;
            }
            return true;
        }

        public static bool OQTimeInForceToIBTimeInForce(OQ.TimeInForce oqtif, out IB.TimeInForce ibtif)
        {
            ibtif = IB.TimeInForce.Undefined;
            switch (oqtif)
            {

                case OQ.TimeInForce.Day:
                    ibtif = IB.TimeInForce.Day;
                    break;
                case OQ.TimeInForce.FOK:
                    ibtif = IB.TimeInForce.FillOrKill;
                    break;
                case OQ.TimeInForce.GTC:
                    ibtif = IB.TimeInForce.GoodTillCancel;
                    break;
                case OQ.TimeInForce.GTD:
                    ibtif = IB.TimeInForce.GoodTillDate;
                    break;
                case OQ.TimeInForce.OPG:
                    ibtif = IB.TimeInForce.MarketOnOpen;
                    break;
                case OQ.TimeInForce.ATC:
                case OQ.TimeInForce.GFS:
                case OQ.TimeInForce.GTX:
                case OQ.TimeInForce.IOC:
                default:
                    return false;
            }
            return true;
        }

        public static bool InstrumentTypeToSecurityType(OQ.InstrumentType instrType, out IB.SecurityType secType)
        {
            secType = SecurityType.Undefined;
            switch (instrType)
            {
                case InstrumentType.Bond:
                    secType = SecurityType.Bond;
                    break;
                case InstrumentType.ETF:
                    secType = SecurityType.Stock;
                    break;
                case InstrumentType.FX:
                    secType = SecurityType.Cash;
                    break;
                case InstrumentType.FutOpt:
                    secType = SecurityType.FutureOption;
                    break;
                case InstrumentType.Futures:
                    secType = SecurityType.Future;
                    break;
                case InstrumentType.Index:
                    secType = SecurityType.Index;
                    break;
                case InstrumentType.Option:
                    secType = SecurityType.Option;
                    break;
                case InstrumentType.Stock:
                    secType = SecurityType.Stock;
                    break;
                case InstrumentType.MultiLeg:
                default:
                    return false;
            }
            return true;
        }

        public static bool OQFAMethodTOIBFAMethod(OQ.IBFaMethod oqFaMethod,
            out IB.FinancialAdvisorAllocationMethod ibFaMethod)
        {
            ibFaMethod = FinancialAdvisorAllocationMethod.None;
            switch (oqFaMethod)
            {
                case IBFaMethod.AvailableEquity:
                    ibFaMethod = FinancialAdvisorAllocationMethod.AvailableEquity;
                    break;
                case IBFaMethod.EqualQuantity:
                    ibFaMethod = FinancialAdvisorAllocationMethod.EqualQuantity;
                    break;
                case IBFaMethod.NetLiq:
                    ibFaMethod = FinancialAdvisorAllocationMethod.NetLiquidity;
                    break;
                case IBFaMethod.PctChange:
                    ibFaMethod = FinancialAdvisorAllocationMethod.PercentChange;
                    break;
                case IBFaMethod.Undefined:
                    ibFaMethod = FinancialAdvisorAllocationMethod.None;
                    break;
                default:
                    return false;
            }
            return true;
        }


        public static bool FAAllocationMethodTOIBFAMethod(IB.FinancialAdvisorAllocationMethod ibFaMethod,
            out OQ.IBFaMethod oqFaMethod)
        {
            oqFaMethod = IBFaMethod.Undefined;
            switch (ibFaMethod)
            {
                case FinancialAdvisorAllocationMethod.AvailableEquity:
                    oqFaMethod = IBFaMethod.AvailableEquity;
                    break;
                case FinancialAdvisorAllocationMethod.EqualQuantity:
                    oqFaMethod = IBFaMethod.EqualQuantity;
                    break;
                case FinancialAdvisorAllocationMethod.NetLiquidity:
                    oqFaMethod = IBFaMethod.NetLiq;
                    break;
                case FinancialAdvisorAllocationMethod.PercentChange:
                    oqFaMethod = IBFaMethod.PctChange;
                    break;
                case FinancialAdvisorAllocationMethod.None:
                default:
                    return false;
            }
            return true;
        }
        #endregion        
    }
}
