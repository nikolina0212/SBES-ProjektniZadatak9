using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    
    [DataContract]
    [Serializable]
    public class Alarm
    {
        [DataMember]
        private DateTime generatingTime;
        [DataMember]
        private string messegAlarm;
        [DataMember]
        private int risk;

        public Alarm(DateTime generatingTime, string messegAlarm, int risk)
        {
            GeneratingTime = generatingTime;
            MessegAlarm = messegAlarm;
            Risk = risk;
        }

        public int Risk
        {
            get
            {
                return risk;
            }
            set
            {
                if (value <= 100 && value >= 1)
                    risk = value;
            }
        }

        public DateTime GeneratingTime { get => generatingTime; set => generatingTime = value; }
        public string MessegAlarm { get => messegAlarm; set => messegAlarm = value; }
    }
}
