using System;
using System.Runtime.InteropServices;

namespace Audio_Sample
{
    public class NativeMethods
    {
        const int MAXPNAMELEN = 32;

        public const int MMSYSERR_NOERROR = 0;

        public const int WAVE_FORMAT_1M08 = 0x00001;
        public const int WAVE_FORMAT_1S08 = 0x00002;
        public const int WAVE_FORMAT_1M16 = 0x00004;
        public const int WAVE_FORMAT_1S16 = 0x00008;
        public const int WAVE_FORMAT_2M08 = 0x00010;
        public const int WAVE_FORMAT_2S08 = 0x00020;
        public const int WAVE_FORMAT_2M16 = 0x00040;
        public const int WAVE_FORMAT_2S16 = 0x00080;
        public const int WAVE_FORMAT_4M08 = 0x00100;
        public const int WAVE_FORMAT_4S08 = 0x00200;
        public const int WAVE_FORMAT_4M16 = 0x00400;
        public const int WAVE_FORMAT_4S16 = 0x00800;

        public const int CALLBACK_WINDOW = 0x10000;
        public const int CALLBACK_FUNCTION = 0x30000;

        public const int WIM_OPEN = 0x3BE;
        public const int WIM_CLOSE = 0x3BF;
        public const int WIM_DATA = 0x3C0;

        public const int WAVE_FORMAT_PCM = 1;
        public const int WAVE_MAPPER = -1;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WaveInCaps
        {
            public Int16 wMid;
            public Int16 wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAXPNAMELEN)]
            public string szPname;
            public int dwFormats;
            public Int16 wChannels;
            Int16 wReserved1;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WaveFormatEx
        {
            public Int16 wFormatTag;
            public Int16 nChannels;
            public int nSamplesPerSec;
            public int nAvgBytesPerSec;
            public Int16 nBlockAlign;
            public Int16 wBitsPerSample;
            public Int16 cbSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct WaveHdr
        {
            public IntPtr lpData;
            public int dwBufferLength;
            public int dwBytesRecorded;
            public IntPtr dwUser;
            public int dwFlags;
            public int dwLoops;
            public IntPtr lpNext;
            public int reserved;
        }

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInGetNumDevs();

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInGetDevCaps(int uDeviceID, ref WaveInCaps wic, int cbwic);

        public delegate void DelegateWaveInProc(IntPtr hwi, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int waveInOpen(ref IntPtr hwi, int uDeviceID, ref WaveFormatEx wfx, DelegateWaveInProc dwCallback, IntPtr dwCallbackInstance, int fdwOpen);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInClose(IntPtr hwi);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInReset(IntPtr hwi);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInPrepareHeader(IntPtr hwi, ref WaveHdr wh, int cbwh);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInUnprepareHeader(IntPtr hwi, ref WaveHdr wh, int cbwh);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInAddBuffer(IntPtr hwi, ref WaveHdr wh, int cbwh);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInStart(IntPtr hwi);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int waveInStop(IntPtr hwi);
    }
}
