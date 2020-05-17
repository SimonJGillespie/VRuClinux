using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace mame
{
    public partial class CPS
    {
        public static ushort[] cps_a_regs, cps_b_regs, cps2_objram1, cps2_objram2, cps2_output;
        public static byte[] mainromop, gfxrom,gfx1rom, audioromop, starsrom, user1rom;
        public static byte[] gfxram;
        public static byte[] qsound_sharedram1, qsound_sharedram2;
        public static byte[] mainram2, mainram3;
        public static byte dswa, dswb, dswc;
        public static int cps_version;
        public static int basebanksnd;
        public static int sf2ceblp_prot;
        public static int dial0, dial1;
        public static int scrollxoff, scrollyoff;
        public static int cps2networkpresent, cps2_objram_bank;
        public static int scancount, cps1_scanline1, cps1_scanline2, cps1_scancalls;
        public static List<gfx_range> lsRange0, lsRange1, lsRange2, lsRangeS;
        public class gfx_range
        {
            public int start;
            public int end;
            public int add;
            public gfx_range(int i1, int i2, int i3)
            {
                start = i1;
                end = i2;
                add = i3;
            }
        }
        public static sbyte[] ByteToSbyte(byte[] bb1)
        {
            sbyte[] bb2 = null;
            int n1;
            if (bb1 != null)
            {
                n1 = bb1.Length;
                bb2 = new sbyte[n1];
                Buffer.BlockCopy(bb1, 0, bb2, 0, n1);
            }
            return bb2;
        }
        public static void CPSInit()
        {
            int i, n;
            cps_a_regs = new ushort[0x20];
            cps_b_regs = new ushort[0x20];
            gfxram = new byte[0x30000];
            Memory.mainram = new byte[0x100000];
            Memory.audioram = new byte[0x800];
            Machine.bRom = true;
            Memory.mainrom = Machine.GetRom("maincpu.rom");
            gfxrom = Machine.GetRom("gfx.rom");
            n = gfxrom.Length;
            gfx1rom = new byte[n * 2];
            for (i = 0; i < n; i++)
            {
                gfx1rom[i * 2] = (byte)(gfxrom[i] & 0x0f);
                gfx1rom[i * 2 + 1] = (byte)(gfxrom[i] >> 4);
            }
            Memory.audiorom = Machine.GetRom("audiocpu.rom");
            switch (Machine.sBoard)
            {
                case "CPS-1":
                    cps_version = 1;
                    starsrom = Machine.GetRom("stars.rom");
                    //OKI6295.okirom = Machine.GetRom("oki.rom");
                    if (Memory.mainrom == null || gfxrom == null /*|| Memory.audiorom == null || OKI6295.okirom == null*/)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "CPS-1(QSound)":
                    cps_version = 1;
                    qsound_sharedram1 = new byte[0x1000];
                    qsound_sharedram2 = new byte[0x1000];
                    audioromop = Machine.GetRom("audiocpuop.rom");
                    user1rom = Machine.GetRom("user1.rom");
                    //QSound.qsoundrom = ByteToSbyte(Machine.GetRom("qsound.rom"));
                    if (Memory.mainrom == null || audioromop == null || gfxrom == null || Memory.audiorom == null /*|| QSound.qsoundrom == null*/)
                    {
                        Machine.bRom = false;
                    }
                    break;
                case "CPS2":
                    cps_version = 2;
                    cps2_objram1 = new ushort[0x1000];
                    cps2_objram2 = new ushort[0x1000];
                    cps2_output = new ushort[0x06];
                    cps2networkpresent = 0;
                    cps2_objram_bank = 0;
                    scancount = 0;
                    cps1_scanline1 = 262;
                    cps1_scanline2 = 262;
                    cps1_scancalls = 0;
                    qsound_sharedram1 = new byte[0x1000];
                    qsound_sharedram2 = new byte[0x1000];
                    if (Machine.sManufacturer != "bootleg")
                    {
                        mainromop = Machine.GetRom("maincpuop.rom");
                    }
                    audioromop = Machine.GetRom("audiocpu.rom");
                    //QSound.qsoundrom = ByteToSbyte(Machine.GetRom("qsound.rom"));
                    if (Memory.mainrom == null || (Machine.sManufacturer != "bootleg" && mainromop == null) || audioromop == null || gfxrom == null || Memory.audiorom == null /*|| QSound.qsoundrom == null*/)
                    {
                        Machine.bRom = false;
                    }
                    break;
            }
            if (Machine.bRom)
            {
                scrollxoff = 0x00;
                scrollyoff = 0x100;
                switch (Machine.sName)
                {

                    case "ffight":
                    case "ffighta":
                    case "ffightu":
                    case "ffightu1":
                    case "ffightj":
                        cpsb_addr = 0x20;
                        cpsb_value = 0x0004;
                        mult_factor1 = -1;
                        mult_factor2 = -1;
                        mult_result_lo = -1;
                        mult_result_hi = -1;
                        layer_control = 0x2e;
                        priority = new int[4] { 0x26, 0x30, 0x28, 0x32 };
                        palette_control = 0x2a;
                        layer_enable_mask = new int[5] { 0x02, 0x04, 0x08, 0x00, 0x00 };
                        in2_addr = 0x00;
                        in3_addr = 0x00;
                        out2_addr = 0x00;
                        bootleg_kludge = 0;
                        dswa = 0xff;
                        dswb = 0xf4;
                        dswc = 0x9f;
                        lsRange0 = new List<gfx_range>();
                        lsRange0.Add(new gfx_range(0x4400, 0x4bff, 0));
                        lsRange1 = new List<gfx_range>();
                        lsRange1.Add(new gfx_range(0x3000, 0x3fff, 0));
                        lsRange2 = new List<gfx_range>();
                        lsRange2.Add(new gfx_range(0x0980, 0x0bff, 0));
                        lsRangeS = new List<gfx_range>();
                        lsRangeS.Add(new gfx_range(0x0000, 0x21ff, 0));
                        break;

                }
                if (cps_version == 2)
                {
                    cpsb_addr = 0x32;
                    cpsb_value = -1;
                    mult_factor1 = 0x00;
                    mult_factor2 = 0x02;
                    mult_result_lo = 0x04;
                    mult_result_hi = 0x06;
                    layer_control = 0x26;
                    priority = new int[4] { 0x28, 0x2a, 0x2c, 0x2e };
                    palette_control = 0x30;
                    layer_enable_mask = new int[5] { 0x02, 0x04, 0x08, 0x30, 0x30 };
                    in2_addr = 0x00;
                    in3_addr = 0x00;
                    out2_addr = 0x00;
                    bootleg_kludge = 0;
                    lsRange0 = new List<gfx_range>();
                    lsRange0.Add(new gfx_range(0x0000, 0x1ffff, 0x20000));
                    lsRange1 = new List<gfx_range>();
                    lsRange1.Add(new gfx_range(0x0000, 0xffff, 0x10000));
                    lsRange2 = new List<gfx_range>();
                    lsRange2.Add(new gfx_range(0x0000, 0x3fff, 0x4000));
                    lsRangeS = new List<gfx_range>();
                    lsRangeS.Add(new gfx_range(0x0000, 0xffff, 0));
                }
            }
        }
        public static sbyte cps1_dsw_r(int offset)
        {
            string[] dswname = { "IN0", "DSWA", "DSWB", "DSWC" };
            int in0 = 0;
            if (offset == 0)
            {
                in0 = sbyte0;
            }
            else if (offset == 1)
            {
                in0 = dswa;
            }
            else if (offset == 2)
            {
                in0 = dswb;
            }
            else if (offset == 3)
            {
                in0 = dswc;
            }
            else
            {
                in0 = 0;
            }
            return (sbyte)in0;
        }
        public static void cps1_snd_bankswitch_w(byte data)
        {
            int bankaddr;
            bankaddr = ((data & 1) * 0x4000);
            basebanksnd = 0x10000 + bankaddr;
        }
        public static void cps1_oki_pin7_w(byte data)
        {
            //OKI6295.okim6295_set_pin7(data & 1);
        }
        public static void cps1_coinctrl_w(ushort data)
        {
            Generic.coin_counter_w(0, data & 0x0100);
            Generic.coin_counter_w(1, data & 0x0200);
            Generic.coin_lockout_w(0, ~data & 0x0400);
            Generic.coin_lockout_w(1, ~data & 0x0800);
        }
        public static void qsound_banksw_w(byte data)
        {
            basebanksnd = 0x10000 + ((data & 0x0f) * 0x4000);
        }
        public static short qsound_rom_r(int offset)
        {
            if (user1rom != null)
            {
                return (short)(user1rom[offset] | 0xff00);
            }
            else
            {
                return 0;
            }
        }
        public static short qsound_sharedram1_r(int offset)
        {
            return (short)(qsound_sharedram1[offset] | 0xff00);
        }
        public static void qsound_sharedram1_w(int offset, byte data)
        {
            qsound_sharedram1[offset] = (byte)data;
        }
        public static short qsound_sharedram2_r(int offset)
        {
            return (short)(qsound_sharedram2[offset] | 0xff00);
        }
        public static void qsound_sharedram2_w(int offset, byte data)
        {
            qsound_sharedram2[offset] = (byte)(data);
        }
        public static void cps1_interrupt()
        {
            Cpuint.cpunum_set_input_line(0, 5, LineState.HOLD_LINE);
        }
        public static void cpsq_coinctrl2_w(ushort data)
        {
            Generic.coin_counter_w(2, data & 0x01);
            Generic.coin_lockout_w(2, ~data & 0x02);
            Generic.coin_counter_w(3, data & 0x04);
            Generic.coin_lockout_w(3, ~data & 0x08);
        }
        public static int cps1_eeprom_port_r()
        {
            return Eeprom.eeprom_read_bit();
        }
        public static void cps1_eeprom_port_w(int data)
        {
            /*
            bit 0 = data
            bit 6 = clock
            bit 7 = cs
            */
            Eeprom.eeprom_write_bit(data & 0x01);
            Eeprom.eeprom_set_cs_line(((data & 0x80) != 0) ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Eeprom.eeprom_set_clock_line(((data & 0x40) != 0) ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
        }
        public static void sf2m3_layer_w(ushort data)
        {
            cps1_cps_b_w(0x0a, data);
        }
        public static short cps2_objram2_r(int offset)
        {
            if ((cps2_objram_bank & 1) != 0)
            {
                return (short)cps2_objram1[offset];
            }
            else
            {
                return (short)cps2_objram2[offset];
            }
        }
        public static void cps2_objram1_w(int offset, ushort data)
        {
            if ((cps2_objram_bank & 1) != 0)
            {
                cps2_objram2[offset] = data;
            }
            else
            {
                cps2_objram1[offset] = data;
            }
        }
        public static void cps2_objram2_w(int offset, ushort data)
        {
            if ((cps2_objram_bank & 1) != 0)
            {
                cps2_objram1[offset] = data;
            }
            else
            {
                cps2_objram2[offset] = data;
            }
        }
        public static short cps2_qsound_volume_r()
        {
            if (cps2networkpresent != 0)
            {
                return (short)0x2021;
            }
            else
            {
                return unchecked((short)(0xe021));
            }
        }
        public static short kludge_r()
        {
            return -1;
        }
        public static void cps2_eeprom_port_bh(int data)
        {
            data = (data & 0xff) << 8;
            Eeprom.eeprom_write_bit(data & 0x1000);
            Eeprom.eeprom_set_clock_line(((data & 0x2000) != 0) ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
            Eeprom.eeprom_set_cs_line(((data & 0x4000) != 0) ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
        }
        public static void cps2_eeprom_port_bl(int data)
        {
            Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_RESET, ((data & 0x0008) != 0) ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            Generic.coin_counter_w(0, data & 0x0001);
            Generic.coin_counter_w(1, data & 0x0002);
            Generic.coin_lockout_w(0, ~data & 0x0010);
            Generic.coin_lockout_w(1, ~data & 0x0020);
            Generic.coin_lockout_w(2, ~data & 0x0040);
            Generic.coin_lockout_w(3, ~data & 0x0080);
        }
        public static void cps2_eeprom_port_w(int data)
        {
            //high 8 bits
            {
                /* bit 0 - Unused */
                /* bit 1 - Unused */
                /* bit 2 - Unused */
                /* bit 3 - Unused? */
                /* bit 4 - Eeprom data  */
                /* bit 5 - Eeprom clock */
                /* bit 6 - */
                /* bit 7 - */

                /* EEPROM */
                Eeprom.eeprom_write_bit(data & 0x1000);
                Eeprom.eeprom_set_clock_line(((data & 0x2000)!=0) ? LineState.ASSERT_LINE : LineState.CLEAR_LINE);
                Eeprom.eeprom_set_cs_line(((data & 0x4000)!=0) ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);
            }
            //low 8 bits
            {
                /* bit 0 - coin counter 1 */
                /* bit 0 - coin counter 2 */
                /* bit 2 - Unused */
                /* bit 3 - Allows access to Z80 address space (Z80 reset) */
                /* bit 4 - lock 1  */
                /* bit 5 - lock 2  */
                /* bit 6 - */
                /* bit 7 - */

                /* Z80 Reset */
                Cpuint.cpunum_set_input_line(1, (int)LineState.INPUT_LINE_RESET, ((data & 0x0008) != 0) ? LineState.CLEAR_LINE : LineState.ASSERT_LINE);

                Generic.coin_counter_w(0, data & 0x0001);                
                Generic.coin_counter_w(1, data & 0x0002);
                
                Generic.coin_lockout_w(0, ~data & 0x0010);
                Generic.coin_lockout_w(1, ~data & 0x0020);
                Generic.coin_lockout_w(2, ~data & 0x0040);
                Generic.coin_lockout_w(3, ~data & 0x0080);

                /*
                set_led_status(0,data & 0x01);
                set_led_status(1,data & 0x10);
                set_led_status(2,data & 0x20);
                */
            }
        }
        public static void cps2_objram_bank_w(int data)
        {
            cps2_objram_bank = data & 1;
        }
        public static void cps2_interrupt()
        {
            /* 2 is vblank, 4 is some sort of scanline interrupt, 6 is both at the same time. */
            if (scancount >= 261)
            {
                scancount = -1;
                cps1_scancalls = 0;
            }
            scancount++;
            if ((cps_b_regs[0x10 / 2] & 0x8000) != 0)
            {
                cps_b_regs[0x10 / 2] = (ushort)(cps_b_regs[0x10 / 2] & 0x1ff);
            }
            if ((cps_b_regs[0x12 / 2] & 0x8000) != 0)
            {
                cps_b_regs[0x12 / 2] = (ushort)(cps_b_regs[0x12 / 2] & 0x1ff);
            }
            /*if(cps1_scanline1 == scancount || (cps1_scanline1 < scancount && (cps1_scancalls!=0)))
            {
                CPS1.cps1_cps_b_regs[0x10 / 2] = 0;

                cpunum_set_input_line(machine, 0, 4, HOLD_LINE);
                cps2_set_sprite_priorities();
                video_screen_update_partial(machine->primary_screen, 16 - 10 + scancount);
                cps1_scancalls++;
            }
            if(cps1_scanline2 == scancount || (cps1_scanline2 < scancount && !cps1_scancalls))
            {
                cps1_cps_b_regs[0x12/2] = 0;
                cpunum_set_input_line(machine, 0, 4, HOLD_LINE);
                cps2_set_sprite_priorities();
                video_screen_update_partial(machine->primary_screen, 16 - 10 + scancount);
                cps1_scancalls++;
            }*/
            if (scancount == 256)  /* VBlank */
            {
                cps_b_regs[0x10 / 2] = (ushort)cps1_scanline1;
                cps_b_regs[0x12 / 2] = (ushort)cps1_scanline2;
                Cpuint.cpunum_set_input_line(0, 2, LineState.HOLD_LINE);
                if (cps1_scancalls != 0)
                {
                    cps2_set_sprite_priorities();
                    //video_screen_update_partial(machine->primary_screen, 256);
                }
                cps2_objram_latch();
            }
        }
        public static void machine_reset_cps()
        {
            basebanksnd = 0;
        }
    }
}
