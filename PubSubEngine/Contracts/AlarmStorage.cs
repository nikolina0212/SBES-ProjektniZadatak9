using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class AlarmStorage
    {
        public static List<Alarm> alarms = new List<Alarm>();
        public static Dictionary<byte[], byte[]> alarmsEncripted = new Dictionary<byte[], byte[]>();
        public static Dictionary<byte[], Alarm> alarmsSigned = new Dictionary<byte[], Alarm>();
        
    }
}
