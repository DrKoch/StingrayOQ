using System.IO;
using Krs.Ats.IBNet96;
using Mockups;
using Moq;
using OpenQuant.API;
using finantic.OQPlugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OQ = OpenQuant.API;

namespace MTest.StingrayOQ5
{
    /// <summary>
    ///This is a test class for StingrayOQTest and is intended
    ///to contain all StingrayOQTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StingrayOQTest
    {
        private TestContext testContextInstance;
        private Mock<Logger> logger;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            logger = new Mock<Logger>();
            Factory.mockLogger = logger.Object;
            // create file directory for log files
            string bp = AppDomain.CurrentDomain.BaseDirectory.ToString();
            Directory.CreateDirectory(bp);
            string bp2 = Path.Combine(bp, "bin");
            Directory.CreateDirectory(bp2);
        }

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion


        /// <summary>
        ///A test for StingrayOQ Constructor
        ///</summary>
        [TestMethod()]
        public void StingrayOQConstructorTest()
        {
            // StingrayOQ target = new StingrayOQ();
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();

            // base Properties
            Assert.AreEqual("finantic's Execution Provider for Interactive Brokers", target.description, "Description");
            Assert.AreEqual(129, target.id, "Id");
            Assert.AreEqual("StingrayOQ", target.name, "Name");
            Assert.AreEqual("http://www.finantic.de", target.url, "Url");
            // Properties
            Assert.AreEqual("1.42.3.5", target.StingrayOQVersion, "StingrayOQVersion");
            Assert.AreEqual(4262, target.ClientId, "ClientId");
            Assert.AreEqual("127.0.0.1", target.Hostname, "HostName");
            Assert.AreEqual(LogDestination.File, target.LogDestination, "LogDestination");
            Assert.AreEqual(LoggerLevel.Error, target.LoggerLevel, "LoggerLevel");
            Assert.AreEqual(false, target.OutsideRTH, "OutsideRTH");
            Assert.AreEqual(7496, target.Port, "Port"); // TWS default port
            Assert.AreEqual(false, target.TWSConnected, "TWSConnected");
            Assert.AreEqual(true, target.AutoTransmit, "AutoTransmit");
            // FA
            Assert.AreEqual(FinancialAdvisorAllocationMethod.None, target.FAMethod, "FAMethod");
            Assert.AreEqual(null, target.FAGroup, "FAGroup");
            Assert.AreEqual(null, target.FAPercentage, "FAPercentage");
            Assert.AreEqual(null, target.FAProfile, "FAProfile");

            //Version must match AssemblyVersion in Mockups->AssemblyInfo.cs
            Assert.AreEqual("1.42.3.5", target.Version(), "Version");

            // Accounts
            Assert.IsNotNull(target.Accounts, "Accounts not null");

            // Broker Info
            BrokerInfo brokerInfo = target.GetBrokerInfo();

            Assert.IsNotNull(brokerInfo, "brokerInfo not null");
        }

        /// <summary>
        ///A test for StingrayOQ ibclient_NextValidId
        ///</summary>
        [TestMethod()]
        public void StingrayOQnextValidIDLoggerTest()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            target.InitLogger();

            target.ibclient_NextValidId(null, new NextValidIdEventArgs(123));

            logger.Verify(
                log =>
                log.AddLog(LoggerLevel.Detail,
                           It.IsRegex("START.* ibclient_NextValidId.*: ID= 123 waiting for private_lock")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* ibclient_NextValidId()")));
        }

        /// <summary>
        /// A test for Connect()
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void ConnectTest()
        {
            Mock<IBClient> mockIBClient = new Mock<IBClient>();
            Factory.mockIBClient = mockIBClient.Object;

            Mock<UserProviderWrapper> userProvider = new Mock<UserProviderWrapper>();
            Factory.userProviderWrapper = userProvider.Object;

            // make ibclient.IConnected work correctly
            bool isConnected = false;
            mockIBClient.Setup(ib => ib.Connected).Returns(() => isConnected);
            mockIBClient.Setup(ib => ib.Connect("MyTestHost", 6543, 9876)).Callback(() => isConnected = true);
            // other calls
            mockIBClient.Setup(ib => ib.ServerVersion).Returns(42);
            mockIBClient.Setup(ib => ib.TwsConnectionTime).Returns("03:04:05 2011-11-08");
            // Invoke ibclient Events (Callbacks)
            mockIBClient.Setup(ib => ib.RequestManagedAccts())
                .Raises(ib => ib.ManagedAccounts += null,
                        new ManagedAccountsEventArgs("U111,U222"));

            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            target.Hostname = "MyTestHost";
            target.ClientId = 9876;
            target.Port = 6543;

            target.Connect(); // Act

            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* Initialize.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Information, It.IsRegex("=== StingrayOQ V .* started ===")));
            // CheckTWS
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* CheckTWS.*")));
            // openConnection
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("START.* openConnection.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, "new IBClient"));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, "Connect"));

            mockIBClient.Verify(ib => ib.Connect("MyTestHost", 6543, 9876));

            mockIBClient.Verify(ib => ib.RequestManagedAccts());
            logger.Verify(log => log.AddLog(LoggerLevel.Information, It.IsRegex("2 Accounts found")));

            // RequestAccount Updates
            mockIBClient.Verify(ib => ib.RequestAccountUpdates(true, "U111"));
            mockIBClient.Verify(ib => ib.RequestAccountUpdates(true, "U222"));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("Request account updates for U111")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("Request account updates for U222")));

            // Request Open Orders
            mockIBClient.Verify(ib => ib.RequestOpenOrders());
            // Request IDs
            mockIBClient.Verify(ib => ib.RequestIds(1));

            logger.Verify(
                log =>
                log.AddLog(LoggerLevel.Information,
                           It.IsRegex("Connected, Server Version = 42, Connection Time = 03:04:05 2011-11-08")));

            // EmitConncted
            userProvider.Verify(up => up.EmitConnected());

            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* openConnection.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* CheckTWS.*")));
            logger.Verify(log => log.AddLog(LoggerLevel.Detail, It.IsRegex("END.* Initialize.*")));

            // never saw an error?
            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsAny<string>()), Times.Never(),
                          "unexpected error log");

            // State
            Assert.IsTrue(target.IsConnected, "IsConnected");
            Assert.IsTrue(target.TWSConnected, "TWSConnected");
        }

        /// <summary>
        ///A test for Version
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void VersionTest()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            string version = target.Version();
            //Version must match AssemblyVersion in Mockups->AssemblyInfo.cs
            Assert.AreEqual("1.42.3.5", version, "Version");
        }

        /// <summary>
        ///A test for error
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void errorTest()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor();
            target.InitLogger();

            target.error("My Error Text");

            logger.Verify(log => log.AddLog(LoggerLevel.Error, It.IsRegex("My Error Text")));
        }

        /// <summary>
        ///A test for info
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Mockups.dll")]
        public void infoTest()
        {
            StingrayOQ_Accessor target = new StingrayOQ_Accessor(); // TODO: Initialize to an appropriate value
            target.InitLogger();

            target.info("My Info Text");

            logger.Verify(log => log.AddLog(LoggerLevel.Information, It.IsRegex("My Info Text")));
        }
    }
}
