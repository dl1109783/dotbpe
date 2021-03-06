﻿using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Peach.Buffer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Peach
{
    /// <summary>
    /// 链接上下文的默认实现
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class SocketContext<TMessage> : ISocketContext<TMessage> where TMessage : Messaging.IMessage
    {
        private readonly IChannel _channel;
        private readonly Protocol.IProtocol<TMessage> _protocol;
        public SocketContext(IChannel channel,Protocol.IProtocol<TMessage> protocol)
        {
            this._channel = channel;
            this._protocol = protocol;         
           
        }
        public bool Active
        {
            get
            {
                return this._channel.Active;
            }
        }

        public IChannel Channel
        {
            get
            {
                return this._channel;
            }
        }

        public string Id
        {
            get {
                return this._channel.Id.AsLongText();
            }      
        }

        public IPEndPoint LocalEndPoint
        {
            get {
                return (IPEndPoint) this._channel.LocalAddress;
            }
           
        }

        public IPEndPoint RemoteEndPoint
        {
            get {
                return (IPEndPoint) this._channel.RemoteAddress;
            }
        }
        public Task SendAsync(TMessage message)
        {
            if (this._channel.IsWritable)
            {
                var buffer = GetBuffer(message);
                if(buffer != null)
                {
                    return this._channel.WriteAndFlushAsync(buffer);
                }                
            }

            return Task.CompletedTask;
        }

        private IByteBuffer GetBuffer(TMessage message)
        {
            if (message == null )
            {
                return null;
            }
            var length = message.Length;
          
            if(length <= 0)
            {
                return null;
            }

            var buff = this._channel.Allocator.Buffer(length);
            IBufferWriter writer = ByteBufferManager.CreateBufferWriter(buff);
            this._protocol.Pack(writer, message);
            return buff;
        }
    }
}
