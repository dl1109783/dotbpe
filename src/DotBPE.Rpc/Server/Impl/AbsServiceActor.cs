using DotBPE.Rpc.Protocol;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Peach;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Environment = DotBPE.Rpc.Internal.Environment;
using DotBPE.Rpc.Diagnostics;

namespace DotBPE.Rpc.Server
{
    public abstract class AbsServiceActor : IServiceActor<AmpMessage>
    {
        private ILogger _logger;
        protected ILogger Logger
        {
            get => this._logger ?? NullLogger.Instance;
            set => this._logger = value;
        }

        public abstract string Id { get; }

        public abstract string GroupName { get; }

        /// <summary>
        /// process receive message from remote client
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task ReceiveAsync(ISocketContext<AmpMessage> context, AmpMessage message)
        {

            AmpMessage rsp =null;
            try
            {
                DotBPEDiagnosticListenerExtensions.Listener.ServiceActorReceiveRequest(context, message);

                Logger.LogDebug("Receive message,Id={0}", message.Id);
                rsp = await ProcessAsync(message);
                rsp.Sequence = message.Sequence; //通讯请求序列

                await context.SendAsync(rsp);

            }
            catch (ClosedChannelException closedEx)
            {
                Logger.LogError(closedEx, "Receive message occ error,channel closed,{messageId}", message.Id);
                DotBPEDiagnosticListenerExtensions.Listener.ServiceActorException(context, message, closedEx);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Receive message occ error ,messageId={messageId}", message.Id);
                rsp = await SendErrorResponseAsync(context, message);
                DotBPEDiagnosticListenerExtensions.Listener.ServiceActorException(context, message, ex);
            }
            finally
            {
                DotBPEDiagnosticListenerExtensions.Listener.ServiceActorSendResponse(context,message,rsp);
            }
        }

        /// <summary>
        /// remote call
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        protected abstract Task<AmpMessage> ProcessAsync(AmpMessage req);


        /// <summary>
        /// 发送服务端意外错误的消息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reqMessage"></param>
        /// <returns></returns>
        private async Task<AmpMessage> SendErrorResponseAsync(ISocketContext<AmpMessage> context, AmpMessage reqMessage)
        {
            var rsp = AmpMessage.CreateResponseMessage(reqMessage.ServiceId, reqMessage.MessageId);
            rsp.InvokeMessageType = InvokeMessageType.Response;
            rsp.Sequence = reqMessage.Sequence;
            rsp.Code = RpcErrorCodes.CODE_INTERNAL_ERROR; //内部错误
            try
            {
                await context.SendAsync(rsp);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "send error response fail:" + ex.Message);
            }

            return rsp;
        }
    }
}
