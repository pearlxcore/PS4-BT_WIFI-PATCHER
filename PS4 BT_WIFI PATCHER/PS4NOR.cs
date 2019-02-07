using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PS4_BT_WIFI_PATCHER
{
    /// <summary>
    /// Public Class that handles the PS4 NOR Flash
    /// </summary>
    public class PS4Nor
    {
        #region FlashMagics
        static byte[] sceBigMagic = new byte[32]
        {
            0x53, 0x4F, 0x4E, 0x59, 0x20, 0x43, 0x4F, 0x4D, 0x50, 0x55, 0x54, 0x45, 0x52, 0x20, 0x45, 0x4E,
            0x54, 0x45, 0x52, 0x54, 0x41, 0x49, 0x4E, 0x4D, 0x45, 0x4E, 0x54, 0x20, 0x49, 0x4E, 0x43, 0x2E,
        };
        static byte[] sceSmallMagic = new byte[32]
        {
            0x53, 0x6F, 0x6E, 0x79, 0x20, 0x43, 0x6F, 0x6D, 0x70, 0x75, 0x74, 0x65, 0x72, 0x20, 0x45, 0x6E,
            0x74, 0x65, 0x72, 0x74, 0x61, 0x69, 0x6E, 0x6D, 0x65, 0x6E, 0x74, 0x20, 0x49, 0x6E, 0x63, 0x2E,
        };
        static byte[] dbcbMagic = new byte[24]
        {
             0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xBE, 0xBE, 0xDE, 0xAF, 0xBE, 0xEF, 0xCA, 0xFE, 0xBE, 0xBE,
             0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8,
        };
        static byte[] consoleConstant1 = new byte[50]
        {
             0xB9, 0x29, 0x00, 0x07, 0xFF, 0x07, 0x00, 0x07, 0xFF, 0x07, 0x00, 0x07, 0x0C, 0x04, 0x00, 0x00,
             0x00, 0x04, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
             0xFF, 0xFF,
        };
        static byte[] consoleConstant2 = new byte[48]
        {
             0x00, 0x07, 0xFF, 0x07, 0x00, 0x07, 0xFF, 0x07, 0x00, 0x07, 0x0C, 0x04, 0x00, 0x00, 0x00, 0x04,
             0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
             0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        /// <summary>
        /// Used from the GUI Code to see if the returned value is not only FF bytes
        /// </summary>
        public static byte[] ffCSerial = new byte[17]
        {
             0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
             0xFF,
        };

        static byte[] consoleConstant3 = new byte[9] { 0x34, 0x76, 0xB3, 0x80, 0x02, 0x00, 0x00, 0x00, 0x02, };
        static byte[] scevtrmMagic = new byte[7] { 0x53, 0x43, 0x45, 0x56, 0x54, 0x52, 0x4D, };
        static byte[] slb2Magic = new byte[5] { 0x53, 0x4C, 0x42, 0x32, 0x01, };
        static byte[] ffMagic = new byte[16] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };

        /// <summary>
        /// Used from the GUI Code to see if the returned value is not only FF bytes
        /// </summary>
        public static byte[] ffMAC = new byte[6] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        /// <summary>
        /// Used from the GUI Code to see if the returned value is not only FF bytes
        /// </summary>
        public static byte[] ffSKU = new byte[9] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        #endregion FlashMagics

        #region FlashOffsets
        static long macOffset = 0x1C4021;
        static long skuOffset = 0x1C8041;
        static long cidOffset = 0x1C8030;
        static long fwvOffset = 0x1CA604;
        static long dbcbOffset1 = 0x4218;
        static long dbcbOffset2 = 0x64218;
        static long dbcbOffset3 = 0xC4218;
        static long slb2Offset1 = 0x4000;
        static long slb2Offset2 = 0x64000;
        static long slb2Offset3 = 0xC4000;
        static long slb2Offset4 = 0x144000;

        static long[] slb2Offsets = new long[4] { slb2Offset1,
                                                  slb2Offset2,
                                                  slb2Offset3,
                                                  slb2Offset4 };

        static long scevtrmOffset1 = 0x380048;
        static long scevtrmOffset2 = 0x3A0048;
        static long sceOffset1 = 0x2000;
        static long sceOffset2 = 0x3000;
        static long blobOffset1 = 0x200000;
        static long blobOffset2 = 0x201000;
        static long blobOffset3 = 0x202000;
        static long blobOffset4 = 0x203000;
        static long blobOffset5 = 0x290800;
        static long blobOffset6 = 0x290A00;
        static long blobOffset7 = 0x290B00;
        static long blobOffset8 = 0x290C00;
        static long blobOffset9 = 0x3A2000;
        static long ccOffset1 = 0x1C4FFE;
        static long ccOffset2 = 0x1CE000;
        static long ccOffset3 = 0x1CA5D0;
        static long c01x1A = 0x4230;
        static long c01x1B = 0x64230;
        static long c01x2A = 0x4280;
        static long c01x2B = 0x64280;
        static long slb2ChecksumOffset3 = 0xC4250;
        #endregion FlashOffsets

        #region BufferBytes
        static byte[] mac = new byte[6];
        static byte[] fwv = new byte[4];
        static byte[] cid = new byte[17];
        static byte[] sku = new byte[14];
        static byte[] bufferA = new byte[0];
        static byte[] bufferB = new byte[0];
        static byte[] bufferC = new byte[0];
        static byte[] bufferD = new byte[0];
        static byte[] bufferE = new byte[0];
        #endregion BufferBytes

        #region Variables
        static long containerSize;
        static long containerCount;
        static int blockSize = 512;
        static long norSizePS4 = 33554432;

        static string[] flashFiles = new string[7] { "sceheader0.bin",
                                                     "sceheader1.bin",
                                                     "cid.bin",
                                                     "Unk.bin",
                                                     "scevtrm0.bin",
                                                     "scevtrm1.bin",
                                                     "CoreOS.bin"};
        #endregion Variables

        #region Boolians
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _sceBigMagic = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _sceSmallMagic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _sceSmallMagic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _dbcbMagic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _dbcbMagic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _dbcbMagic3 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _slb2Magic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _scevtrmMagic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic3 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic4 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic5 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic6 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic7 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic8 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _blobMagic9 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _ccMagic1 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _ccMagic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _ccMagic3 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _c01A = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _c01B = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _slb2Magic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _slb2Magic3 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _slb2Magic4 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _scevtrmMagic2 = false;
        /// <summary>
        /// Checked from the GUI Code to see which checks have passed and which not
        /// </summary>
        static public bool _slb2NR3Checksum = false;
        #endregion Boolians

        /// <summary>
        /// Check the Input Dump file for a few known values to wether indicate it is a PS4 NOR Dump or not and to see if the Dumped Data do match the known Console Constants
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        public static void CheckDumpFull(string dump)
        {
            #region CheckFlashRegionComplet
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                // Checking very first SCE Magic on offset 0x00
                bufferA = new byte[32];

                b.Read(bufferA, 0, 32);

                if (Tool.CompareBytes(bufferA, sceBigMagic) == true)
                {
                    _sceBigMagic = true;
                }

                Array.Clear(bufferA, 0, 32);

                // Checking second and third SCE Magic with normal letters
                b.BaseStream.Seek(sceOffset1, SeekOrigin.Begin);
                b.Read(bufferA, 0, 32);

                if (Tool.CompareBytes(bufferA, sceSmallMagic) == true)
                {
                    _sceSmallMagic1 = true;
                }

                Array.Clear(bufferA, 0, 32);

                b.BaseStream.Seek(sceOffset2, SeekOrigin.Begin);
                b.Read(bufferA, 0, 32);

                if (Tool.CompareBytes(bufferA, sceSmallMagic) == true)
                {
                    _sceSmallMagic2 = true;
                }

                // Checking for 4 SLB2 Headers within the Flash
                bufferA = new byte[5];
                bufferB = new byte[5];
                bufferC = new byte[5];
                bufferD = new byte[5];

                b.BaseStream.Seek(slb2Offset1, SeekOrigin.Begin);
                b.Read(bufferA, 0, 5);
                b.BaseStream.Seek(slb2Offset2, SeekOrigin.Begin);
                b.Read(bufferB, 0, 5);
                b.BaseStream.Seek(slb2Offset3, SeekOrigin.Begin);
                b.Read(bufferC, 0, 5);
                b.BaseStream.Seek(slb2Offset4, SeekOrigin.Begin);
                b.Read(bufferD, 0, 5);

                if (Tool.CompareBytes(bufferA, slb2Magic) == true)
                {
                    _slb2Magic1 = true;
                }
                if (Tool.CompareBytes(bufferB, slb2Magic) == true)
                {
                    _slb2Magic2 = true;
                }
                if (Tool.CompareBytes(bufferC, slb2Magic) == true)
                {
                    _slb2Magic3 = true;
                }
                if (Tool.CompareBytes(bufferD, slb2Magic) == true)
                {
                    _slb2Magic4 = true;
                }

                // Checking for the DeadBeef CafeBebe Magic on 3 known places
                bufferA = new byte[24];
                bufferB = new byte[24];
                bufferC = new byte[24];

                b.BaseStream.Seek(dbcbOffset1, SeekOrigin.Begin);
                b.Read(bufferA, 0, 24);
                b.BaseStream.Seek(dbcbOffset2, SeekOrigin.Begin);
                b.Read(bufferB, 0, 24);
                b.BaseStream.Seek(dbcbOffset3, SeekOrigin.Begin);
                b.Read(bufferC, 0, 24);

                if (Tool.CompareBytes(bufferA, dbcbMagic) == true)
                {
                    _dbcbMagic1 = true;
                }
                if (Tool.CompareBytes(bufferB, dbcbMagic) == true)
                {
                    _dbcbMagic2 = true;
                }
                if (Tool.CompareBytes(bufferC, dbcbMagic) == true)
                {
                    _dbcbMagic3 = true;
                }

                // Checking if MediaCon FW Stage 1 & Stage 2 do match with 2x byte[32]
                bufferA = new byte[32];
                bufferB = new byte[32];
                bufferC = new byte[32];
                bufferD = new byte[32];

                b.BaseStream.Seek(c01x1A, SeekOrigin.Begin);
                b.Read(bufferA, 0, 32);
                b.BaseStream.Seek(c01x2A, SeekOrigin.Begin);
                b.Read(bufferB, 0, 32);
                b.BaseStream.Seek(c01x1B, SeekOrigin.Begin);
                b.Read(bufferC, 0, 32);
                b.BaseStream.Seek(c01x2B, SeekOrigin.Begin);
                b.Read(bufferD, 0, 32);

                if (Tool.CompareBytes(bufferA, bufferC) == true)
                {
                    _c01A = true;
                }
                if (Tool.CompareBytes(bufferB, bufferD) == true)
                {
                    _c01B = true;
                }

                #region CheckUnknownFlashRegion
                // Checking (till now) Unk File Section for first 4 static hex blob Offsets with each of 512 bytes
                // additional we check the first 16 byte Before and After the hex blob if they are FF bytes otherwise we won't check the known blob
                // After that we do the same with the other 4 static hex blob Offsets
                bufferA = new byte[16];
                bufferB = new byte[512];
                bufferC = new byte[16];
                bufferD = new byte[512];

                b.BaseStream.Seek(0x1FFFF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset1, SeekOrigin.Begin);
                b.Read(bufferB, 0, 512);
                b.BaseStream.Seek(0x200200, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic1 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                Array.Clear(bufferB, 0, 512);
                Array.Clear(bufferC, 0, 16);

                b.BaseStream.Seek(0x200FF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset2, SeekOrigin.Begin);
                b.Read(bufferB, 0, 512);
                b.BaseStream.Seek(0x201200, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic2 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                Array.Clear(bufferB, 0, 512);
                Array.Clear(bufferC, 0, 16);

                b.BaseStream.Seek(0x201FF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset3, SeekOrigin.Begin);
                b.Read(bufferB, 0, 512);
                b.BaseStream.Seek(0x202200, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic3 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                Array.Clear(bufferB, 0, 512);
                Array.Clear(bufferC, 0, 16);

                b.BaseStream.Seek(0x202FF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset4, SeekOrigin.Begin);
                b.Read(bufferB, 0, 512);
                b.BaseStream.Seek(0x203200, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic4 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                bufferB = new byte[304];
                Array.Clear(bufferC, 0, 16);
                bufferD = new byte[304];

                b.BaseStream.Seek(0x2907F0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset5, SeekOrigin.Begin);
                b.Read(bufferB, 0, 304);
                b.BaseStream.Seek(0x290930, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic5 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                bufferB = new byte[224];
                Array.Clear(bufferC, 0, 16);
                bufferD = new byte[224];

                b.BaseStream.Seek(0x2909F0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset6, SeekOrigin.Begin);
                b.Read(bufferB, 0, 224);
                b.BaseStream.Seek(0x290AE0, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic6 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                bufferB = new byte[352];
                Array.Clear(bufferC, 0, 16);
                bufferD = new byte[352];

                b.BaseStream.Seek(0x290BF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset7, SeekOrigin.Begin);
                b.Read(bufferB, 0, 352);
                b.BaseStream.Seek(0x290D60, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic7 = true;
                    }
                }

                Array.Clear(bufferA, 0, 16);
                bufferB = new byte[64];
                Array.Clear(bufferC, 0, 16);
                bufferD = new byte[64];

                b.BaseStream.Seek(0x290DF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset8, SeekOrigin.Begin);
                b.Read(bufferB, 0, 64);
                b.BaseStream.Seek(0x290E40, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true)
                {
                    if (Tool.CompareBytes(bufferB, bufferD) == false)
                    {
                        _blobMagic8 = true;
                    }
                }
                #endregion CheckUnknownFlashRegion

                // Checking for SCEVTRM Magic on 2 known places
                bufferA = new byte[7];
                bufferB = new byte[7];

                b.BaseStream.Seek(scevtrmOffset1, SeekOrigin.Begin);
                b.Read(bufferA, 0, 7);
                b.BaseStream.Seek(scevtrmOffset2, SeekOrigin.Begin);
                b.Read(bufferB, 0, 7);

                if (Tool.CompareBytes(bufferA, scevtrmMagic) == true)
                {
                    _scevtrmMagic1 = true;
                }
                if (Tool.CompareBytes(bufferB, scevtrmMagic) == true)
                {
                    _scevtrmMagic2 = true;
                }

                // checking for 3 known Console Constants, one in SCEVTRM and two in Unk File
                bufferA = new byte[50];
                bufferB = new byte[48];
                bufferC = new byte[9];

                b.BaseStream.Seek(ccOffset1, SeekOrigin.Begin);
                b.Read(bufferA, 0, 50);
                b.BaseStream.Seek(ccOffset2, SeekOrigin.Begin);
                b.Read(bufferB, 0, 48);
                b.BaseStream.Seek(ccOffset3, SeekOrigin.Begin);
                b.Read(bufferC, 0, 9);

                if (Tool.CompareBytes(bufferA, consoleConstant1) == true)
                {
                    _ccMagic1 = true;
                }
                if (Tool.CompareBytes(bufferB, consoleConstant2) == true)
                {
                    _ccMagic2 = true;
                }
                if (Tool.CompareBytes(bufferC, consoleConstant3) == true)
                {
                    _ccMagic3 = true;
                }

                // Checking for second known Console Specific Constant in SCEVTRM
                bufferA = new byte[16];
                bufferB = new byte[32];
                bufferC = new byte[16];
                bufferD = new byte[0];
                bufferE = new byte[32];

                b.BaseStream.Seek(0x3A1FF0, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);
                b.BaseStream.Seek(blobOffset9, SeekOrigin.Begin);
                b.Read(bufferB, 0, 32);
                b.BaseStream.Seek(0x3A2020, SeekOrigin.Begin);
                b.Read(bufferC, 0, 16);
                bufferD = ffMagic.Concat(ffMagic).ToArray();

                if (Tool.CompareBytes(bufferA, ffMagic) == true && Tool.CompareBytes(bufferC, ffMagic) == true && Tool.CompareBytes(bufferB, bufferD) == false && Tool.CompareBytes(bufferB, bufferE) == false)
                {
                    _blobMagic9 = true;
                }

                // Checking 48 byte long Checksum Region of SLB2 #3
                bufferA = new byte[48];
                bufferB = new byte[0];
                bufferC = new byte[48];

                b.BaseStream.Seek(slb2ChecksumOffset3, SeekOrigin.Begin);
                b.Read(bufferA, 0, 48);
                bufferB = ffMagic.Concat(ffMagic).Concat(ffMagic).ToArray();

                if (Tool.CompareBytes(bufferA, bufferC) == false && Tool.CompareBytes(bufferA, bufferB) == false)
                {
                    _slb2NR3Checksum = true;
                }

                b.Close();
            }
            #endregion CheckFlashRegionComplet
        }

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
        /// Check the Size of the Input Dump
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>True if the Size of the Input Dump do match the knowen Length</returns>
        public static bool CheckSize(string dump)
        {
            FileInfo fileInfo = new FileInfo(dump);
            long dumpLength = fileInfo.Length;
            if (dumpLength == norSizePS4)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get MAC Address
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>The MAC address of the Console</returns>
        public static byte[] GetMAC(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(macOffset, SeekOrigin.Begin);
                b.Read(mac, 0, 6);
                return mac;
            }
        }

        /// <summary>
        /// Get Console ID
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>The Console Serial</returns>
        public static byte[] GetCID(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(cidOffset, SeekOrigin.Begin);
                b.Read(cid, 0, 17);
                return cid;
            }
        }

        /// <summary>
        /// Get SKU Model
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>The SKU Version</returns>
        public static byte[] GetSKU(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                b.BaseStream.Seek(skuOffset, SeekOrigin.Begin);
                b.Read(sku, 0, 14);
                return sku;
            }
        }

        /// <summary>
        /// Get FirmWare Version
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <returns>The FirmWare Version</returns>
        public static byte[] GetFWV(string dump)
        {
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open)))
            {
                bufferA = new byte[4];
                b.BaseStream.Seek(fwvOffset, SeekOrigin.Begin);
                b.Read(fwv, 0, 4);
                Array.Reverse(fwv);
                return fwv;
            }
        }

        /// <summary>
        /// Extract all 4 SLB2 Container from a PS4 NOR Flash Dump and write them to a file with a .SLB2 extension
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <param name="path">The path where to extract the files</param>
        private static void ExtractSLB2(string dump, string path)
        {
            int x = 0;
            while (x != 4)
            {
                File.Create(path + @"\C0020001_extracted\" + "slb" + x + ".slb2").Close();
                x++;
            }

            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open, FileAccess.Read)))
            {
                x = 0;
                while (x != 4)
                {
                    bufferA = new byte[4];

                    b.BaseStream.Seek((slb2Offsets[x] + 0x10), SeekOrigin.Begin);
                    b.Read(bufferA, 0, 4);
                    containerCount = Tool.HexToDec(bufferA, "reverse");
                    containerSize = (containerCount * blockSize);
                    bufferB = new byte[containerSize];
                    b.BaseStream.Seek(slb2Offsets[x], SeekOrigin.Begin);
                    b.Read(bufferB, 0, bufferB.Length);
                    Tool.ReadWriteData(path + @"\C0020001_extracted\" + "slb" + x + ".slb2", null, "w", "bi", bufferB);
                    x++;
                }

                b.Close();
            }
        }

        /// <summary>
        /// Extract a PS4 NOR Dump File
        /// </summary>
        /// <param name="dump">The Input Dump File</param>
        /// <param name="path">The path to where the files will be extracted</param>
        public static void ExtractDump(string dump, string path)
        {
            // Extract SLB2's from Dump
            ExtractSLB2(dump, path);

            // Extract the SLB2 Container previous dumped from Flash
            int y = 0;
            while (y != 4)
            {
                // Is SLB2 Header valied?
                if (!SLB2.CheckHeader(path + @"\C0020001_extracted\" + "slb" + y + ".slb2"))
                {
                    MessageBox.Show("SLB2 Header of Flash Internal SLB2: " + (path + @"\C0020001_extracted\" + "slb" + y + ".slb2") + " is corrupted");
                }

                // Read SLB2 Header Info
                SLB2.Read(path + @"\C0020001_extracted\" + "slb" + y + ".slb2");

                // Is the Size of the SLB2 Container valied?
                if (!SLB2.CheckSize())
                {
                    MessageBox.Show("The Size of Flash Internal SLB2: " + (path + @"\C0020001_extracted\" + "slb" + y + ".slb2") + " is corrupted");
                }

                // Finally Extract the SLB2 container now
                SLB2.Extract(path + @"\C0020001_extracted\" + "slb" + y + ".slb2", path + @"\C0020001_extracted\");

                // Cleaning up the work File
                File.Delete(path + @"\C0020001_extracted\" + "slb" + y + ".slb2");
                y++;
            }

            int z = 0;
            while (z != 7)
            {
                // Create the Flash Files to fill them up
                File.Create(path + @"\C0020001_extracted\" + flashFiles[z]).Close();
                z++;
            }

            // Read and Write the other Flash Files, fill up the pre-created Flash Files
            using (BinaryReader b = new BinaryReader(new FileStream(dump, FileMode.Open, FileAccess.Read)))
            {
                // Dumping the 2 SCE Header
                bufferA = new byte[0xB0];

                b.BaseStream.Seek(0x2000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0xB0);
                Tool.ReadWriteData(path + @"\C0020001_extracted\sceheader0.bin", null, "w", "bi", bufferA);

                b.BaseStream.Seek(0x3000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0xB0);
                Tool.ReadWriteData(path + @"\C0020001_extracted\sceheader1.bin", null, "w", "bi", bufferA);

                // Dumping Main Console Information
                bufferA = new byte[0x6000];

                b.BaseStream.Seek(0x1C4000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0x6000);
                Tool.ReadWriteData(path + @"\C0020001_extracted\cid.bin", null, "w", "bi", bufferA);

                // Dumping Unknown File
                bufferA = new byte[0x1B6000];

                b.BaseStream.Seek(0x1CA000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0x1B6000);
                Tool.ReadWriteData(path + @"\C0020001_extracted\Unk.bin", null, "w", "bi", bufferA);

                // Dumping scevtrm in two parts (0, 1)
                bufferA = new byte[0x20000];

                b.BaseStream.Seek(0x380000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0x20000);
                Tool.ReadWriteData(path + @"\C0020001_extracted\scevtrm0.bin", null, "w", "bi", bufferA);

                b.BaseStream.Seek(0x3A0000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 0x20000);
                Tool.ReadWriteData(path + @"\C0020001_extracted\scevtrm1.bin", null, "w", "bi", bufferA);

                // Dumping CoreOS
                long byteCounter = 0;
                bufferA = new byte[16];
                bufferB = new byte[16] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, };
                bufferC = new byte[31500000];

                b.BaseStream.Seek(0x3C0000, SeekOrigin.Begin);
                b.Read(bufferA, 0, 16);

                // Do we read only FF bytes?
                while (!Tool.CompareBytes(bufferA, bufferB))
                {
                    // We Copy over the readed 16 bytes that are not FF bytes
                    Array.Copy(bufferA, 0, bufferC, byteCounter, 16);

                    // The byteCounter is used to get the real size of the Dumped Data so we add 16 to it cause we have written 16 bytes to our Array
                    byteCounter += 16;

                    // Read the next 16 bytes before the loop start again
                    b.Read(bufferA, 0, 16);
                }

                // We shrink the Array. To do that we set up a new byte Array in size of the Dumped Data and Copy over our readed bytes. 
                byte[] result = new byte[byteCounter];
                Array.Copy(bufferC, result, byteCounter);
                Tool.ReadWriteData(path + @"\C0020001_extracted\CoreOS.bin", null, "w", "bi", result);

                b.Close();
            }
        }
    }
}
