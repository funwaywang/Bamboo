using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Bamboo
{
    public class FileSummary : INotifyPropertyChanged
    {
        private const int GeneralSamplesCount = 6;
        private string _FileName;
        private DateTime _UpdateTime;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileSummary()
        {
        }

        public FileSummary(Document document)
        {
            if (document != null)
            {
                FileName = document.FileName;
                UpdateTime = DateTime.Now;
            }
        }

        public string FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged(nameof(FileName));
                }
            }
        }

        public DateTime UpdateTime
        {
            get => _UpdateTime;
            set
            {
                if (_UpdateTime != value)
                {
                    _UpdateTime = value;
                    OnPropertyChanged(nameof(UpdateTime));
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
