using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Ion.Net.Messages;
using Ion.Specialized.Encoding;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Utilities;
using System.Collections.Generic;

namespace Ion.Net.Connections.Codec
{
    internal class NetworkDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer buffer, List<object> output)
        {
            buffer.MarkReaderIndex();

            if (buffer.ReadableBytes < 5)
            {
                // If the incoming data is less than 6 bytes, it's junk.
                return;
            }

            byte delimiter = buffer.ReadByte();
            buffer.ResetReaderIndex();

            if (delimiter == 60)
            {
                string policy = "<?xml version=\"1.0\"?>\r\n"
                        + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                        + "<cross-domain-policy>\r\n"
                        + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                        + "</cross-domain-policy>\0)";

                ctx.Channel.WriteAndFlushAsync(Unpooled.CopiedBuffer(IonEnvironment.GetDefaultTextEncoding().GetBytes(policy)));
            }
            else
            {
                buffer.MarkReaderIndex();
                int length = Base64Encoding.DecodeInt32(new byte[] { buffer.ReadByte(), buffer.ReadByte(), buffer.ReadByte() });

                if (buffer.ReadableBytes < length)
                {
                    buffer.ResetReaderIndex();
                    return;
                }

                if (length < 0)
                {
                    return;
                }

                var message = new byte[length];
                buffer.ReadBytes(message, 0, message.Length);

                var messageHeader = Base64Encoding.DecodeUInt32(new byte[] { message[0], message[1] });
                output.Add(new ClientMessage(messageHeader, message));
            }
        }
    }
}