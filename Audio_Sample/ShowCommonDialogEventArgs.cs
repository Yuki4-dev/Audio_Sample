using Microsoft.Win32;
using System;

namespace Audio_Sample
{
    public class ShowCommonDialogEventArgs : EventArgs
    {
        public bool IsModal { get; set; } = false;
        public bool IsCancel { get; set; } = false;
        public Action<CommonDialog> PreparationDialog { get; set; }
        public Action<CommonDialog> CallBack { get; set; }
        public Type DialogType { get; }

        public ShowCommonDialogEventArgs(Type dialogType)
        {
            if (!dialogType.IsSubclassOf(typeof(CommonDialog)))
            {
                throw new ArgumentException(nameof(dialogType));
            }

            DialogType = dialogType;
        }
    }
}
