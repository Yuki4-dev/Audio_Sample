using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Audio_Sample
{
    public class MicSoundRecorder
    {
        public event EventHandler WaveOpen;
        public event EventHandler WaveClose;
        public event EventHandler<byte[]> WaveData;
        public RecorderState State { get; private set; } = RecorderState.Close;

        private NativeMethods.DelegateWaveInProc _WaveProc;
        private IntPtr _Hwi = IntPtr.Zero;
        private NativeMethods.WaveFormatEx _WaveFormat;
        private NativeMethods.WaveHdr _WaveHdr;

        public MicSoundRecorder()
        {
            _WaveProc = new NativeMethods.DelegateWaveInProc(WaveInProc);

            _WaveFormat = new NativeMethods.WaveFormatEx();
            _WaveFormat.wFormatTag = NativeMethods.WAVE_FORMAT_PCM;
            _WaveFormat.cbSize = 0;
            _WaveFormat.nChannels = 1;
            _WaveFormat.nSamplesPerSec = 11025;
            _WaveFormat.wBitsPerSample = 8;
            _WaveFormat.nBlockAlign = (short)(_WaveFormat.wBitsPerSample / 8 * _WaveFormat.nChannels);
            _WaveFormat.nAvgBytesPerSec = _WaveFormat.nSamplesPerSec * _WaveFormat.nBlockAlign;
        }

        public bool TryWaveInOpen()
        {
            if (State != RecorderState.Close)
            {
                return true;
            }

            var result = NativeMethods.waveInOpen(ref _Hwi, NativeMethods.WAVE_MAPPER, ref _WaveFormat, _WaveProc, IntPtr.Zero, NativeMethods.CALLBACK_FUNCTION);
            if (result == NativeMethods.MMSYSERR_NOERROR)
            {
                return true;
            }

            return false;
        }

        public void WaveInStart(int recordSecond)
        {
            if (State == RecorderState.Close || State == RecorderState.Recording)
            {
                throw new InvalidOperationException($"State : {State}");
            }

            var dataSize = _WaveFormat.nAvgBytesPerSec * recordSecond;
            _WaveHdr = new NativeMethods.WaveHdr();
            _WaveHdr.lpData = Marshal.AllocHGlobal(dataSize);
            _WaveHdr.dwBufferLength = dataSize;

            var cdwh = Marshal.SizeOf<NativeMethods.WaveHdr>();
            NativeMethods.waveInPrepareHeader(_Hwi, ref _WaveHdr, cdwh);
            NativeMethods.waveInAddBuffer(_Hwi, ref _WaveHdr, cdwh);
            if (State == RecorderState.Open)
            {
                NativeMethods.waveInStart(_Hwi);
            }

            State = RecorderState.Recording;
        }

        public void WaveInProc(IntPtr hwi, uint uMsg, IntPtr dwInstance, IntPtr dwParam1, IntPtr dwParam2)
        {
            switch (uMsg)
            {
                case NativeMethods.WIM_OPEN:

                    State = RecorderState.Open;
                    WaveOpen?.Invoke(this, EventArgs.Empty);
                    break;
                case NativeMethods.WIM_CLOSE:

                    State = RecorderState.Close;
                    WaveClose?.Invoke(this, EventArgs.Empty);
                    break;
                case NativeMethods.WIM_DATA:

                    // var wh = Marshal.PtrToStructure<NativeMethods.WaveHdr>(dwParam1);
                    State = RecorderState.Ready;
                    OnWaveData();
                    break;
                default:
                    break;
            }
        }

        private void OnWaveData()
        {
            var headerSize = 44;
            var dataSize = _WaveHdr.dwBufferLength + headerSize;
            var waveData = new byte[dataSize];

            Array.Copy(Encoding.ASCII.GetBytes("RIFF"), 0, waveData, 0, 4);
            Array.Copy(BitConverter.GetBytes((uint)(dataSize - 8)), 0, waveData, 4, 4);
            Array.Copy(Encoding.ASCII.GetBytes("WAVE"), 0, waveData, 8, 4);
            Array.Copy(Encoding.ASCII.GetBytes("fmt "), 0, waveData, 12, 4);
            Array.Copy(BitConverter.GetBytes((uint)16), 0, waveData, 16, 4);
            Array.Copy(BitConverter.GetBytes((ushort)(_WaveFormat.wFormatTag)), 0, waveData, 20, 2);
            Array.Copy(BitConverter.GetBytes((ushort)(_WaveFormat.nChannels)), 0, waveData, 22, 2);
            Array.Copy(BitConverter.GetBytes((uint)(_WaveFormat.nSamplesPerSec)), 0, waveData, 24, 4);
            Array.Copy(BitConverter.GetBytes((uint)(_WaveFormat.nAvgBytesPerSec)), 0, waveData, 28, 4);
            Array.Copy(BitConverter.GetBytes((ushort)(_WaveFormat.nBlockAlign)), 0, waveData, 32, 2);
            Array.Copy(BitConverter.GetBytes((ushort)(_WaveFormat.wBitsPerSample)), 0, waveData, 34, 2);
            Array.Copy(Encoding.ASCII.GetBytes("data"), 0, waveData, 36, 4);
            Array.Copy(BitConverter.GetBytes((uint)(_WaveHdr.dwBufferLength)), 0, waveData, 40, 4);
            Marshal.Copy(_WaveHdr.lpData, waveData, headerSize, _WaveHdr.dwBufferLength);

            NativeMethods.waveInUnprepareHeader(_Hwi, ref _WaveHdr, Marshal.SizeOf<NativeMethods.WaveHdr>());
            Marshal.FreeHGlobal(_WaveHdr.lpData);

            WaveData?.Invoke(this, waveData);
        }

        public void WaveInClose()
        {
            if (State == RecorderState.Close)
            {
                return;
            }

            if (State != RecorderState.Open)
            {
                NativeMethods.waveInStop(_Hwi);
            }
            //NativeMethods.waveInReset(_Hwi);
            NativeMethods.waveInClose(_Hwi);
        }

        public enum RecorderState
        {
            Close, Open, Ready, Recording,
        }
    }
}
