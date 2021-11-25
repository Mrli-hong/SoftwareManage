using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManage.Model
{
    public class DeviceModel
    {
        public string  DeviceID { get; set; }
    
        public string DeviceName { get; set; }

        //检测设备是否报警
        public bool IsWarning { get; set; } = false;

        //动态列表显示设备参数信息
        public ObservableCollection<MonitorValueModel> MonitorValueList { get; set; } = new 
            ObservableCollection<MonitorValueModel>();

        //动态列表显示报错提示
        public ObservableCollection<WarningMessageModel> WarningMessageList { get; set; } = new
            ObservableCollection<WarningMessageModel>();
     

    }
}
