using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet96;

namespace finantic.OQPlugins
{
    // return external dependencies.
    // enables isaolation and unit tests
    // the static class Factory is replaced during unit tests 
    // by another factory which returns mock objects.
    public static class Factory
    {
        // return live ib client
        public static IBClient GetIbClient()
        {
            return new IBClient();
        }       

        // return live logger
        public static Logger GetLogger(string source, string name)
        {
            return new Logger(source, name);
        }
    }
}
