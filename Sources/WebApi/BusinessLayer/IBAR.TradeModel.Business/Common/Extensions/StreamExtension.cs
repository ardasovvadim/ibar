using System.IO;

namespace IBAR.TradeModel.Business.Common.Extensions
{
    public static class StreamExtension
    {
        public static byte[] GetByteArray(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
