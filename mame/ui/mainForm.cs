using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using mame;

namespace ui
{
    public partial class mainForm : Form
    {
        private ToolStripMenuItem[] itemSize;
        public string sSelect;
        public static Thread t1;
        public string handle1;
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern int ShowCursor(bool bShow);
        internal static void ShowCursor()
        {
            while (ShowCursor(true) < 0)
            {
                ShowCursor(true);
            }
        }
        internal static void HideCursor()
        {
            while (ShowCursor(false) >= 0)
            {
                ShowCursor(false);
            }
        }
        public mainForm()
        {
            InitializeComponent();
        }

        public void LoadRom()
        {


            this.Close();
            Mame.exit_pending = true;
            Thread.Sleep(100);

            mame.Timer.lt = new List<mame.Timer.emu_timer>();
            sSelect = RomInfo.Rom.Name;
            Machine.FORM = this;
            Machine.rom = RomInfo.Rom;
            Machine.sName = Machine.rom.Name;
            Machine.sParent = Machine.rom.Parent;
            Machine.sBoard = Machine.rom.Board;
            Machine.sDirection = Machine.rom.Direction;
            Machine.sDescription = Machine.rom.Description;
            Machine.sManufacturer = Machine.rom.Manufacturer;
            Machine.bRom = true;
            Machine.lsParents = new List<String>();
            Machine.lsParents.Add("ffight");
            int i;
            cpsToolStripMenuItem.Enabled = false;
            neogeoToolStripMenuItem.Enabled = false;
            namcos1ToolStripMenuItem.Enabled = false;
            pgmToolStripMenuItem.Enabled = false;
            m72ToolStripMenuItem.Enabled = false;
            m92ToolStripMenuItem.Enabled = false;
            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                case "CPS2":
                    Video.nMode = 3;
                    itemSize = new ToolStripMenuItem[Video.nMode];
                    for (i = 0; i < Video.nMode; i++)
                    {
                        itemSize[i] = new ToolStripMenuItem();
                        //itemSize[i].Size = new Size(152, 22);
                        itemSize[i].Click += new EventHandler(itemsizeToolStripMenuItem_Click);
                    }
                    itemSize[0].Text = "512x512";
                    itemSize[1].Text = "512x256";
                    itemSize[2].Text = "384x224";
                    resetToolStripMenuItem.DropDownItems.Clear();
                    resetToolStripMenuItem.DropDownItems.AddRange(itemSize);
                    itemSelect();
                    cpsToolStripMenuItem.Enabled = true;
                    CPS.CPSInit();
                    //CPS.GDIInit();
                    break;

            }
            if (Machine.bRom)
            {
                this.Text = "MAME.NET: " + Machine.sDescription + " [" + Machine.sName + "]";
                Mame.init_machine();
                Generic.nvram_load();
            }
            else
            {
                MessageBox.Show("error rom");
            }
            if (Machine.bRom)
            {

                Mame.exit_pending = false;
                this.resetToolStripMenuItem.Enabled = true;

                mainForm.t1 = new Thread(Mame.mame_execute);
                mainForm.t1.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StreamReader sr1 = new StreamReader("mame.ini");
            sr1.ReadLine();
            sSelect = sr1.ReadLine();
            sr1.Close();
            this.Text = Version.build_version;
            resetToolStripMenuItem.Enabled = false;
            gameStripMenuItem.Enabled = false;
            Mame.sHandle1 = this.Handle.ToString();
            RomInfo.Rom=new RomInfo();
            RomInfo.Rom.Name="ffightu";
            RomInfo.Rom.Board = "CPS-1";
            RomInfo.Rom.Parent="ffight";
            RomInfo.Rom.Direction="";
            RomInfo.Rom.Description="";
            RomInfo.Rom.Manufacturer="CapCom";

            LoadRom();
        }

        public void HandleMouse()
        {
            switch (Machine.sName)
            {
                case "drgnwrld":

                    break;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Machine.bRom)
            {
                //UI.cpurun();
            }
            Mame.exit_pending = true;
            Thread.Sleep(100);
            Generic.nvram_save();

            StreamWriter sw1 = new StreamWriter("mame.ini", false);
            sw1.WriteLine("[select]");
            sw1.WriteLine(sSelect);
            sw1.Close();
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Mame.exit_pending = true;
            //if (TrackInfo.CurTrack != null)
            {
                //CPS1.StopDelegate(RomInfo.IStop);
            }
            if (Machine.bRom)
            {
                //UI.cpurun();
                Mame.mame_pause(true);
            }

        }
        /*private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetPicturebox();
        }*/
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void cheatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cheatform.Show();
        }
        private void cheatsearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cheatsearchform.Show();
        }
        private void ipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ipsform.ShowDialog();
        }
        private void cpsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //cpsform.Show();
        }
        private void neogeoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //neogeoform.Show();
        }
        private void namcos1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //namcos1form.Show();
        }
        private void pgmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pgmform.Show();
        }
        private void m72ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //m72form.Show();
        }
        private void m92ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //m92form.Show();
        }
        private void m68000ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //m68000form.Show();
        }
        private void z80ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //z80form.Show();
        }
        private void m6809ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //m6809form.Show();
        }

        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == 0x0112)
            {
                if (msg.WParam.ToString("X4") == "F100")
                {
  
                }
            }
            // Pass message to default handler.
            base.WndProc(ref msg);
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //aboutForm about1 = new aboutForm();
            //about1.ShowDialog();
        }
        private void mainForm_Resize(object sender, EventArgs e)
        {
            ResizeMain();
        }
        private void ResizeMain()
        {
            int deltaX, deltaY;
            switch (Machine.sDirection)
            {
                case "":
                case "180":
                    deltaX = this.Width - (Video.width + 38);
                    deltaY = this.Height - (Video.height + 108);
                    pictureBox1.Width = Video.width + deltaX;
                    pictureBox1.Height = Video.height + deltaY;
                    break;
                case "90":
                case "270":
                    deltaX = this.Width - (Video.height + 38);
                    deltaY = this.Height - (Video.width + 108);
                    pictureBox1.Width = Video.height + deltaX;
                    pictureBox1.Height = Video.width + deltaY;
                    break;
            }
        }
        private void itemsizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i, n;
            n = itemSize.Length;
            for (i = 0; i < n; i++)
            {
                itemSize[i].Checked = false;
            }
            for (i = 0; i < n; i++)
            {
                if (itemSize[i] == (ToolStripItem)sender)
                {
                    Video.iMode = i;
                    itemSelect();
                    break;
                }
            }
        }
        private void itemSelect()
        {
            itemSize[Video.iMode].Checked = true;
            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                case "CPS2":
                    if (Video.iMode == 0)
                    {
                        Video.offsetx = 0;
                        Video.offsety = 0;
                        Video.width = 512;
                        Video.height = 512;
                    }
                    else if (Video.iMode == 1)
                    {
                        Video.offsetx = 0;
                        Video.offsety = 256;
                        Video.width = 512;
                        Video.height = 256;
                    }
                    else if (Video.iMode == 2)
                    {
                        Video.offsetx = 64;
                        Video.offsety = 272;
                        Video.width = 384;
                        Video.height = 224;
                    }
                    break;

            }
            switch (Machine.sDirection)
            {
                case "":
                case "180":
                    this.Width = Video.width + 38;
                    this.Height = Video.height + 108;
                    break;
                case "90":
                case "270":
                    this.Width = Video.height + 38;
                    this.Height = Video.width + 108;
                    break;
            }
            ResizeMain();
        }           
        
    }
}