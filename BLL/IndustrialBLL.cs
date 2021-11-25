using Communication;
using SoftwareManage.DAL;
using SoftwareManage.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
//功能一：配置信息初始化，从APP.config中获取串口得配置信息，加载到这个类中返回一个数据初始化得数据对象,数据对象类型为DataResult
//功能二：
namespace SoftwareManage.BLL
{
    public class IndustrialBLL
    {
        //创建一个全局数据访问变量da
        DataAccess da = new DataAccess();
        //获取串口信息
        public DataResult<SerialInfo> InitSerialInfo()
        {
            DataResult<SerialInfo> result = new DataResult<SerialInfo>();
            

            try
            {
                SerialInfo si = new SerialInfo();
                si.PortName = ConfigurationManager.AppSettings["port"].ToString();
                si.BaudRate = int.Parse(ConfigurationManager.AppSettings["baud"].ToString());
                si.DataBit = int.Parse(ConfigurationManager.AppSettings["data_bit"].ToString());
                si.Parity = (Parity)Enum.Parse(typeof(Parity),ConfigurationManager.AppSettings["Parity"].ToString(),true);
                si.StopBits = (StopBits)Enum.Parse(typeof(StopBits), ConfigurationManager.AppSettings["stopbit"].ToString(), true);
                result.State = true;
                result.Data = si;
            }
            catch (Exception ex)
            {

                result.Message = ex.Message;
            }
            return result;
        }

        //获取数据访问层的信息，返回给上层使用

        //获取存储区信息
        public DataResult<List<StorageModel>> InitStorageArea() 
        {
            DataResult<List<StorageModel>> result = new DataResult<List<StorageModel>>();

            try
            {

                //用sa接受从数据库获取的存储区得信息
                var sa = da.GetStorageArea();
                result.State = true;

                //将DataTable中的数据取出，并转化为StorageModel的一个列表，DataTable中每一行就是一个StorageModel类型对象
                result.Data = (from q in sa.AsEnumerable()
                               select new StorageModel
                               {
                                   id = q.Field<string>("id"),
                                   SlaveAddress = q.Field<Int32>("slave_add"),
                                   FunCode = q.Field<string>("fun_code"),
                                   StartAddress = int.Parse(q.Field<string>("start_reg")),
                                   Length = int.Parse(q.Field<string>("length"))
                               }).ToList();
            }
            catch (Exception ex)
            {

                result.Message = ex.Message;
            }


            return result;
        }


        public DataResult<List<DeviceModel>> InitDevice() 
        {
            DataResult<List<DeviceModel>> result = new DataResult<List<DeviceModel>>();
            try
            {
                var device = da.GetDevice();
                var monitorValues = da.GetMonitorValues();
                List<DeviceModel> deviceList = new List<DeviceModel>();
                foreach (var dr in device.AsEnumerable())
                {
                    DeviceModel dModel = new DeviceModel();
                    deviceList.Add(dModel);
                    dModel.DeviceID = dr.Field<String>("d_id");
                    dModel.DeviceName = dr.Field<String>("d_name");

                    foreach (var mv in monitorValues.AsEnumerable().Where(m => m.Field<string>("id") ==
                    dModel.DeviceID))
                    {
                        MonitorValueModel mvm = new MonitorValueModel();
                        dModel.MonitorValueList.Add(mvm);

                        mvm.ValueId = mv.Field<string>("value_id");
                        mvm.ValueName = mv.Field<string>("value_name");
                        mvm.StorageAreaId = mv.Field<string>("s_area_id");
                        mvm.StartAddress = mv.Field<Int32>("address");
                        mvm.DataType = mv.Field<string>("data_type");
                        mvm.IsAlarm = mv.Field<Int32>("is_alarm") == 1;
                        mvm.ValueDesc = mv.Field<string>("description");
                        mvm.Unit = mv.Field<string>("unit");

                        //警戒值
                        var column = mv.Field<string>("alarm_lolo");
                        mvm.LoLoAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_low");
                        mvm.LowAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_high");
                        mvm.HigehAlarm = column == null ? 0.0 : double.Parse(column);
                        column = mv.Field<string>("alarm_hihi");
                        mvm.HiHiAlarm = column == null ? 0.0 : double.Parse(column);

                        mvm.ValueStateChanged = (state, msg, value_id) =>
                        {


                            var index = dModel.WarningMessageList.ToList().FindIndex(w => w.ValueId ==
                            value_id);
                            if (index > -1)
                                dModel.WarningMessageList.RemoveAt(index);

                            if (state != Base.MonitorValueState.OK)
                            {
                                dModel.IsWarning = true;
                                dModel.WarningMessageList.Add(new WarningMessageModel
                                {
                                    ValueId = value_id,
                                    Message = msg
                                });
                            }


                            var ss = dModel.WarningMessageList.Count > 0;
                            if (dModel.IsWarning != ss)
                            {
                                dModel.IsWarning = ss;
                            }

                        };
                }   }

                result.State = true;
                result.Data = deviceList;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }


            return result;
        }







    }
}
