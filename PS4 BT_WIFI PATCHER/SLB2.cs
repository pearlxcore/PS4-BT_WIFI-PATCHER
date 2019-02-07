using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS4_BT_WIFI_PATCHER
{
    /// <summary>
    /// SLB2 Class to handle the PS4'S SLB2 container
    /// </summary>
    public class SLB2
    {
        #region Variables
        static long slb2BaseOffset = 0x200;
        static long containerSize;
        static long slb2Version;
        static long fileCount;
        static int blockSize = 512;
        static long containerCount;
        static int blockCountOffset = 0x20;
        static int byteCountOffset = 0x24;
        static int fileNameOffset = 0x30;
        static long byteCount;
        static long blockCount;
        static string fileName;
        #endregion Variables

        /// <summary>
        /// Reset the major Variables for the next SLB2 Container
        /// </summary>
        private static void ResetVars()
        {
            slb2BaseOffset = 0x200;
            blockCountOffset = 0x20;
            byteCountOffset = 0x24;
            fileNameOffset = 0x30;
        }

        /// <summary>
        /// Check SLB2 Magic of a SLB2 Container
        /// </summary>
        /// <param name="slb2">The SLB2 Container to check</param>
        /// <returns>True if the File Magic do match the SLB2 Magic</returns>
        public static bool CheckHeader(string slb2)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open)))
            {
                byte[] buffer = new byte[4];
                byte[] slb2Magic = new byte[5] { 0x53, 0x4C, 0x42, 0x32, 0x01, };
                b.Read(buffer, 0, 4);
                if (Tool.CompareBytes(buffer, slb2Magic))
                {
                    return true;
                }
                b.Close();
            }
            return false;
        }

        /// <summary>
        /// Read the TOC of the SLB2 Container
        /// </summary>
        /// <param name="slb2">The SLB2 Container to read the TOC from</param>
        public static void Read(string slb2)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open)))
            {
                byte[] bufferA = new byte[4];
                byte[] bufferB = new byte[4];
                byte[] bufferC = new byte[4];
                slb2Version = 0;
                fileCount = 0;
                containerCount = 0;
                containerSize = 0;

                FileInfo fileInfo = new FileInfo(slb2);
                containerSize = fileInfo.Length;

                b.BaseStream.Seek(0x04, SeekOrigin.Begin);
                b.Read(bufferA, 0, 4);
                slb2Version = Tool.HexToDec(bufferA, "reverse");

                b.BaseStream.Seek(0x0C, SeekOrigin.Begin);
                b.Read(bufferB, 0, 4);
                fileCount = Tool.HexToDec(bufferB, "reverse");

                b.BaseStream.Seek(0x10, SeekOrigin.Begin);
                b.Read(bufferC, 0, 4);
                containerCount = Tool.HexToDec(bufferC, "reverse");

                b.Close();
            }
        }

        /// <summary>
        /// Get the Version of the SLB2 Container (Need's Read() to be called once before)
        /// </summary>
        /// <returns>The version of the SLB2 Container</returns>
        public static long GetVersion()
        {
            return slb2Version;
        }

        /// <summary>
        /// Get File Count of SLB2 Container (Need's Read() to be called once before)
        /// </summary>
        /// <returns>File Count of the input SLB2 Container</returns>
        public static long GetFileCount()
        {
            return fileCount;
        }

        /// <summary>
        /// Check the Size of the input SLB2 Container against the saved size in the header (Need's Read() to be called once before)
        /// </summary>
        /// <returns>True if the saved size in header do match the reall file size</returns>
        public static bool CheckSize()
        {
            if ((containerCount * blockSize) == containerSize)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Extract a SLB2 Container
        /// </summary>
        /// <param name="slb2">The SLB2 Container to use</param>
        /// <param name="path">The path where to save the extracted files</param>
        public static void Extract(string slb2, string path)
        {
            int flag = 0;
            ASCIIEncoding enc = new ASCIIEncoding();

            while (fileCount != 0)
            {
                byte[] bufferA = new byte[4];
                byte[] bufferB = new byte[4];
                byte[] bufferC = new byte[16];

                using (BinaryReader b = new BinaryReader(new FileStream(slb2, FileMode.Open, FileAccess.Read)))
                {
                    b.BaseStream.Seek(blockCountOffset, SeekOrigin.Begin);
                    b.Read(bufferA, 0, 4);
                    b.BaseStream.Seek(byteCountOffset, SeekOrigin.Begin);
                    b.Read(bufferB, 0, 4);
                    b.BaseStream.Seek(fileNameOffset, SeekOrigin.Begin);
                    b.Read(bufferC, 0, 16);

                    b.Close();
                }

                blockCount = Tool.HexToDec(bufferA, "reverse");
                byteCount = Tool.HexToDec(bufferB, "reverse");

                for (int i = 15; i > 0; i--)
                {
                    if (bufferC[i] != 00)
                    {
                        byte[] newByte = new byte[i + 1];
                        Array.Copy(bufferC, 0, newByte, 0, i + 1);
                        fileName = enc.GetString(newByte);
                        break;
                    }
                }

                if (blockCount > 1)
                {
                    slb2BaseOffset = (blockCount * blockSize);
                }

                if (fileName == "C0000001")
                {
                    if (!File.Exists(path + fileName + "_stage1.bin") == true)
                    {
                        File.Create(path + fileName + "_stage1.bin").Close();
                        flag = 1;
                    }
                    else
                    {
                        File.Create(path + fileName + "_stage2.bin").Close();
                        flag = 2;
                    }
                }
                else if (fileName == "C0008001")
                {
                    if (!File.Exists(path + fileName + "_stage1.bin") == true)
                    {
                        File.Create(path + fileName + "_stage1.bin").Close();
                        flag = 1;
                    }
                    else
                    {
                        File.Create(path + fileName + "_stage2.bin").Close();
                        flag = 2;
                    }
                }
                else if (fileName == "C0010001" ||
                         fileName == "eap_kbl" ||
                         fileName == "C0018001" ||
                         fileName == "C0020001" ||
                         fileName == "C0028001")
                {
                    File.Create(path + fileName + ".bin").Close();
                    flag = 3;
                }
                else
                {
                    File.Create(path + fileName).Close();
                }

                if (flag == 1)
                {
                    Tool.ReadWriteData(slb2, (path + fileName + "_stage1.bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                }
                else if (flag == 2)
                {
                    Tool.ReadWriteData(slb2, (path + fileName + "_stage2.bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                }
                else if (flag == 3)
                {
                    Tool.ReadWriteData(slb2, (path + fileName + ".bin"), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                }
                else
                {
                    Tool.ReadWriteData(slb2, (path + fileName), "b", "bi", null, 0, slb2BaseOffset, byteCount);
                }

                flag = 0;
                fileCount -= 1;
                blockCountOffset += 0x30;
                byteCountOffset += 0x30;
                fileNameOffset += 0x30;
            }

            // Reseting the main vars to the standart sart value
            ResetVars();
        }
    }
}
