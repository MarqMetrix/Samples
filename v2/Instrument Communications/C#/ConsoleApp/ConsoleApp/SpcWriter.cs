using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    static class SpcWriter
    {

        public static void WriteSpcStreamData(Stream stream, double[] samplesDataX, double[] samplesDataY, DateTime sampleTakenDate, string logData, string spcHeaderText = "MarqMetrix", bool unevenlySpaced = true, bool isProcessed = true, bool multifile = false)
        {
            var dataWriter = new BinaryWriter(stream);
            {
                if (unevenlySpaced) // storage format uneven is mostly used for Raman shift, even for raw spectra
                {
                    var min = samplesDataX.Min();
                    var max = samplesDataX.Max();
                    WriteSpcHeader(dataWriter, min, max, samplesDataX.Length, sampleTakenDate, spcHeaderText);
                }
                else
                    WriteSpcHeader(dataWriter, 0, samplesDataY.Length - 1, samplesDataX.Length, sampleTakenDate, spcHeaderText);

                // if unevenly spaced - write the x values
                if (unevenlySpaced) // write x data for raman shift
                    WriteXdata(dataWriter, samplesDataX);

                WriteSubHeader(dataWriter, unevenlySpaced);

                // write subfile Y data if unevenly spaced or data if evenly spaced
                WriteYdata(dataWriter, samplesDataY);

                // write the log data
                if (logData.Length > 0)
                {
                    WriteLogDataBlock(dataWriter, logData.Length);
                    WriteLogData(dataWriter, logData);
                }

                // write flush data
                dataWriter.Flush();

#if LEGACY_WINDOWS

#else
                dataWriter.Close();
#endif
            }
        }

        private static void WriteSpcHeader(BinaryWriter dataWriter, double firstRamanShift, double lastRamanShift, int spectralPoints, DateTime fdate, string description, bool unevenlySpaced = true)
        {
            dataWriter.Write((byte)(unevenlySpaced ? 0x80 : 0x00)); // uneven 0x80, 0 = evenly spaced
            dataWriter.Write((byte)0x4B);      // new SPC format
            dataWriter.Write((byte)0x0B);      //Raman spectrum
            dataWriter.Write((byte)0x80);      // stored as floats
            dataWriter.Write((uint)spectralPoints);      // spectral points
            dataWriter.Write(firstRamanShift);  // First Raman shift
            dataWriter.Write(lastRamanShift);   // Last Raman shift
            dataWriter.Write((uint)1);         // number of sub files
            dataWriter.Write((byte)13);        // x axis type Raman shift
            dataWriter.Write((byte)4);         // y axis type counts
            dataWriter.Write((byte)0);         // not used
            dataWriter.Write((byte)0);         // not used
            dataWriter.Write(ConvertDateTime(fdate)); // Write file creation date-time

            var blankdata = new byte[9];

            dataWriter.Write(blankdata);        // do not specify resolution
            dataWriter.Write(blankdata);        // 9 characters is not enough to give a meaningful description of the instrument. Do not use
            dataWriter.Write((ushort)0);       // NA
            var floatNull = new float[8];
            foreach (var item in floatNull)
                dataWriter.Write(item);         // fspare

            // write experiment name, etc 130 char
            var descriptionArray = new char[130];
            if (description.Length > 130)
                description = description.Substring(0, 130);
            var charArray = description.ToCharArray();
            charArray.CopyTo(descriptionArray, 0);
            dataWriter.Write(descriptionArray);

            dataWriter.Write(new char[30]); // fcatxt

            var spectralPointsSize = unevenlySpaced ? spectralPoints * 8 + 0x220 : spectralPoints * 4 + 0x220;  // 0x4220 or 0x2220
            dataWriter.Write((uint)spectralPointsSize); // data dependent LogData offset
                                                        //     dataWriter.Write((uint)(unevenlySpaced ? 0x4220 : 0x2220)); // data dependent LogData offset
                                                        //     dataWriter.Write((UInt32)(4 * 2048 ) + 512 +32 + 64); // data dependent LogData offset
            dataWriter.Write((uint)0);         // fmods
            dataWriter.Write((byte)0);         // 
            dataWriter.Write((byte)0);         // 
            dataWriter.Write((ushort)0);       // fsampin
            dataWriter.Write((float)0);        // fsampin
            dataWriter.Write(new char[48]);     // fsampin
            dataWriter.Write((float)0);        // fzinc
            dataWriter.Write((uint)0);         // fwplanes
            dataWriter.Write((float)0);        // fwinc
            dataWriter.Write((byte)0);         // fwtype
            dataWriter.Write(new char[187]);    // freserv
        }

        private static void WriteSubHeader(BinaryWriter dataWriter, bool unevenlySpaced)
        {
            dataWriter.Write((byte)0x00);      // subflgs
            dataWriter.Write((byte)(unevenlySpaced ? 0x80 : 0x00)); // subexp
            dataWriter.Write((ushort)0);       // subindx
            dataWriter.Write((float)0);        // subtime
            dataWriter.Write((float)0);        // subnext
            dataWriter.Write((float)0);        // subnois
            dataWriter.Write((uint)0);         // subnpts
            dataWriter.Write((uint)0);         // subscan
            dataWriter.Write((float)0);        // subwlevel
            dataWriter.Write(new char[4]);      // subresv
        }
        private static void WriteXdata(BinaryWriter dataWriter, double[] samplesDataX, bool isProcessed = true)
        {
            foreach (var item in samplesDataX)
            {
                dataWriter.Write(Convert.ToSingle(item));
            }
        }
        private static void WriteYdata(BinaryWriter dataWriter, double[] samplesData)
        {
            foreach (var item in samplesData)
            {
                dataWriter.Write(Convert.ToSingle(item)); // new SPC format
            }
        }
        private static void WriteLogDataBlock(BinaryWriter dataWriter, int logCnt)
        {
            var logsizd = 64 + logCnt;
            var logsizma = logsizd / 4096;
            var logsizmaMod = logsizd % 4096;
            var logsizm = logsizma * 4096;
            if (logsizmaMod > 0)
            {
                logsizm += 4096;
            }
            dataWriter.Write((uint)logsizd);    // logsizd byte size of log block and header
            dataWriter.Write((uint)logsizm);    // logsizm byte size for memory rounded up to 4096
            dataWriter.Write((uint)64);         // logxto byte offset from beginnng of log block
            dataWriter.Write((uint)0);          // logbins private binary bloc
            dataWriter.Write((uint)0);          // logdsk fixed
            dataWriter.Write(new char[42]);     // logspar
        }
        private static void WriteLogData(BinaryWriter dataWriter, string logDataBlockDataGoesHere)
        {
            dataWriter.Write(logDataBlockDataGoesHere);
        }

        private static uint ConvertDateTime(DateTime dateTimeNow)
        {
            uint dateTimeSpc = 0;

            dateTimeSpc |= (uint)dateTimeNow.Year & 0xFFF;
            dateTimeSpc <<= 4;
            dateTimeSpc |= (uint)dateTimeNow.Month & 0xF;
            dateTimeSpc <<= 5;
            dateTimeSpc |= (uint)dateTimeNow.Day & 0x1F;
            dateTimeSpc <<= 5;
            dateTimeSpc |= (uint)dateTimeNow.Hour & 0x1F;
            dateTimeSpc <<= 6;
            dateTimeSpc |= (uint)dateTimeNow.Minute & 0x3F;

            return dateTimeSpc;
        }

        private static (double Min, double Max) GetMinMaxSpectrum(double[] samplesData)
            => (samplesData.Min(), samplesData.Max());

    }
}
