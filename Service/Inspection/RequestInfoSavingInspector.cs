using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Xml;
using Domain.Entities;
using Domain.Enums;
using Message = System.ServiceModel.Channels.Message;

namespace Service.Inspection
{
    public class RequestInfoSavingInspector : IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var actionName = request.Headers.Action ?? request.Headers.To.ToString();
            actionName = actionName.Substring(actionName.LastIndexOf('/') + 1);

            RequestType requestType;
            if (Enum.TryParse(actionName, out requestType))
            {
                if (requestType == RequestType.Setup)
                    return null; //probably no database yet, neither do we want to log

                var outp = new RequestInfo
                    {
                        RequestType = requestType
                    };

                FillDeviceAndAccountId(ref request, ref outp);

                using (var ctx = new TypeFineContext())
                {
                    ctx.RequestInfos.Add(outp);
                    ctx.SaveChanges();
                }
            }
            return null;


        }

        private static void FillDeviceAndAccountId(ref Message message, ref RequestInfo requestInfo)
        {
            //load the old message into XML
            var msgbuf = message.CreateBufferedCopy(int.MaxValue);
            var tmpMessage = msgbuf.CreateMessage();

            var xdr = tmpMessage.GetReaderAtBodyContents();

            var xdoc = new XmlDocument();
            xdoc.Load(xdr);
            xdr.Close();


            //transform the xmldocument
            var nsmgr = new XmlNamespaceManager(xdoc.NameTable);
            nsmgr.AddNamespace("a", "http://schemas.datacontract.org/2004/07/Service.Contracts");

            var accountIdNode = xdoc.SelectSingleNode("//a:AccountId", nsmgr);
            if (accountIdNode != null) requestInfo.AccountId = accountIdNode.InnerText;

            var deviceIdNode = xdoc.SelectSingleNode("//a:DeviceId", nsmgr);
            if (deviceIdNode != null) requestInfo.DeviceId = deviceIdNode.InnerText;

            message = msgbuf.CreateMessage();
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
           if ( WebOperationContext.Current != null)
               WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}