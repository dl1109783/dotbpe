using System;
using System.Collections.Generic;
using System.Text;

namespace DotBPE.Rpc
{
    [AttributeUsage(AttributeTargets.Interface , AllowMultiple = false, Inherited = true)]
    public class RpcServiceAttribute : Attribute
    {
        public RpcServiceAttribute()
        {

        }
        public RpcServiceAttribute(ushort serviceId,string groupName="default")
        {
            this.ServiceId = serviceId;
            this.GroupName = groupName;
        }

        public ushort ServiceId { get; set; }

        public string GroupName { get; set; } = "default";
    }
}