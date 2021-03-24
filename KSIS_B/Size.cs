using System;
using System.Collections.Generic;
using System.Text;

namespace KSIS_B
{
    internal class Size: Program
    {
        public int SizeCount(ICMP pack)
        {
            int packetsize;
            packetsize = pack.MessageSize + 4;
            return packetsize;
        }
    }
}
