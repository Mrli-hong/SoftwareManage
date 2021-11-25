using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareManage.Model
{
    //用来存放数据逻辑层处理过后得存储区信息
    public class StorageModel
    {
        public string id { get; set; }
        public int SlaveAddress { get; set; }

        public string FunCode { get; set; }

        public int StartAddress { get; set; }
        public int Length { get; set; }


    }
}
