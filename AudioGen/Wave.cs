using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace JackBotV2.AudioGen
{
    public class Wave
    {
        public class HeaderChunk
        {
            public string ID;
            public uint fileSize;
            public string format;
            public HeaderChunk()
            {
                fileSize = 0;
                ID = "RIFF";
                format = "WAVE";
            }
        }

        public class FormatChunk
        {
            public string ID;
            public uint chunkSize;
            public ushort audioFormat;
            public ushort channels;
            public uint sampleRate;
            public uint byteRate;
            public ushort blockAlign;
            public ushort bitsPersample;
            public FormatChunk()
            {
                ID = "fmt ";
                chunkSize = 16;
                audioFormat = 1;
                channels = 2;
                sampleRate = 8000;
                bitsPersample = 16;
                blockAlign = (ushort)((channels * bitsPersample) / 8);
                byteRate = sampleRate * blockAlign;
            }
        }

        public class DataChunk
        {
            public string ID;
            public uint chunkSize;
            public short[] dataArray; //byte array for 8-bit audio
            public DataChunk()
            {
                ID = "data";
                chunkSize = 0;
                dataArray = new short[0];
            }
        }
    }
}
