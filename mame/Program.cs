using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using mame;
using System.Threading;
using System.IO;


namespace ui
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Control.CheckForIllegalCrossThreadCalls = false;
            Form1_Load();
        }


        public static string sSelect;
        public static Thread t1;
        public static string handle1;

        public static void LoadRom()
        {


            //this.Close();
            Mame.exit_pending = true;
            Thread.Sleep(100);

            mame.Timer.lt = new List<mame.Timer.emu_timer>();
            sSelect = RomInfo.Rom.Name;
            //Machine.FORM = this;
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

            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                case "CPS2":
                    Video.nMode = 3;
                    CPS.CPSInit();
                    break;

            }
            if (Machine.bRom)
            {
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
                t1 = new Thread(Mame.mame_execute);
                t1.Start();
            }
        }

        private static void Form1_Load()
        {
            StreamReader sr1 = new StreamReader("mame.ini");
           sr1.ReadLine();
            sSelect = sr1.ReadLine();
            sr1.Close();
            RomInfo.Rom = new RomInfo();
            RomInfo.Rom.Name = "ffightu";
            RomInfo.Rom.Board = "CPS-1";
            RomInfo.Rom.Parent = "ffight";
            RomInfo.Rom.Direction = "";
            RomInfo.Rom.Description = "";
            RomInfo.Rom.Manufacturer = "CapCom";

            LoadRom();
        }

    }
}