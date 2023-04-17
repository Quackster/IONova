using System;

namespace Ion.Net.Messages
{
    public interface ISerializableObject
    {
        void Serialize(ServerMessage message);
    }
}
