using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.IO.Compression;
using System.Security.Cryptography;

namespace PS4_BT_WIFI_PATCHER
{
    public partial class Form1 : Form
    {
        //not used
        #region patchMagic
        //using magic value at 0x144230 to identify firmware type
        static byte[] Patch_1 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x2D, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_2 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x2A, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_3 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x29, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_4 = new byte[16]
        {
            0xAC, 0x68, 0x00, 0x00, 0xB0, 0x68, 0x00, 0x00, 0xB4, 0x68, 0x00, 0x00, 0xB8, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_5 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x25, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_6 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x23, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_7 = new byte[16]
        {
            0x10, 0x82, 0x0E, 0x20, 0xCC, 0x68, 0x00, 0x00, 0x50, 0x68, 0x00, 0x00, 0x54, 0x68, 0x00, 0x00,
        };
        static byte[] Patch_8 = new byte[16]
        {
            0x5E, 0x5C, 0x55, 0xA7, 0x32, 0x4D, 0x55, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBC, 0x09, 0x7C, 0x53,
        };
        static byte[] Patch_9 = new byte[16]
        {
            0x05, 0xB8, 0x56, 0xB8, 0xBE, 0xAB, 0x56, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBD, 0x0B, 0x7C, 0x53,
        };
        static byte[] Patch_10 = new byte[16]
        {
            0xFA, 0x3F, 0x55, 0x8A, 0xFA, 0x3F, 0x55, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBC, 0x09, 0x7C, 0x13,
        };
        static byte[] Patch_11 = new byte[16]
        {
            0x31, 0x25, 0x59, 0x9F, 0x60, 0x1E, 0x59, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBD, 0x09, 0x5C, 0x53,
        };
        static byte[] Patch_12 = new byte[16]
        {
            0x4E, 0xDC, 0x58, 0xB5, 0x52, 0xCB, 0x58, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBD, 0x09, 0x7C, 0x13,
        };
        static byte[] Patch_13 = new byte[16]
        {
            0x32, 0x56, 0x57, 0xD0, 0x3E, 0x47, 0x57, 0xF5, 0x01, 0x14, 0x00, 0x84, 0xBD, 0x0B, 0x5C, 0x13,
        };
        byte[] FF_16 =  Enumerable.Repeat((byte)0xFF, 16).ToArray();

        #endregion patchMagic 

        static string bufferString, path;
        static byte[] bufferB, bufferC = new byte[0];
        static int flag = 0;

        public Form1()
        {
            InitializeComponent();
        }

        //function to check torus firmware
        private void CheckTorusFirmware (string str)
        {
            tbActualMD5.ForeColor = Color.Black;
            

            if (str == "s")
            {

            }
            else if (str == "o")
            {
                bufferString = tbLoadDump.Text;
            }

            //set path for current working dir
            string directoryPath = Environment.CurrentDirectory;

            //if C0020001.bin extracted, check the firmware tally with which patches
            if (File.Exists(directoryPath + @"\C0020001_extracted\C0020001.bin"))
            {
                //get the md5 hash of the extracted firmware
                txtMd5.Text = BytesToString(GetHashMD5(directoryPath + @"\C0020001_extracted\C0020001.bin"));

                //get the file size of the extracted firmware
                FileInfo f = new FileInfo(directoryPath + @"\C0020001_extracted\C0020001.bin");
                long fileSize = f.Length;
                tbSize.Text = fileSize.ToString("###,###") + " Bytes";

                //if the filesize OR md5 hash tally with patches, then verify the firmware wether o.k or k.o
                //use "||" operator so if the firmware hash md5 is mismatch, use the filesize to identify patches
                if (fileSize.ToString() == "453028" && txtMd5.Text == "42abb3b655f6085f029a408fe7e94831")
                {
                    //get hex value of identified patch 
                    bufferB = BT_WIFI.GetPatch_1(null);
                    //get hex value of extracted firmware
                    bufferC = BT_WIFI.GetOriginalValue1(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    //compare both hex value of patch and firmware
                    
                    if (Tool.CompareBytes(bufferB, bufferC) == true) // if both hex value is equal, firmware is valid
                    {
                        //show some info
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else //if both hex value is not equal, the firmware is not valid, enable patch button
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    //show some info
                    tbActualMD5.Text = "42abb3b655f6085f029a408fe7e94831";
                    tbPatchType.Text = "Patch 1";
                }
                //the checks goes on...
                else if (fileSize.ToString() == "452764" && txtMd5.Text == "92d4149f8c165abaf88a3ec93c491980")
                {

                    bufferB = BT_WIFI.GetPatch_2(null);
                    bufferC = BT_WIFI.GetOriginalValue2(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "92d4149f8c165abaf88a3ec93c491980";
                    tbPatchType.Text = "Patch 2";
                }
                else if (fileSize.ToString() == "452728" && txtMd5.Text == "86aede5276e8948b2ca6eed320e72266")
                {
                    bufferB = BT_WIFI.GetPatch_3(null);
                    bufferC = BT_WIFI.GetOriginalValue3(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "86aede5276e8948b2ca6eed320e72266";
                    tbPatchType.Text = "Patch 3";
                }
                else if (fileSize.ToString() == "451312" && txtMd5.Text == "571a67c2d0f64ab8cf1a65c201f0c60d")
                {
                    bufferB = BT_WIFI.GetPatch_4(null);
                    bufferC = BT_WIFI.GetOriginalValue4(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "571a67c2d0f64ab8cf1a65c201f0c60d";
                    tbPatchType.Text = "Patch 4";
                }
                else if (fileSize.ToString() == "450940" && txtMd5.Text == "c5dca09c92a2f0362d00bde4edb7548b")
                {
                    bufferB = BT_WIFI.GetPatch_5(null);
                    bufferC = BT_WIFI.GetOriginalValue5(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "c5dca09c92a2f0362d00bde4edb7548b";
                    tbPatchType.Text = "Patch 5";
                }
                else if (fileSize.ToString() == "450796" && txtMd5.Text == "13c18569ca45e3732fb17d9c14160081")
                {
                    bufferB = BT_WIFI.GetPatch_6(null);
                    bufferC = BT_WIFI.GetOriginalValue6(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "13c18569ca45e3732fb17d9c14160081";
                    tbPatchType.Text = "Patch 6";
                }
                else if (fileSize.ToString() == "449960" || txtMd5.Text == "d51c9935b5409313041177fb0393550b")
                {
                    bufferB = BT_WIFI.GetPatch_7(null);
                    bufferC = BT_WIFI.GetOriginalValue7(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        tbFWStatus.ForeColor = Color.Red;
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbPatchType.Text = "Patch 7";
                    tbActualMD5.Text = "d51c9935b5409313041177fb0393550b";
                }
                else if (fileSize.ToString() == "434871" && txtMd5.Text == "9efc56daf6c27ab00922baa38d49f8ab")
                {
                    bufferB = BT_WIFI.GetPatch_8(null);
                    bufferC = BT_WIFI.GetOriginalValue8(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbPatchType.Text = "Patch 8";
                    tbActualMD5.Text = "9efc56daf6c27ab00922baa38d49f8ab";
                }
                else if (fileSize.ToString() == "431614" && txtMd5.Text == "b658224645f34392019d21f1dee74889")
                {
                    bufferB = BT_WIFI.GetPatch_9(null);
                    bufferC = BT_WIFI.GetOriginalValue9(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "b658224645f34392019d21f1dee74889";
                    tbPatchType.Text = "Patch 9";
                }
                else if (fileSize.ToString() == "434685" && txtMd5.Text == "9976779772adfeddbf74b5a4f3047854")
                {
                    bufferB = BT_WIFI.GetPatch_10(null);
                    bufferC = BT_WIFI.GetOriginalValue10(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "9976779772adfeddbf74b5a4f3047854";
                    tbPatchType.Text = "Patch 10";
                }
                else if (fileSize.ToString() == "432158" && txtMd5.Text == "a601d993986c83d9db38c52a212d1c8b")
                {
                    bufferB = BT_WIFI.GetPatch_11(null);
                    bufferC = BT_WIFI.GetOriginalValue11(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "a601d993986c83d9db38c52a212d1c8b";
                    tbPatchType.Text = "Patch 11";
                }
                else if (fileSize.ToString() == "432033" && txtMd5.Text == "e77157f9c23dc6cbd8debfc24e32dd6d")
                {
                    bufferB = BT_WIFI.GetPatch_12(null);
                    bufferC = BT_WIFI.GetOriginalValue12(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "e77157f9c23dc6cbd8debfc24e32dd6d";
                    tbPatchType.Text = "Patch 12";
                }
                else if (fileSize.ToString() == "431685" && txtMd5.Text == "7aa816b366fce4adbec2b07b53e1482f")
                {
                    bufferB = BT_WIFI.GetPatch_13(null);
                    bufferC = BT_WIFI.GetOriginalValue13(directoryPath + @"\C0020001_extracted\C0020001.bin");
                    if (Tool.CompareBytes(bufferB, bufferC) == true)
                    {
                        txtMd5.ForeColor = Color.Green;
                        tbFWStatus.ForeColor = Color.Green;
                        tbFWStatus.Text = "OK";
                        button2.Enabled = false;
                    }
                    else
                    {
                        txtMd5.ForeColor = Color.Red;
                        tbFWStatus.ForeColor = Color.Red;
                        tbFWStatus.Text = "BAD";
                        button2.Enabled = true;
                    }
                    tbActualMD5.Text = "7aa816b366fce4adbec2b07b53e1482f";
                    tbPatchType.Text = "Patch 13";
                }
                else //if program could not detect the firmware
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = false;
                    tbPatchType.Text = "Could not detect BT_WIFI firmware";
                }

            }
            else //if no firmware extracted detected
            {
                MessageBox.Show("Could not detect BT_WIFI firmware");
                tbLoadDump.Text = "Select NOR dump";
                tbFWStatus.Text = "";
                tbPatchType.Text = "";
                txtMd5.Text = "";
                tbSize.Text = "";
                tbActualMD5.Text = "";
            }

            /* old code
             
            bufferA = BT_WIFI.GetPatch(tbLoadDump.Text);
            if (Tool.CompareBytes(bufferA, Patch_1) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_1(null);
                bufferC = BT_WIFI.GetOriginalValue1(tbLoadDump.Text);
                if (Tool.CompareBytes (bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 1";
            }
            else if (Tool.CompareBytes(bufferA, Patch_2) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_2(null);
                bufferC = BT_WIFI.GetOriginalValue2(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 2";
            }
            else if (Tool.CompareBytes(bufferA, Patch_3) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_3(null);
                bufferC = BT_WIFI.GetOriginalValue3(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 3";
            }
            else if (Tool.CompareBytes(bufferA, Patch_4) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_4(null);
                bufferC = BT_WIFI.GetOriginalValue4(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 4";
            }
            else if (Tool.CompareBytes(bufferA, Patch_5) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_5(null);
                bufferC = BT_WIFI.GetOriginalValue5(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 5";
            }
            else if (Tool.CompareBytes(bufferA, Patch_6) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_6(null);
                bufferC = BT_WIFI.GetOriginalValue6(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 6";
            }
            else if (Tool.CompareBytes(bufferA, Patch_7) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_7(null);
                bufferC = BT_WIFI.GetOriginalValue7(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 7";
            }
            else if (Tool.CompareBytes(bufferA, Patch_8) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_8(null);
                bufferC = BT_WIFI.GetOriginalValue8(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 8";
            }
            else if (Tool.CompareBytes(bufferA, Patch_9) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_9(null);
                bufferC = BT_WIFI.GetOriginalValue9(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 9";
            }
            else if (Tool.CompareBytes(bufferA, Patch_10) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_10(null);
                bufferC = BT_WIFI.GetOriginalValue10(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 10";
            }
            else if (Tool.CompareBytes(bufferA, Patch_11) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_11(null);
                bufferC = BT_WIFI.GetOriginalValue11(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 11";
            }
            else if (Tool.CompareBytes(bufferA, Patch_12) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_12(null);
                bufferC = BT_WIFI.GetOriginalValue12(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 12";
            }
            else if (Tool.CompareBytes(bufferA, Patch_13) == true && Tool.CompareBytes(bufferA, FF_16) == false)
            {
                bufferB = BT_WIFI.GetPatch_13(null);
                bufferC = BT_WIFI.GetOriginalValue13(tbLoadDump.Text);
                if (Tool.CompareBytes(bufferB, bufferC) == true)
                {
                    tbFWStatus.ForeColor = Color.Green;
                    tbFWStatus.Text = "OK";
                    button2.Enabled = false;
                }
                else
                {
                    tbFWStatus.ForeColor = Color.Red;
                    tbFWStatus.Text = "BAD";
                    button2.Enabled = true;
                }
                tbPatchType.Text = "Patch 13";
            }
            else
            {
                MessageBox.Show("Could not detected firmware type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbFWStatus.Text = "";
                tbSize.Text = "";
                tbPatchType.Text = "";
            }*/


        }
        
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //exit program tool strip
            Application.Exit();
        }

        
        private void aboutPS4TORUSPATCHERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //open up about form
            About About = new About();
            About.ShowDialog();
        }

        //method to extract embedded patches to the program
        private static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))

            using (BinaryReader r = new BinaryReader(s))

            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))

            using (BinaryWriter w = new BinaryWriter(fs))

            w.Write(r.ReadBytes((int)s.Length));

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //once form loaded, check if patches folder exist. 
            string path = Environment.CurrentDirectory;

            //delete if exist
            if (Directory.Exists(@"Patches"))
            {
                Directory.Delete(path + "\\Patches", true);
            }

            //once form loaded, extract the embedded patches
            Extract("PS4_BT_WIFI_PATCHER", path, "Resources", "Patches.zip");
            ZipFile.ExtractToDirectory(@"Patches.zip", path + "\\Patches");
            File.Delete("Patches.zip");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //when the program exist, delete patches dir
            string path = Environment.CurrentDirectory;
            Directory.Delete(path + "\\Patches", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //select file
            if ( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbLoadDump.Text = openFileDialog1.FileName;
                bufferString = tbLoadDump.Text.Replace(" ", "");
                //if file is valid ps4 dump
                if (BT_WIFI.CheckHeader(tbLoadDump.Text) == true)
                {
                    //extract firmware
                    ExtractDump("o");
                    //verify firmware
                    CheckTorusFirmware("o");
                }
                else//if file is not valid ps4 dump
                {
                    MessageBox.Show("Invalid file or corrupt flash dump.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbLoadDump.Text = "Select NOR dump";
                    tbFWStatus.Text = "";
                    tbPatchType.Text = "";
                    txtMd5.Text = "";
                    tbSize.Text = "";
                    tbActualMD5.Text = "";
                }
            }
        }

        //patch button
        private void button2_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(tbLoadDump.Text);
            //messagebox
            MessageBox.Show("Patching start from 0x0144200\n\nPatch size : " + tbSize.Text, "Patching Firmware", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if file not exist, create file
            if (!File.Exists(tbLoadDump.Text + ".BAK"))
            {
                File.Copy(tbLoadDump.Text, Path.Combine(path, Path.GetFileName(tbLoadDump.Text + ".BAK")), true);
            }
           
            //use binarywriter to patch the ps4 dump
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(tbLoadDump.Text));
            bw.Seek(0x144200, SeekOrigin.Begin);
            //bufferB is from BT_'WIFI.GetPatch_xx'
            bw.Write(bufferB);
            bw.Dispose();

            string name = new DirectoryInfo(tbLoadDump.Text).Name;
            //messagebox
            MessageBox.Show("Patching done.\nOriginal dump backed up as '" + name + ".BAK'.", "Firmware patched", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //once patching done, repeat the step to check newly patched dump/firmware is valid
            ExtractDump("o");
            CheckTorusFirmware("o");
        }

        //credit to cfwprophet for extracting dump method
        private void ExtractDump(string str)
        {
            if (str == "s")
            {

            }
            else if (str == "o")
            {
                bufferString = tbLoadDump.Text;
            }

            string directoryPath = Environment.CurrentDirectory;

            if (!Directory.Exists(directoryPath + @"\C0020001_extracted\") == true)
            {
                Directory.CreateDirectory(directoryPath + @"\C0020001_extracted\");

                if (!Directory.Exists(directoryPath + @"\C0020001_extracted\") == true)
                {
                    MessageBox.Show("An error occur. Please try again.");
                }
                else
                {
                    flag = 3;
                }
            }
            else if (Directory.Exists(directoryPath + @"\C0020001_extracted\"))
            {
                Directory.Delete(directoryPath + @"\C0020001_extracted\", true);

                if (Directory.Exists(directoryPath + @"\C0020001_extracted\") == true)
                {
                    MessageBox.Show("An error occur. Please try again.");
                }
                else
                {
                    Directory.CreateDirectory(directoryPath + @"\C0020001_extracted\");

                    if (!Directory.Exists(directoryPath + @"\C0020001_extracted\") == true)
                    {
                        MessageBox.Show("An error occur. Please try again.");
                    }
                    else
                    {
                        flag = 3;
                    }
                }
            }

            else
            {
            }

            if (flag == 3)
            {
                int exception = 0;

                try
                {
                    PS4Nor.ExtractDump(tbLoadDump.Text, directoryPath);
                }
                catch (Exception a)
                {
                    MessageBox.Show(a.ToString());
                    exception = 1;
                }
                finally
                {
                    if (exception != 1)
                    {
                        //once extracted, delete other useless file. we only need 'C0020001.bin'
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0000001_stage1.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0000001_stage2.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0008001_stage1.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0008001_stage2.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0010001.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0018001.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\C0028001.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\cid.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\CoreOS.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\eap_kbl.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\sceheader0.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\sceheader1.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\scevtrm0.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\scevtrm1.bin");
                        File.Delete(directoryPath + "\\C0020001_extracted\\Unk.bin");
                    }
                }
            }


            

        }

        //md5 checking method
        private MD5 md5 = MD5.Create();

        //md5 checking method
        private byte[] GetHashMD5(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return md5.ComputeHash(stream);
            }
        }

        //md5 checking method
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

    }
}
