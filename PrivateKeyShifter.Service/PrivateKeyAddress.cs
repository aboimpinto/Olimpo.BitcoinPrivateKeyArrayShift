using MongoDB.Bson.Serialization.Attributes;

namespace PrivateKeyShifter.Service
{
    public class PrivateKeyAddress
    {
        [BsonElement("PrivateKeyBytes")]
        public byte[] PrivateKeyBytes { get; set; }
    }
}