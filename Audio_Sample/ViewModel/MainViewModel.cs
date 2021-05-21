using Microsoft.Win32;
using System.IO;
using System.Windows.Input;

namespace Audio_Sample
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand ExecuteCommand { get; }

        public string _RecordSecond = "5";
        public string RecordSecond
        {
            get => _RecordSecond;
            set => OnPropertyChanged(ref _RecordSecond, value, nameof(RecordSecond));
        }

        private string _StausText = "";
        public string StausText
        {
            get => _StausText;
            set => OnPropertyChanged(ref _StausText, value, nameof(StausText));
        }

        private readonly MicSoundRecorder _MicSoundRecorder = new MicSoundRecorder();

        public MainViewModel()
        {
            ExecuteCommand = new Command((p) => RecordingStart());
            _MicSoundRecorder.WaveData += MicSoundRecorder_WaveDate;
        }

        private void RecordingStart()
        {
            IsBusy = true;
            OnShowMessageBox("録音を開始します。");
            StausText = "録音中...";

            if (_MicSoundRecorder.TryWaveInOpen())
            {
                _MicSoundRecorder.WaveInStart(int.Parse(RecordSecond));
            }
            else
            {
                OnShowMessageBox("デバイスのオープンに失敗しました。");
                StausText = "";
                IsBusy = false;
            }
        }

        private void MicSoundRecorder_WaveDate(object sender, byte[] waveData)
        {
            StausText = "保存中...";

            var args = new ShowCommonDialogEventArgs(typeof(SaveFileDialog))
            {
                IsModal = true,
                PreparationDialog = (d) =>
                {
                    ((SaveFileDialog)d).Title = "ファイルを保存";
                    ((SaveFileDialog)d).Filter = "waveファイル|*.wav";
                },
                CallBack = (d) =>
                {
                    using var fs = new FileStream(((SaveFileDialog)d).FileName, FileMode.Create);
                    using var bw = new BinaryWriter(fs);
                    bw.Write(waveData);
                }
            };

            OnShowCommonDialog(args);
            StausText = "";
            IsBusy = false;
        }

        public override void PostPrccess()
        {
            _MicSoundRecorder.WaveInClose();
        }
    }
}
