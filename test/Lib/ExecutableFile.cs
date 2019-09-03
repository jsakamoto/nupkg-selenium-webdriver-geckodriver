using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Selenium.WebDriver.GeckoDriver.NuPkg.Test.Lib
{
    public class ExecutableFile
    {
        public enum Format
        {
            Unknown,
            PE32,
            PE64,
            ELF,
            MachO
        }

        internal static Format DetectFormat(string path)
        {
            const int maxHeaderBytesLength = 4;
            if (new FileInfo(path).Length < maxHeaderBytesLength) return Format.Unknown;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var headerBytes = new byte[maxHeaderBytesLength];
                stream.Read(headerBytes, 0, headerBytes.Length);
                if (headerBytes.Take(2).SequenceEqual(new[] { (byte)'M', (byte)'Z' }))
                {
                    var buff = new byte[4];
                    const int offsetOfNEHeaderOffset = 60;
                    stream.Seek(offsetOfNEHeaderOffset, SeekOrigin.Begin);
                    stream.Read(buff, 0, 4);
                    var posOfNEHeader = BitConverter.ToInt32(buff, 0);

                    stream.Seek(posOfNEHeader, SeekOrigin.Begin);
                    stream.Read(buff, 0, 4);
                    if (!buff.Take(4).SequenceEqual(new byte[] { (byte)'P', (byte)'E', 0, 0 }))
                        return Format.Unknown;

                    stream.Read(buff, 0, 2);
                    if (buff.Take(2).SequenceEqual(new byte[] { 0x4c, 0x01 }))
                        return Format.PE32;
                    if (buff.Take(2).SequenceEqual(new byte[] { 0x64, 0x86 }))
                        return Format.PE64;

                    return Format.Unknown;
                }
                if (headerBytes.Take(4).SequenceEqual(new byte[] { 0x7f, (byte)'E', (byte)'L', (byte)'F' })) return Format.ELF;
                if (headerBytes.Take(4).SequenceEqual(new byte[] { 0xcf, 0xfa, 0xed, 0xfe })) return Format.MachO;
            }
            return Format.Unknown;
        }
    }
}
