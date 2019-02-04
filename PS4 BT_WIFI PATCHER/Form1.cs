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

namespace PS4_BT_WIFI_PATCHER
{
    public partial class Form1 : Form
    {
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
        byte[] FF_16 = Enumerable.Repeat((byte)0xFF, 16).ToArray();

        #endregion patchMagic

        static string bufferString;
        byte[] dump;
        static byte[] bufferA, bufferB, bufferC = new byte[0];

        public Form1()
        {
            InitializeComponent();
        }

        private void CheckTorusFirmware (string str)
        { 
            if (str == "s")
            {

            }
            else if (str == "o")
            {
                bufferString = tbLoadDump.Text;
            }

            //main process started
            //get dump's torus firmware type
            //compare with known patches
            //enable patch button if firmware is invalid
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
                tbSize.Text = "453,028 bytes";
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
                tbSize.Text = "452,764 bytes";
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
                tbSize.Text = "452,728 bytes";
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
                tbSize.Text = "451,312 bytes";
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
                tbSize.Text = "450,940 bytes";
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
                tbSize.Text = "450,796 bytes";
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
                tbSize.Text = "449,960 bytes";
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
                tbSize.Text = "434,871 bytes";
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
                tbSize.Text = "431,614 bytes";
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
                tbSize.Text = "434,685 bytes";
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
                tbSize.Text = "432,158 bytes";
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
                tbSize.Text = "432,033 bytes";
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
                tbSize.Text = "431,685 bytes";
                tbPatchType.Text = "Patch 13";
            }
            else
            {
                MessageBox.Show("Could not detected firmware type", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tbFWStatus.Text = "";
                tbSize.Text = "";
                tbPatchType.Text = "";
            }


        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutPS4TORUSPATCHERToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

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
            string path = Environment.CurrentDirectory;

            Extract("PS4_BT_WIFI_PATCHER", path, "Resources", "Patches.zip");
            ZipFile.ExtractToDirectory(@"Patches.zip", path + "\\Patches");
            File.Delete("Patches.zip");

            /*if (!Directory.Exists(@"Patches"))
            {
                MessageBox.Show("Please included 'Patches' folder!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }*/
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            string path = Environment.CurrentDirectory;
            Directory.Delete(path + "\\Patches", true);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbLoadDump.Text = openFileDialog1.FileName;
                bufferString = tbLoadDump.Text.Replace(" ", "");
                if (BT_WIFI.CheckHeader(tbLoadDump.Text) == true)
                {
                    CheckTorusFirmware("o");
                }
                else
                {
                    MessageBox.Show("Invalid file or corrupt flash dump.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    tbLoadDump.Text = "Select NOR dump";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(tbLoadDump.Text);
            MessageBox.Show("Patching start from 0x0144200\nPatch size : " + tbSize.Text, "Patching Firmware", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (File.Exists(tbLoadDump.Text + ".BAK"))
            {
                File.Copy(tbLoadDump.Text, Path.Combine(path, Path.GetFileName(tbLoadDump.Text + ".BAK")), true);

            }
           
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(tbLoadDump.Text));
            bw.Seek(0x144200, SeekOrigin.Begin);
            bw.Write(bufferB);
            bw.Dispose();

            string name = new DirectoryInfo(tbLoadDump.Text).Name;
            MessageBox.Show("Patching done.\nOriginal dump backed up as '" + name + ".BAK'.", "Firmware patched", MessageBoxButtons.OK, MessageBoxIcon.Information);
            CheckTorusFirmware("o");
        }
    }
}
