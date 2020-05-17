using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using cpu.m68000;
using cpu.z80;
//using cpu.m6800;
// cpu.m6809;
//using cpu.nec;
using ui;

namespace mame
{
    public class cpuexec_data
    {
        public int cpunum;
        public byte suspend;
        public byte nextsuspend;
        public byte eatcycles;
        public byte nexteatcycles;
        public int trigger;
        public ulong totalcycles;			// total CPU cycles executed
        public Atime localtime;				// local time, relative to the timer system's global time
        public int cycles_per_second;
        public long attoseconds_per_cycle;
        public int cycles_running;
        public int cycles_stolen;
        public int icount;
        public Timer.emu_timer partial_frame_timer;
        public Atime partial_frame_period;
        public virtual ulong TotalExecutedCycles { get; set; }
        public virtual int PendingCycles { get; set; }        
        public virtual int ExecuteCycles(int cycles) { return 0; }        
        public virtual void Reset() { }
        public virtual void set_irq_line(int irqline, LineState state) { }
        public virtual void set_input_line_and_vector(int line, LineState state, int vector) { }
        public virtual void cpunum_set_input_line_and_vector(int cpunum, int line, LineState state, int vector) { }
    }
    public class Cpuexec
    {
        public static byte SUSPEND_REASON_HALT = 0x01, SUSPEND_REASON_RESET = 0x02, SUSPEND_REASON_SPIN = 0x04, SUSPEND_REASON_TRIGGER = 0x08, SUSPEND_REASON_DISABLE = 0x10, SUSPEND_ANY_REASON = 0xff;
        public static int iloops, activecpu, icpu, ncpu;
        public static cpuexec_data[] cpu;
        public static Timer.emu_timer timedint_timer;
        public static Atime timedint_period;
        public delegate void vblank_delegate();
        public static vblank_delegate vblank_interrupt;
        public static Timer.emu_timer interleave_boost_timer;
        public static Timer.emu_timer interleave_boost_timer_end;
        public static Atime perfect_interleave;
        public static int vblank_interrupts_per_frame;        
        public static void cpuexec_init()
        {
            switch (Machine.sBoard)
            {
                case "CPS-1":
                    MC68000.m1 = new MC68000();
                    Z80A.z1 = new Z80A();
                    MC68000.m1.cpunum = 0;
                    Z80A.z1.cpunum = 1;
                    ncpu = 2;
                    cpu = new cpuexec_data[ncpu];
                    cpu[0] = MC68000.m1;
                    cpu[1] = Z80A.z1;
                    cpu[0].cycles_per_second = 10000000;

                    cpu[1].cycles_per_second = 3579545;
                    cpu[0].attoseconds_per_cycle = Attotime.ATTOSECONDS_PER_SECOND / cpu[0].cycles_per_second;
                    cpu[1].attoseconds_per_cycle = Attotime.ATTOSECONDS_PER_SECOND / cpu[1].cycles_per_second;
                    break;

            }
            activecpu = -1;
            for (icpu = 0; icpu < ncpu; icpu++)
            {
                
                
                    cpu[icpu].suspend = SUSPEND_REASON_RESET;
                    cpu[icpu].localtime = Attotime.ATTOTIME_ZERO;
                    cpu[icpu].TotalExecutedCycles = 0;
                    cpu[icpu].PendingCycles = 0;
                
            }
            compute_perfect_interleave();
        }
        public static void cpuexec_reset()
        {
            switch (Machine.sBoard)
            {
                case "CPS-1":
                    MC68000.m1.ReadOpByte = CPS.MCReadOpByte;
                    MC68000.m1.ReadByte = CPS.MCReadByte;
                    MC68000.m1.ReadOpWord = CPS.MCReadOpWord;
                    MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord;
                    MC68000.m1.ReadOpLong = CPS.MCReadOpLong;
                    MC68000.m1.ReadLong = MC68000.m1.ReadPcrelLong = CPS.MCReadLong;
                    MC68000.m1.WriteByte = CPS.MCWriteByte;
                    MC68000.m1.WriteWord = CPS.MCWriteWord;
                    MC68000.m1.WriteLong = CPS.MCWriteLong;
                    Z80A.z1.ReadOp = CPS.ZCReadOp;
                    Z80A.z1.ReadOpArg = CPS.ZCReadMemory;
                   Z80A.z1.ReadMemory = CPS.ZCReadMemory;
                   Z80A.z1.WriteMemory = CPS.ZCWriteMemory;
                    Z80A.z1.ReadHardware = CPS.ZCReadHardware;
                    Z80A.z1.WriteHardware = CPS.ZCWriteHardware;
                    Z80A.z1.IRQCallback = CPS.ZIRQCallback;                    
                    switch (Machine.sName)
                    {
                        case "forgottn":
                        case "forgottna":
                        case "forgottnu":
                        case "forgottnue":
                        case "forgottnuc":
                        case "forgottnua":
                        case "forgottnuaa":
                        case "lostwrld":
                        case "lostwrldo":
                            MC68000.m1.ReadByte = CPS.MCReadByte_forgottn;
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_forgottn;
                            MC68000.m1.ReadLong = MC68000.m1.ReadPcrelLong = CPS.MCReadLong_forgottn;
                            MC68000.m1.WriteByte = CPS.MCWriteByte_forgottn;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_forgottn;
                            MC68000.m1.WriteLong = CPS.MCWriteLong_forgottn;
                            break;
                        case "sf2ee":
                        case "sf2ue":
                        case "sf2thndr":
                            MC68000.m1.ReadByte = CPS.MCReadByte_sf2thndr;
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_sf2thndr;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_sf2thndr;
                            break;
                        case "sf2ceblp":
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_sf2ceblp;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_sf2ceblp;
                            break;
                        case "sf2m3":
                        case "sf2m8":
                            MC68000.m1.ReadByte = CPS.MCReadByte_sf2m3;
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_sf2m3;
                            MC68000.m1.ReadLong = MC68000.m1.ReadPcrelLong = CPS.MCReadLong_sf2m3;
                            MC68000.m1.WriteByte = CPS.MCWriteByte_sf2m3;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_sf2m3;
                            MC68000.m1.WriteLong = CPS.MCWriteLong_sf2m3;
                            break;
                        case "sf2m10":
                            CPS.mainram2 = new byte[0x100000];
                            CPS.mainram3 = new byte[0x100];
                            MC68000.m1.ReadByte = CPS.MCReadByte_sf2m10;
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_sf2m10;
                            MC68000.m1.ReadLong = MC68000.m1.ReadPcrelLong = CPS.MCReadLong_sf2m10;
                            MC68000.m1.WriteByte = CPS.MCWriteByte_sf2m10;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_sf2m10;
                            MC68000.m1.WriteLong = CPS.MCWriteLong_sf2m10;
                            break;
                        case "sf2dongb":
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_sf2dongb;
                            break;
                        case "dinohunt":
                            MC68000.m1.ReadByte = CPS.MCReadByte_dinohunt;
                            break;
                        case "pang3":
                        case "pang3r1":
                        case "pang3j":
                        case "pang3b":
                            MC68000.m1.ReadByte = CPS.MCReadByte_pang3;
                            MC68000.m1.ReadWord = MC68000.m1.ReadPcrelWord = CPS.MCReadWord_pang3;
                            MC68000.m1.WriteByte = CPS.MCWriteByte_pang3;
                            MC68000.m1.WriteWord = CPS.MCWriteWord_pang3;
                            break;
                    }
                    break;
 
            }
            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                case "CPS2":
                case "Neo Geo":
                case "PGM":

                    break;                

            }
            for (icpu = 0; icpu < ncpu; icpu++)
            {
                cpu[icpu].Reset();
            }
        }
        public static void null_callback()
        {

        }
        public static byte null_callback1(int address)
        {
            return 0;
        }
        public static ushort null_callback2(int address)
        {
            return 0;
        }
        public static void null_callback(int address, byte value)
        {

        }
        public static void null_callback(int address, ushort value)
        {

        }
        private static void cpu_inittimers()
        {
            switch (Machine.sBoard)
            {
                case "CPS-1(QSound)":
                case "CPS2":
                    timedint_period = new Atime(0, (long)(1e18 / 250));
                    timedint_timer = Timer.timer_alloc_common(Generic.irq_1_0_line_hold, "irq_1_0_line_hold", false);
                    Timer.timer_adjust_periodic(timedint_timer, timedint_period, timedint_period);
                    break;
                /*case "Neo Geo":
                    interleave_boost_timer = Timer.timer_alloc_common(null_callback, "boost_callback", false);
                    interleave_boost_timer_end = Timer.timer_alloc_common(end_interleave_boost, "end_interleave_boost", false);
                    break;
                case "CPS1":
                case "Namco System 1":                
                case "IGS011":
                case "PGM":
                case "M72":
                case "M92":
                    break;*/
            }
        }        
        public static void cpuexec_timeslice()
        {
            StreamWriter sw2 = null;
            Atime target = Timer.lt[0].expire;
            Atime tbase = Timer.global_basetime;
            int ran;
            Atime at;
            for (icpu = 0; icpu < ncpu; icpu++)
            {
                cpu[icpu].suspend = cpu[icpu].nextsuspend;
                cpu[icpu].eatcycles = cpu[icpu].nexteatcycles;
            }
            for (icpu = 0; icpu < ncpu; icpu++)
            {
                if (cpu[icpu].suspend == 0)
                {
                    at = Attotime.attotime_sub(target, cpu[icpu].localtime);
                    cpu[icpu].cycles_running = (int)(at.seconds * cpu[icpu].cycles_per_second + at.attoseconds / cpu[icpu].attoseconds_per_cycle);
                    if (cpu[icpu].cycles_running > 0)
                    {
                        cpu[icpu].cycles_stolen = 0;
                        activecpu = icpu;
                        ran = cpu[icpu].ExecuteCycles(cpu[icpu].cycles_running);
                        activecpu = -1;
                        ran -= cpu[icpu].cycles_stolen;
                        cpu[icpu].totalcycles += (ulong)ran;
                        cpu[icpu].localtime = Attotime.attotime_add(cpu[icpu].localtime, new Atime(ran / cpu[icpu].cycles_per_second, ran * cpu[icpu].attoseconds_per_cycle));
                        if (Attotime.attotime_compare(cpu[icpu].localtime, target) < 0)
                        {
                            if (Attotime.attotime_compare(cpu[icpu].localtime, tbase) > 0)
                                target = cpu[icpu].localtime;
                            else
                                target = tbase;
                        }
                    }
                }
            }
            for (icpu = 0; icpu < ncpu; icpu++)
            {
                if (cpu[icpu].suspend != 0 && cpu[icpu].eatcycles != 0 && Attotime.attotime_compare(cpu[icpu].localtime, target) < 0)
                {
                    at = Attotime.attotime_sub(target, cpu[icpu].localtime);
                    cpu[icpu].cycles_running = (int)(at.seconds * cpu[icpu].cycles_per_second + at.attoseconds / cpu[icpu].attoseconds_per_cycle);
                    cpu[icpu].totalcycles += (ulong)cpu[icpu].cycles_running;
                    cpu[icpu].localtime = Attotime.attotime_add(cpu[icpu].localtime, new Atime(cpu[icpu].cycles_running / cpu[icpu].cycles_per_second, cpu[icpu].cycles_running * cpu[icpu].attoseconds_per_cycle));
                }
                cpu[icpu].suspend = cpu[icpu].nextsuspend;
                cpu[icpu].eatcycles = cpu[icpu].nexteatcycles;
            }
            Timer.timer_set_global_time(target);
           /* if (Timer.global_basetime.attoseconds == 0 && Machine.FORM.cheatform.lockState == cheatForm.LockState.LOCK_SECOND)
            {
                Machine.FORM.cheatform.ApplyCheat();
            }*/
        }
        public static void cpu_boost_interleave(Atime timeslice_time, Atime boost_duration)
        {
            if (Attotime.attotime_compare(timeslice_time, perfect_interleave) < 0)
                timeslice_time = perfect_interleave;
            Timer.timer_adjust_periodic(interleave_boost_timer, timeslice_time, timeslice_time);
            if (!Timer.timer_enabled(interleave_boost_timer_end) || Attotime.attotime_compare(Timer.timer_timeleft(interleave_boost_timer_end), boost_duration) < 0)
                Timer.timer_adjust_periodic(interleave_boost_timer_end, boost_duration, Attotime.ATTOTIME_NEVER);
        }
        public static void activecpu_abort_timeslice(int cpunum)
        {
            int current_icount;
            current_icount = cpu[cpunum].PendingCycles + 1;
            cpu[cpunum].cycles_stolen += current_icount;
            cpu[cpunum].cycles_running -= current_icount;
            cpu[cpunum].PendingCycles = -1;
        }
        public static void cpunum_suspend(int cpunum, byte reason, byte eatcycles)
        {
            cpu[cpunum].nextsuspend |= reason;
            cpu[cpunum].nexteatcycles = eatcycles;
            if (Cpuexec.activecpu >= 0)
            {
                activecpu_abort_timeslice(Cpuexec.activecpu);
            }
        }
        public static void cpunum_resume(int cpunum, byte reason)
        {
            cpu[cpunum].nextsuspend &= (byte)(~reason);
            if (Cpuexec.activecpu >= 0)
            {
                activecpu_abort_timeslice(Cpuexec.activecpu);
            }
        }
        public static bool cpunum_is_suspended(int cpunum, byte reason)
        {
            return ((cpu[cpunum].nextsuspend & reason) != 0);
        }
        public static Atime cpunum_get_localtime(int cpunum)
        {
            Atime result;
            result = cpu[cpunum].localtime;
            int cycles;
            cycles = cpu[cpunum].cycles_running - cpu[cpunum].PendingCycles;
            result = Attotime.attotime_add(result, new Atime(cycles / cpu[cpunum].cycles_per_second, cycles * cpu[cpunum].attoseconds_per_cycle));
            return result;
        }
        public static void cpunum_suspend_until_trigger(int cpunum, int trigger, int eatcycles)
        {
            cpunum_suspend(cpunum, (byte)SUSPEND_REASON_TRIGGER, (byte)eatcycles);
            cpu[cpunum].trigger = trigger;
        }
        public static void on_vblank()
        {
            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                    CPS.cps1_interrupt();
                    break;
            }
        }
        public static void trigger_partial_frame_interrupt()
        {
            switch (Machine.sBoard)
            {
                case "CPS2":
                case "IGS011":
                    if (iloops == 0)
                    {
                        iloops = vblank_interrupts_per_frame;
                    }
                    iloops--;
                    vblank_interrupt();
                    if (iloops > 1)
                    {
                        Timer.timer_adjust_periodic(Cpuexec.cpu[0].partial_frame_timer, Cpuexec.cpu[0].partial_frame_period, Attotime.ATTOTIME_NEVER);
                    }
                    break;
                case "M72":
                    if (iloops == 0)
                    {
                        iloops = vblank_interrupts_per_frame;
                    }
                    iloops--;
                    vblank_interrupt();
                    if (iloops > 1)
                    {
                        Timer.timer_adjust_periodic(Cpuexec.cpu[1].partial_frame_timer, Cpuexec.cpu[1].partial_frame_period, Attotime.ATTOTIME_NEVER);
                    }
                    break;
            }
        }
        public static void end_interleave_boost()
        {
            Timer.timer_adjust_periodic(interleave_boost_timer, Attotime.ATTOTIME_NEVER, Attotime.ATTOTIME_NEVER);
        }
        public static void compute_perfect_interleave()
        {
            long smallest = cpu[0].attoseconds_per_cycle;
            int cpunum;
            perfect_interleave = Attotime.ATTOTIME_ZERO;
            perfect_interleave.attoseconds = Attotime.ATTOSECONDS_PER_SECOND - 1;
            for (cpunum = 1; cpunum < ncpu; cpunum++)
            {
                if (cpu[cpunum].attoseconds_per_cycle < smallest)
                {
                    perfect_interleave.attoseconds = smallest;
                    smallest = cpu[cpunum].attoseconds_per_cycle;
                }
                else if (cpu[cpunum].attoseconds_per_cycle < perfect_interleave.attoseconds)
                    perfect_interleave.attoseconds = cpu[cpunum].attoseconds_per_cycle;
            }
            if (perfect_interleave.attoseconds == Attotime.ATTOSECONDS_PER_SECOND - 1)
                perfect_interleave.attoseconds = cpu[0].attoseconds_per_cycle;
        }
    }
}
