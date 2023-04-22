using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallPaper.Utility
{
    internal class EnumVar
    {
        public enum ExitReason
        {
            ExitByWindow=0,
            ExitByTray=1,
        }
        public enum ButtonStatus
        {
            Stopped=0,
            Running=1,
            
        }
    }
}
