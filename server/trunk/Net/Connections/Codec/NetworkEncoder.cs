using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Ion.Net.Messages;

namespace Ion.Net.Connections.Codec
{
    internal class NetworkEncoder : MessageToMessageEncoder<byte[]>
    {
        protected override void Encode(IChannelHandlerContext ctx, byte[] bytes, List<object> output)
        {
            try
            {
                var buffer = Unpooled.Buffer();
                buffer.WriteBytes(bytes);
                output.Add(buffer);
            }
            catch (Exception ex)
            {
                IonEnvironment.GetLog().WriteError($"Error occurred: {ex.ToString()}");
            }
        }
    }
}