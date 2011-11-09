using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet96;
using Mockups;

namespace finantic.OQPlugins
{
    // return mock objects for external dependencies.
    // enables isaolation and unit tests
    public static class Factory
    {
        public static Logger mockLogger;
        public static IBClient mockIBClient;
        public static UserProviderWrapper userProviderWrapper;

        // return mocked ib client
        public static IBClient GetIbClient()
        {
            return mockIBClient;
        }       

        // return mocked logger
        public static Logger GetLogger(string source, string name)
        {
            return mockLogger;
        }

        // return a mocked wrapper for UserProvider
        public static UserProviderWrapper GetUserProviderWrapper()
        {
            return userProviderWrapper;
        }
    }
}
