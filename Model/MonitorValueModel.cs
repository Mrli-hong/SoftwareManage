using SoftwareManage.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManage.Model
{
    //接受数据访问层返回的监控信息
    public  class MonitorValueModel
    {
        public Action<MonitorValueState, string,string> ValueStateChanged;
        public string ValueId { get; set; }
        public string ValueName { get; set; }
        public string StorageAreaId{ get; set; }
        public int StartAddress  { get; set; }
        public string DataType { get; set; }
        public bool IsAlarm { get; set; }
        public double LoLoAlarm { get; set; }
        public double LowAlarm { get; set; }
        public double HigehAlarm { get; set; }
        public double HiHiAlarm { get; set; }
        //描述信息，提示监控点位的高低
        public string ValueDesc { get; set; }
        public string Unit  { get; set; }
        //全局监控初始化列表之后从串口获得信息，写回列表，在写回的时候需要判断写回值是否在监控点位上
        private double  _currentValue;

        public double  CurrentValue
        {
            get { return _currentValue; }
            set { 
                _currentValue = value; 

                if(IsAlarm)
                {
                    string msg = ValueDesc;
                    MonitorValueState state = MonitorValueState.OK;


                    if (value < LoLoAlarm)
                    { msg += "极低"; state = MonitorValueState.LoLo; }
                    else if (value < LowAlarm)
                    { msg += "过低"; state = MonitorValueState.Low; }
                    else if (value > HiHiAlarm)
                    { msg += "极高"; state = MonitorValueState.HiHi; }
                    else if (value > HigehAlarm)
                    { msg += "过高"; state = MonitorValueState.High; }

                    ValueStateChanged(state, msg + "。当前值：" + value.ToString(),ValueId);
                }
            
            }
        }

    }
}
