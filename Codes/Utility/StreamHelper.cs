using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    static class StreamHelper
    {
        public static void CopySegmentTo(this Stream source, Stream destination, int length)
        {
            var buffer = new byte[1024 * 10];
            var len = length;
            var readed = source.Read(buffer, 0, Math.Min(len, buffer.Length));
            while (readed > 0)
            {
                destination.Write(buffer, 0, readed);
                len -= readed;
                if (len <= 0)
                {
                    break;
                }
                readed = source.Read(buffer, 0, Math.Min(len, buffer.Length));
            }
        }
    }
}
