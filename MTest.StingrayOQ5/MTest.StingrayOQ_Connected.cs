using System.IO;
using Krs.Ats.IBNet96;
using Mockups;
using Moq;
using OpenQuant.API;
using finantic.OQPlugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using IB = Krs.Ats.IBNet96;
using OQ = OpenQuant.API;
using OrderType = Krs.Ats.IBNet96.OrderType;
using TimeInForce = OpenQuant.API.TimeInForce;

namespace MTest.StingrayOQ5
{    
    /// <summary>
    ///This is a test class for StingrayOQTest and is intended
    ///to contain all StingrayOQTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StingrayOQTest_Connected
    {
        private TestContext testContextInstance;

        private Mock<Logger> logger;
        private Mock<IBClient> mockIBClient;
        private Mock<UserProviderWrapper> userProvider;
        private StingrayOQ_Accessor target;

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

        // init mock objects
        // we assume StingrayOQ in connected State
        [TestInitialize()]
        public void MyTestInitialize()
        {
            // Setup Logger
            logger = new Mock<Logger>();            
            Factory.mockLogger = logger.Object;
            // create file directory for log files
            string bp = AppDomain.CurrentDomain.BaseDirectory.ToString();
            Directory.CreateDirectory(bp);
            string bp2 = Path.Combine(bp, "bin");
            Directory.CreateDirectory(bp2);

            // Setup IBClient
            mockIBClient = new Mock<IBClient>();
            Factory.mockIBClient = mockIBClient.Object;

            // make ibclient.IConnected work correctly
            bool isConnected = false;
            mockIBClient.Setup(ib => ib.Connected).Returns(() => isConnected);
            mockIBClient.Setup(ib => ib.Connect("MyTestHost", 6543, 9876)).Callback(() => isConnected = true);
            // other calls
            mockIBClient.Setup(ib => ib.ServerVersion).Returns(42);
            mockIBClient.Setup(ib => ib.TwsConnectionTime).Returns("03:04:05 2011-11-08");
            // Invoke ibclient Events (Callbacks)
            mockIBClient.Setup(ib => ib.RequestIds(1))
                        .Raises(ib => ib.NextValidId += null,
                                   new NextValidIdEventArgs(42));

            mockIBClient.Setup(ib => ib.RequestManagedAccts())
                        .Raises(ib => ib.ManagedAccounts += null,
                                   new ManagedAccountsEventArgs("U111,U222"));

           // mockIBClient.Setup(ib => ib.Disconnect()).Callback(() => isConnected = false); // does not work...
            mockIBClient.Setup(ib => ib.Disconnect()).Raises(ib => ib.ConnectionClosed += null,
                                   new ConnectionClosedEventArgs());

            // Setup UserProvider
            userProvider = new Mock<UserProviderWrapper>();
            Factory.userProviderWrapper = userProvider.Object;

            // Connect StingrayOQ
            target = new StingrayOQ_Accessor();
            target.Hostname = "MyTestHost";
            target.ClientId = 9876; // used for all orders
            target.Port = 6543;

            target.Connect();
        }
        
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        /// Test again if we are successfully connected
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void ConnectTest()
        {
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, "new IBClient"));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, "Connect"));            
            logger.Verify(log => log.AddLog(LoggerLevel.Information, It.IsRegex("Connected, Server Version = 42, Connection Time = 03:04:05 2011-11-08")));

            // EmitConncted
            userProvider.Verify(up => up.EmitConnected());
            // never saw an error?
            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsAny<string>()), Times.Never(), "unexpected error log");
            // State
            Assert.IsTrue(target.IsConnected, "IsConnected");
            Assert.IsTrue(target.TWSConnected, "TWSConnected");
        }

        /// <summary>
        ///A test for Disconnect
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void DisconnectTest()
        {            
            target.Disconnect();
            // Interaction
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* Disconnect.*")));

            mockIBClient.Verify(ib => ib.Disconnect());
            // calls ibclient_ConnectionClosed
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* ibclient_ConnectionClosed.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsRegex("ConnectionClosed.*"))); 

            userProvider.Verify(up => up.EmitDisconnected());

            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* ibclient_ConnectionClosed.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* Disconnect.*"))); 

            // State
            Assert.IsFalse(target.IsConnected, "IsConnected");
            Assert.IsFalse(target.TWSConnected, "TWSConnected");
        }

        /// <summary>
        ///A test for DoReplaceSend
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void DoReplaceSendTest()
        {
            //StingrayOQ_Accessor target = new StingrayOQ_Accessor(); // TODO: Initialize to an appropriate value
            //string orderId = string.Empty; // TODO: Initialize to an appropriate value
            //target.DoReplaceSend(orderId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetBrokerInfo
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void GetBrokerInfoTest()
        {
            //StingrayOQ_Accessor target = new StingrayOQ_Accessor(); // TODO: Initialize to an appropriate value
            //BrokerInfo expected = null; // TODO: Initialize to an appropriate value
            //BrokerInfo actual;
            //actual = target.GetBrokerInfo();
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Replace
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void ReplaceTest()
        {
            //StingrayOQ_Accessor target = new StingrayOQ_Accessor(); // TODO: Initialize to an appropriate value
            //OQ.Order order = null; // TODO: Initialize to an appropriate value
            //double newQty = 0F; // TODO: Initialize to an appropriate value
            //double newPrice = 0F; // TODO: Initialize to an appropriate value
            //double newStopPrice = 0F; // TODO: Initialize to an appropriate value
            //target.Replace(order, newQty, newPrice, newStopPrice);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }


        private void AcceptAndFillOrder(int orderId, IB.Contract contract, IB.Order order)
        {
            int filled = 0;
            int remaining = order.TotalQuantity;
            decimal fillPrice = (decimal) order.LimitPrice;
            // Accept
            mockIBClient.Raise(ib => ib.OrderStatus += null,
                new OrderStatusEventArgs(orderId, IB.OrderStatus.Submitted, filled, remaining,
                                         (decimal)order.LimitPrice, 333333, 0, (decimal)0.0, order.ClientId, ""));
            int fillqty = order.TotalQuantity;
            filled += fillqty;
            remaining -= fillqty;
            // Fill
            mockIBClient.Raise(ib => ib.OrderStatus += null,
                new OrderStatusEventArgs(orderId, IB.OrderStatus.Filled, filled, remaining,
                             fillPrice, 333333, 0, fillPrice, order.ClientId, ""));
        }

        /// <summary>
        ///A test for Send
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void SendTest()
        {      
            // Arrange
            OQ.Order order = CreateOQOrder(456);
            
            // Accept and Fill Order
            mockIBClient.Setup(ib => ib.PlaceOrder(It.IsAny<int>(), It.IsAny<IB.Contract>(), It.IsAny<IB.Order>()))
                .Callback((int id, IB.Contract ctr, IB.Order ord) => AcceptAndFillOrder(id, ctr, ord));
                                       
            target.Send(order); // Act

            // info PlaceOrder
            string expected_text = @"PlaceOrder.ID=42, Orderref=, Symbol=OQOQ, Action=Buy, Size=456, OrderType=Limit, limit price=12.34, Aux price=45.57.";
            logger.Verify(log => log.AddLog(LoggerLevel.Information, It.IsRegex(expected_text)));

            // expected contract
            Contract exp_contract = new Contract();
            exp_contract.Currency = "CHF";
            exp_contract.Exchange = "Brasil";
            exp_contract.SecurityType = SecurityType.Stock;
            exp_contract.Symbol = "OQOQ";

            // expected order
            IB.Order exp_iborder = new IB.Order();
            exp_iborder.Account = "U111";
            exp_iborder.ClientId = 9876; // must come from Settings
            exp_iborder.OcaGroup = "MyOCA";
            exp_iborder.OrderId = 42;
            exp_iborder.TotalQuantity = 456;
            exp_iborder.Action = ActionSide.Buy;
            exp_iborder.AuxPrice = (decimal)45.57;
            exp_iborder.LimitPrice = (decimal)12.34;
            exp_iborder.Tif = Krs.Ats.IBNet96.TimeInForce.GoodTillCancel;
            exp_iborder.OrderType = OrderType.Limit;

            mockIBClient.Verify(ib => ib.PlaceOrder(42,
                It.Is<IB.Contract>(ctr => MatchContracts(exp_contract, ctr)),
                It.Is<IB.Order>(ord => MatchOrders(exp_iborder, ord))));
            
            // check Info returned in  OQ.Order
            Assert.AreEqual("42", order.OrderID);           
            Assert.AreEqual("9876", order.ClientID);   // from Settings / StingrayOQ.ClientId Property  
       
            // ibclient_OrderStatus
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* ibclient_OrderStatus.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Information,
                           "ibclient_OrderStatus: received from TWS  ID: 42, ClientID: 9876, Filled: 0, Remaining: 456, Status: Submitted"));

            // OnOrderAccepted
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* OnOrderAccepted")));
            userProvider.Verify(up => up.EmitAccepted(It.Is<OQ.Order>(ord => ord.OrderID == "42")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* OnOrderAccepted")));

            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* ibclient_OrderStatus.*")));

            // OnOrderFilled
            logger.Verify(log => log.AddLog(LoggerLevel.Information,
                "ibclient_OrderStatus: received from TWS  ID: 42, ClientID: 9876, Filled: 456, Remaining: 0, Status: Filled"));

            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* OnOrderFilled")));
            userProvider.Verify(up => up.EmitFilled(It.Is<OQ.Order>(ord => ord.OrderID == "42"), 12.34, 456));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* OnOrderFilled")));

            // never saw an error?
            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsAny<string>()), Times.Never(),
                          "unexpected error log");
        }

        private void AcceptOrder(int orderId, IB.Contract contract, IB.Order order)
        {
            int filled = 0;
            int remaining = order.TotalQuantity;
            // Accept
            mockIBClient.Raise(ib => ib.OrderStatus += null,
                new OrderStatusEventArgs(orderId, IB.OrderStatus.Submitted, filled, remaining,
                                         (decimal)order.LimitPrice, 333333, 0, (decimal)0.0, order.ClientId, ""));
        }

        private void cancelOrder(int orderId, int clientId)
        {            
            // Cancel
            mockIBClient.Raise(ib => ib.OrderStatus += null,
                new OrderStatusEventArgs(orderId, IB.OrderStatus.Canceled, 0, 0,
                                         (decimal)0.0, 333333, 0, (decimal)0.0, clientId, ""));
        }


        /// <summary>
        ///A test for Cancel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void CancelTest()
        {
            // Arrange
            OQ.Order order = CreateOQOrder(456);
            int clientID = target.ClientId;

            // Accept Order
            mockIBClient.Setup(ib => ib.PlaceOrder(It.IsAny<int>(), It.IsAny<IB.Contract>(), It.IsAny<IB.Order>()))
                .Callback((int id, IB.Contract ctr, IB.Order ord) => AcceptOrder(id, ctr, ord));

            // Cancel Order
            mockIBClient.Setup(ib => ib.CancelOrder(It.IsAny<int>()))
                .Callback((int id) => cancelOrder(id, clientID));

            target.Send(order); // Act
            target.Cancel(order); 
            
            // Assert
            // Order Accepted
            logger.Verify(log => log.AddLog(LoggerLevel.Information,
               "ibclient_OrderStatus: received from TWS  ID: 42, ClientID: 9876, Filled: 0, Remaining: 456, Status: Submitted"));
            userProvider.Verify(up => up.EmitAccepted(It.Is<OQ.Order>(ord => ord.OrderID == "42")));

            // Cancel
            // ibClient.Cancel
            mockIBClient.Verify(ib => ib.CancelOrder(42));
            // OnOrderCanceled
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* OnOrderCanceled")));
            userProvider.Verify(up => up.EmitCancelled(It.Is<OQ.Order>(ord => ord.OrderID == "42")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* OnOrderCanceled")));

            // never saw an error?
            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsAny<string>()), Times.Never(),
                          "unexpected error log");                        
        }


        /// <summary>
        ///A test for Accounts
        ///</summary>
        [TestMethod()]
        public void AccountsTest()
        {
            //StingrayOQ target = new StingrayOQ(); // TODO: Initialize to an appropriate value
            //AccountSettings expected = null; // TODO: Initialize to an appropriate value
            //AccountSettings actual;
            //target.Accounts = expected;
            //actual = target.Accounts;
            //Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        #region Test Helpers

        #region Create Test Data
        private OQ.Order CreateOQOrder(int quantity)
        {
            OQ.Order order = new OQ.Order();
            order.Account = "U111";
            order.ClientID = "9999"; // will be overidden with value from Settings

            order.Instrument = new Instrument();
            order.Instrument.Currency = "CHF";
            order.Instrument.Exchange = "Brasil";
            order.Instrument.Symbol = "OQOQ";
            order.Instrument.Type = InstrumentType.Stock;
            order.OCAGroup = "MyOCA";
            order.Price = 12.34;
            order.OrderID = "7777"; // will be overridden from Broker
            order.Qty = quantity;
            order.Side = OrderSide.Buy;
            order.StopPrice = 45.57;
            order.Text = "My Order Text";
            order.TimeInForce = TimeInForce.GTC;
            order.Type = OpenQuant.API.OrderType.Limit;

            return order;
        }

        #endregion

        #region Matches
        public bool MatchContracts(Contract expected, Contract actual)
        {
            Assert.AreEqual(expected.Currency, actual.Currency, "Contract.Currency");
            Assert.AreEqual(expected.Exchange, actual.Exchange, "Contract.Exchange");
            Assert.AreEqual(expected.SecurityType, actual.SecurityType, "Contract.SecurityType");
            Assert.AreEqual(expected.Symbol, actual.Symbol, "Contract.Symbol");
            return true;
        }

        public bool MatchOrders(IB.Order expected, IB.Order actual)
        {
            Assert.AreEqual(expected.Account, actual.Account, "Order.Account");
            Assert.AreEqual(expected.Action, actual.Action, "Order.Action");
            Assert.AreEqual(expected.ClientId, actual.ClientId, "Order.ClientId");
            Assert.AreEqual(expected.OcaGroup, actual.OcaGroup, "Order.OcaGroup");
            Assert.AreEqual(expected.OrderId, actual.OrderId, "Order.OrderId");
            Assert.AreEqual(expected.TotalQuantity, actual.TotalQuantity, "Order.TotalQuantity");
            Assert.AreEqual((double)expected.LimitPrice, (double)actual.LimitPrice, 0.05, "Order.LimitPrice");
            Assert.AreEqual((double)expected.AuxPrice, (double)actual.AuxPrice, 0.005, "Order.AuxPrice");
            Assert.AreEqual(expected.Tif, actual.Tif, "Order.Tif");
            Assert.AreEqual(expected.OrderType, actual.OrderType, "Order.OrderType");

            return true;
        }
        #endregion
        #endregion
    }
}
