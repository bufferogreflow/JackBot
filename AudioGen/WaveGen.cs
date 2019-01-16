using System;
using System.IO;
namespace JackBotV2.AudioGen
{
    public enum WaveType
    {
        //All the different kinds of waves go here
        SineWave = 0,
        Noise = 1
    }

    public class WaveGen
    {
        Wave.HeaderChunk header;
        Wave.FormatChunk format;
        Wave.DataChunk data;

        public WaveGen(WaveType type, params float[] freqs)
        {
            header = new Wave.HeaderChunk();
            format = new Wave.FormatChunk();
            data = new Wave.DataChunk();

            switch (type)
            {
                case WaveType.SineWave:
                    uint numSamples = format.sampleRate * format.channels;
                    data.dataArray = new short[numSamples];
                    short[] tempArray = new short[numSamples];
                    int amplitude = 32767;
                    int arrayLen = 0;
                    double freq = 440.0f;
                    double timePeriod = (Math.PI * 2 * freq) / (format.sampleRate * format.channels);

                    for (uint i = 0; i < numSamples - 1; i++)
                    {
                        // Take into account each channel
                        for (int channel = 0; channel < format.channels; channel++)
                        {
                            // Data for each channel
                            data.dataArray[i + channel] = Convert.ToInt16(amplitude * Math.Sin(timePeriod * i));
                        }
                    }                   
                    data.chunkSize = (uint)(data.dataArray.Length * (format.bitsPersample / 8));
                    break;
                case WaveType.Noise:
                    numSamples = format.sampleRate * format.channels;
                    data.dataArray = new short[numSamples];
                    amplitude = 32767;
                    Random rnd = new Random();
                    for (uint i = 0; i < numSamples - 1; i++)
                    {
                        for (int channel = 0; channel < format.channels; channel++)
                        {
                            data.dataArray[i + channel] = Convert.ToInt16(i / amplitude);
                        }
                    }
                    data.chunkSize = (uint)(data.dataArray.Length * (format.bitsPersample / 8));
                    break;

            }
        }

        public void Save(string filePath, bool delete = false)
        {
            if (delete)
            {
                try
                {
                    FileInfo file = new FileInfo(filePath);
                    file.Delete();
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);

                bw.Write(header.ID.ToCharArray());
                bw.Write(header.fileSize);
                bw.Write(header.format.ToCharArray());

                bw.Write(format.ID.ToCharArray());
                bw.Write(format.chunkSize);
                bw.Write(format.audioFormat);
                bw.Write(format.channels);
                bw.Write(format.sampleRate);
                bw.Write(format.byteRate);
                bw.Write(format.blockAlign);
                bw.Write(format.bitsPersample);

                bw.Write(data.ID.ToCharArray());
                bw.Write(data.chunkSize);

                foreach (short datapoint in data.dataArray)
                {
                    bw.Write(datapoint);
                }

                bw.Seek(4, SeekOrigin.Begin);
                uint filesize = (uint)bw.BaseStream.Length;
                bw.Write(filesize - 8);
                bw.Close();
                fs.Close();
            }
        }
    }
}
