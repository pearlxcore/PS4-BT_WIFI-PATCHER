using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PS4_BT_WIFI_PATCHER
{
    /// <summary>
    /// Public Class that handles the PS4 NOR Flash
    /// </summary>
    public class BT_WIFI
    {
        #region Magics
        static byte[] sceBigMagic = new byte[32]
        {
            0x53, 0x4F, 0x4E, 0x59, 0x20, 0x43, 0x4F, 0x4D, 0x50, 0x55, 0x54, 0x45, 0x52, 0x20, 0x45, 0x4E,
            0x54, 0x45, 0x52, 0x54, 0x41, 0x49, 0x4E, 0x4D, 0x45, 0x4E, 0x54, 0x20, 0x49, 0x4E, 0x43, 0x2E,
        };
        #endregion Magics

        #region BufferBytes
        static byte[] bufferPatch = new byte[16];
        static byte[] bufferA = new byte[0];
        static byte[] bufferPatch_1 = new byte[453028];
        static byte[] bufferPatch_2 = new byte[452764];
        static byte[] bufferPatch_3 = new byte[452728];
        static byte[] bufferPatch_4 = new byte[451312];
        static byte[] bufferPatch_5 = new byte[450940];
        static byte[] bufferPatch_6 = new byte[450796];
        static byte[] bufferPatch_7 = new byte[449960];
        static byte[] bufferPatch_8 = new byte[434871];
        static byte[] bufferPatch_9 = new byte[431614];
        static byte[] bufferPatch_10 = new byte[434685];
        static byte[] bufferPatch_11 = new byte[432158];
        static byte[] bufferPatch_12 = new byte[432033];
        static byte[] bufferPatch_13 = new byte[431685];
        static byte[] bufferValue_1 = new byte[453028];
        static byte[] bufferValue_2 = new byte[452764];
        static byte[] bufferValue_3 = new byte[452728];
        static byte[] bufferValue_4 = new byte[451312];
        static byte[] bufferValue_5 = new byte[450940];
        static byte[] bufferValue_6 = new byte[450796];
        static byte[] bufferValue_7 = new byte[449960];
        static byte[] bufferValue_8 = new byte[434871];
        static byte[] bufferValue_9 = new byte[431614];
        static byte[] bufferValue_10 = new byte[434685];
        static byte[] bufferValue_11 = new byte[432158];
        static byte[] bufferValue_12 = new byte[432033];
        static byte[] bufferValue_13 = new byte[431685];
        #endregion BufferBytes

        /// <summary>
        /// Quick -Check the Header of the Input Dump file if it is a valied PS4 Dump file
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>True if the Header of the input Dump do Match the SCE Header</returns>
        public static bool CheckHeader(string dump)
        {
            bufferA = new byte[32];
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.Read(bufferA, 0, 32);
                if (Tool.CompareBytes(bufferA, sceBigMagic) == true)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get MAC Address
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>The MAC address of the Console</returns>
        public static byte[] GetPatch(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144230, SeekOrigin.Begin);
                b.Read(bufferPatch, 0, 16);
                return bufferPatch;
            }
        }

        public static byte[] GetPatch_1(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_1.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_1, 0, 453028);
                return bufferPatch_1;
            }
        }

        public static byte[] GetPatch_2(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_2.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_2, 0, 452764);
                return bufferPatch_2;
            }
        }

        public static byte[] GetPatch_3(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_3.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_3, 0, 452728);
                return bufferPatch_3;
            }
        }

        public static byte[] GetPatch_4(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_4.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_4, 0, 451312);
                return bufferPatch_4;
            }
        }

        public static byte[] GetPatch_5(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_5.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_5, 0, 450940);
                return bufferPatch_5;
            }
        }

        public static byte[] GetPatch_6(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_6.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_6, 0, 450796);
                return bufferPatch_6;
            }
        }

        public static byte[] GetPatch_7(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_7.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_7, 0, 449960);
                return bufferPatch_7;
            }
        }

        public static byte[] GetPatch_8(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_8.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_8, 0, 434871);
                return bufferPatch_8;
            }
        }

        public static byte[] GetPatch_9(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_9.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_9, 0, 431614);
                return bufferPatch_9;
            }
        }

        public static byte[] GetPatch_10(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_10.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_10, 0, 434685);
                return bufferPatch_10;
            }
        }

        public static byte[] GetPatch_11(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_11.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_11, 0, 432158);
                return bufferPatch_11;
            }
        }

        public static byte[] GetPatch_12(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_12.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_12, 0, 432033);
                return bufferPatch_12;
            }
        }

        public static byte[] GetPatch_13(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(@"Patches\C0020001_13.bin", FileMode.Open)))
            {
                b.BaseStream.Seek(0x000000, SeekOrigin.Begin);
                b.Read(bufferPatch_13, 0, 431685);
                return bufferPatch_13;
            }
        }

        public static byte[] GetOriginalValue1(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_1, 0, 453028);
                return bufferValue_1;
            }
        }

        public static byte[] GetOriginalValue2(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_2, 0, 452764);
                return bufferValue_2;
            }
        }

        public static byte[] GetOriginalValue3(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_3, 0, 452728);
                return bufferValue_3;
            }
        }

        public static byte[] GetOriginalValue4(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_4, 0, 451312);
                return bufferValue_4;
            }
        }

        public static byte[] GetOriginalValue5(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_5, 0, 450940);
                return bufferValue_5;
            }
        }

        public static byte[] GetOriginalValue6(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_6, 0, 450796);
                return bufferValue_6;
            }
        }

        public static byte[] GetOriginalValue7(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_7, 0, 449960);
                return bufferValue_7;
            }
        }

        public static byte[] GetOriginalValue8(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_8, 0, 434871);
                return bufferValue_8;
            }
        }

        public static byte[] GetOriginalValue9(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_9, 0, 431614);
                return bufferValue_9;
            }
        }

        public static byte[] GetOriginalValue10(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_10, 0, 434685);
                return bufferValue_10;
            }
        }

        public static byte[] GetOriginalValue11(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_11, 0, 432158);
                return bufferValue_11;
            }
        }

        public static byte[] GetOriginalValue12(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_12, 0, 432033);
                return bufferValue_12;
            }
        }

        public static byte[] GetOriginalValue13(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(0x0144200, SeekOrigin.Begin);
                b.Read(bufferValue_13, 0, 431685);
                return bufferValue_13;
            }
        }
    }
}
