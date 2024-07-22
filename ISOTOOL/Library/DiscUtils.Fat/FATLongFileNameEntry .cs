using DiscUtils.Streams;
using System;
using System.IO;
using System.Linq;

namespace DiscUtils.Fat
{
    public class FATLongFileNameEntry
    {
      public  byte entry_index;
        byte[] characters1;
        FatAttributes attributes;
        byte entry_type;
        byte checksum;
        byte[] characters2;
        ushort zero = 0;
        byte[] characters3;
        static byte normal_filename_length = 8;
        static byte normal_extension_length = 3;
        public static byte[] DefaultZero(int len)
        {
            return System.Linq.Enumerable.Repeat((byte)0, len).ToArray();
        }

        public static byte lfn_entry_checksum(byte[] bytes)
        {
            byte[] filename = bytes.Take(normal_filename_length).ToArray();
            byte[] extension = bytes.Skip(normal_filename_length).Take(normal_extension_length).ToArray();

            byte checksum = filename[0];
            for (byte i = 1; i < normal_filename_length; i++)
                checksum = (byte)(((checksum << 7) + (checksum >> 1) + filename[i]) & 0xff);
            for (byte i = 0; i < normal_extension_length; i++)
                checksum = (byte)(((checksum << 7) + (checksum >> 1) + extension[i]) & 0xff);
            return checksum;
        }
        public FATLongFileNameEntry(byte[] fileshort, string filenamelong)
        {
            characters1 = DefaultZero(10);
            characters2 = DefaultZero(12);
            characters3 = DefaultZero(4);

            byte[] namebytes = System.Text.Encoding.Unicode.GetBytes(filenamelong);
            if (namebytes.Length > 10)
            {
                characters1 = namebytes.Take(10).ToArray();
                if (namebytes.Length > 22)
                {
                    characters2 = namebytes.Skip(10).Take(12).ToArray();

                    if (namebytes.Length > 26)
                    {
                        characters3 = namebytes.Skip(22).Take(4).ToArray();

                    }
                    else
                    {

                        int characters3len = namebytes.Length - 22;
                        Array.Copy(namebytes, 22, characters3, 0, characters3len);
                    }
                }
                else
                {
                    int characters2len = namebytes.Length - 10;
                    Array.Copy(namebytes, 10, characters2, 0, characters2len);
                }
            }
            else
            {
                Array.Copy(namebytes, 0, characters1, 0, namebytes.Length);
            }

           

            attributes = FatAttributes.LongFileName;
            entry_type = 0;

            checksum = lfn_entry_checksum(fileshort);
        }



        internal void WriteTo(Stream stream)
        {
            byte[] buffer = new byte[32];
            Array.Copy(characters1, 0, buffer, 1, characters1.Length);
            Array.Copy(characters2, 0, buffer, 14, characters2.Length);
            Array.Copy(characters3, 0, buffer, 0x1c, characters3.Length);
            buffer[11] =(byte)attributes;
            buffer[0] =(byte)entry_index;
            buffer[12] =(byte)entry_type;
            buffer[13] =(byte)checksum;
            EndianUtilities.WriteBytesLittleEndian(zero, buffer, 0x1a);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}