using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Krs.Ats.IBNet96;

namespace finantic.OQPlugins
{
    #region ExecutionInfo
    public class ExecutionInfo
    {
        public Contract contract = null;
        public string symbol = null;
        public Execution execution = null;
        public int OrderId = -1;
    }
    #endregion

}
