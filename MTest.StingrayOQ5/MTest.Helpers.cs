using finantic.OQPlugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Krs.Ats.IBNet96;
using OpenQuant.API;
using IB = Krs.Ats.IBNet96;
using OQ = OpenQuant.API;

namespace MTest.StingrayOQ5
{    
    /// <summary>
    ///This is a test class for HelpersTest and is intended
    ///to contain all HelpersTest Unit Tests
    ///</summary>
    [TestClass()]
    public class HelpersTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion
 

        /// <summary>
        ///A test for ActionSideToOrderSide
        ///</summary>
        [TestMethod()]
        public void ActionSideToOrderSide_BuyTest()
        {
            ActionSide action = ActionSide.Buy;
            OrderSide orderSide;
            bool result = Helpers.ActionSideToOrderSide(action, out orderSide);
            Assert.IsTrue(result);
            Assert.AreEqual(OrderSide.Buy, orderSide);
         }

        [TestMethod()]
        public void ActionSideToOrderSide_SellTest()
        {
            ActionSide action = ActionSide.Sell;
            OrderSide orderSide;
            bool result = Helpers.ActionSideToOrderSide(action, out orderSide);
            Assert.IsTrue(result);
            Assert.AreEqual(OrderSide.Sell, orderSide);
        }

        /// <summary>
        ///A test for OrderSideToActionSide
        ///</summary>
        [TestMethod()]
        public void OrderSideToActionSide_BuyTest()
        {
            OrderSide orderside = OrderSide.Buy;
            ActionSide action;
            bool result = Helpers.OrderSideToActionSide(orderside, out action);
            Assert.IsTrue(result);
            Assert.AreEqual(ActionSide.Buy, action);
        }

        /// <summary>
        ///A test for OrderSideToActionSide
        ///</summary>
        [TestMethod()]
        public void OrderSideToActionSide_SellTest()
        {
            OrderSide orderside = OrderSide.Sell;
            ActionSide action;
            bool result = Helpers.OrderSideToActionSide(orderside, out action);
            Assert.IsTrue(result);
            Assert.AreEqual(ActionSide.Sell, action);
        }

        /// <summary>
        ///A test for FAAllocationMethodTOIBFAMethod
        ///</summary>
        [TestMethod()]
        public void FAAllocationMethodTOIBFAMethodTest()
        {
            FinancialAdvisorAllocationMethod ibFaMethod = FinancialAdvisorAllocationMethod.NetLiquidity;
            IBFaMethod oqFaMethod;
            bool result = Helpers.FAAllocationMethodTOIBFAMethod(ibFaMethod, out oqFaMethod);
            Assert.IsTrue(result);
            Assert.AreEqual(IBFaMethod.NetLiq, oqFaMethod);
         }

        /// <summary>
        ///A test for OQFAMethodTOIBFAMethod
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQFAMethodTOIBFAMethod_None_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.IBFaMethod oqFaMethod = IBFaMethod.Undefined;
            FinancialAdvisorAllocationMethod ibFaMethod;
            bool result = Helpers.OQFAMethodTOIBFAMethod(oqFaMethod, out ibFaMethod);
            Assert.IsTrue(result);
            Assert.AreEqual(IB.FinancialAdvisorAllocationMethod.None, ibFaMethod);
        }


        /// <summary>
        ///A test for InstrumentTypeToSecurityType
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void InstrumentTypeToSecurityType_Stock_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            InstrumentType instrType = InstrumentType.Stock;
            SecurityType secType = new SecurityType();
            bool result = Helpers.InstrumentTypeToSecurityType(instrType, out secType);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(SecurityType.Stock, secType);
        }

        /// <summary>
        ///A test for InstrumentTypeToSecurityType
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void InstrumentTypeToSecurityType_Futures_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            InstrumentType instrType = InstrumentType.Futures;
            SecurityType secType = new SecurityType();
            bool result = Helpers.InstrumentTypeToSecurityType(instrType, out secType);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(SecurityType.Future, secType);
        }

 
        /// <summary>
        ///A test for OQFAMethodTOIBFAMethod
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQFAMethodTOIBFAMethod_NetLiq_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.IBFaMethod oqFaMethod = IBFaMethod.NetLiq;
            FinancialAdvisorAllocationMethod ibFaMethod;
            bool result = Helpers.OQFAMethodTOIBFAMethod(oqFaMethod, out ibFaMethod);
            Assert.IsTrue(result);
            Assert.AreEqual(IB.FinancialAdvisorAllocationMethod.NetLiquidity, ibFaMethod);
        }

        /// <summary>
        ///A test for OQOrderTypeToIBOrderType
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQOrderTypeToIBOrderType_Market_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.OrderType oqtype = OQ.OrderType.Market;
            IB.OrderType ibtype;
            bool result = Helpers.OQOrderTypeToIBOrderType(oqtype, out ibtype);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(IB.OrderType.Market, ibtype, "IB Order Type");
        }

        /// <summary>
        ///A test for OQOrderTypeToIBOrderType
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQOrderTypeToIBOrderType_Limit_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.OrderType oqtype = OQ.OrderType.Limit;
            IB.OrderType ibtype;
            bool result = Helpers.OQOrderTypeToIBOrderType(oqtype, out ibtype);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(IB.OrderType.Limit, ibtype, "IB Order Type");
        }

        /// <summary>
        ///A test for OQTimeInForceToIBTimeInForce
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQTimeInForceToIBTimeInForce_Day_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.TimeInForce oqtif = OQ.TimeInForce.Day;
            IB.TimeInForce ibtif;
            bool result = Helpers.OQTimeInForceToIBTimeInForce(oqtif, out ibtif);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(IB.TimeInForce.Day, ibtif, "IB Time in Force");
        }

        /// <summary>
        ///A test for OQTimeInForceToIBTimeInForce
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void OQTimeInForceToIBTimeInForce_GTC_Test()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            OQ.TimeInForce oqtif = OQ.TimeInForce.GTC;
            IB.TimeInForce ibtif;
            bool result = Helpers.OQTimeInForceToIBTimeInForce(oqtif, out ibtif);
            Assert.IsTrue(result, "returns true");
            Assert.AreEqual(IB.TimeInForce.GoodTillCancel, ibtif, "IB Time in Force");
        }

         /// <summary>
        ///A test for OrderStateToOrderStatus
        ///</summary>
        [TestMethod()]
        public void OrderStateToOrderStatus_NewTest()
        {
            IB.OrderStatus ibstatus = IB.OrderStatus.Submitted;
            OQ.OrderStatus status;            
            bool result = Helpers.OrderStateToOrderStatus(ibstatus, out status);
            Assert.IsTrue(result);
            Assert.AreEqual(OQ.OrderStatus.PendingNew, status);            
        }

        /// <summary>
        ///A test for OrderStateToOrderStatus
        ///</summary>
        [TestMethod()]
        public void OrderStateToOrderStatus_CanceledTest()
        {
            IB.OrderStatus ibstatus = IB.OrderStatus.Canceled;
            OQ.OrderStatus status;
            bool result = Helpers.OrderStateToOrderStatus(ibstatus, out status);
            Assert.IsTrue(result);
            Assert.AreEqual(OQ.OrderStatus.Cancelled, status);
        }

        /// <summary>
        ///A test for OrderTypeToOrderType
        ///</summary>
        [TestMethod()]
        public void OrderTypeToOrderType_MarketTest()
        {
            IB.OrderType ibordertype = IB.OrderType.Market;
            OQ.OrderType oqordertype;
            bool result = Helpers.OrderTypeToOrderType(ibordertype, out oqordertype);
            Assert.IsTrue(result);
            Assert.AreEqual(OQ.OrderType.Market, oqordertype);            
        }

        /// <summary>
        ///A test for OrderTypeToOrderType
        ///</summary>
        [TestMethod()]
        public void OrderTypeToOrderType_LimitTest()
        {
            IB.OrderType ibordertype = IB.OrderType.Limit;
            OQ.OrderType oqordertype;
            bool result = Helpers.OrderTypeToOrderType(ibordertype, out oqordertype);
            Assert.IsTrue(result);
            Assert.AreEqual(OQ.OrderType.Limit, oqordertype);
        }

        /// <summary>
        ///A test for SecurityTypeToInstrumentType
        ///</summary>
        [TestMethod()]
        public void SecurityTypeToInstrumentType_StockTest()
        {
            SecurityType secType = SecurityType.Stock;
            InstrumentType instrType;
            bool expected = false; // TODO: Initialize to an appropriate value
            bool result = Helpers.SecurityTypeToInstrumentType(secType, out instrType);
            Assert.IsTrue(result);
            Assert.AreEqual(InstrumentType.Stock, instrType);
        }
    }
}
