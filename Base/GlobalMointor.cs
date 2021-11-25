using Communication;
using Communication.ModelBus;
using SoftwareManage.BLL;
using SoftwareManage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManage.Base
{
    public class GlobalMointor
    {
        public static List<StorageModel> StorageList { get; set; }
        public static List<DeviceModel> DeviceList { get; set; } = new List<DeviceModel>();
        public static SerialInfo SerialInfo { get; set; }
        static Task mainTask = null;
        static bool isRunning = true;
        static RTU rtuInstance = null;
        public static void Start(Action sucessAction,Action<string> faultAction)
        {
            //创建线程
            mainTask = Task.Run(() =>
            {
                IndustrialBLL bll = new IndustrialBLL();
                //获取串口配置信息
                var si = bll.InitSerialInfo();
                if (si.State)
                    SerialInfo = si.Data;
                else
                {
                    faultAction(si.Message);return;
                }

                //获取存储区信息
                var sa = bll.InitStorageArea();
                if (sa.State==true)
                {
                    StorageList = sa.Data;
                }
                else
                {
                    faultAction(sa.Message); return;
                }
                //初始化变量集合及警戒值
                var dr = bll.InitDevice();
                if (dr.State)
                {
                    DeviceList = dr.Data;
                }
                else
                {
                    faultAction(dr.Message); return;
                }

                //初始化串口通信
                rtuInstance = RTU.GetInstance(SerialInfo);
                if (rtuInstance.Connection())
                {
                    sucessAction();

                    //不停的向串口请求信息
                    while (isRunning)
                    {

                    }
                }
                else 
                {
                    faultAction("程序无法启动。串口连接失败，请检查设备连接是否正常"); 
                }
            });
        }

        public static void Dispose()
        {
            isRunning = false;

            if (rtuInstance != null)
                rtuInstance.Dispose();

            if (mainTask != null)
                mainTask.Wait();
        }

    }
}
