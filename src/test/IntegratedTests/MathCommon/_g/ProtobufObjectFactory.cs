// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: ProtobufObjectfactory

#region Designer generated code

using Google.Protobuf;

namespace MathCommon
{
    public class ProtobufObjectFactory
    {
        public static IMessage GetRequestTemplate(int serviceId, int messageId)
        {

    
            if (serviceId == 10005 && messageId == 1)
            {
                return new AddReq();
            }
    

            return null;
        }

        public static IMessage GetResponseTemplate(int serviceId, int messageId)
        {

    
            if (serviceId == 10005 && messageId == 1)
            {
                return new AddRes();
            }
    

            return null;
        }
    }
}

#endregion Designer generated code