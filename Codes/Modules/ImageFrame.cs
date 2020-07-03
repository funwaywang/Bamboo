using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bamboo
{
    public class ImageFrame : INotifyPropertyChanged
    {
        private int _OffsetInFile;
        private int _Width;
        private int _Height;
        private int _ColorCount;
        private int _ColorPlanes;
        private int _PixelBits;
        private int _DataSize;
        private int _HotspotX;
        private int _HotspotY;
        private BitmapData _RawData;
        private BitmapSource _Thumb;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Width
        {
            get => _Width;
            set
            {
                if (_Width != value)
                {
                    _Width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public int Height
        {
            get => _Height;
            set
            {
                if (_Height != value)
                {
                    _Height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public int ColorCount
        {
            get => _ColorCount;
            set
            {
                if (_ColorCount != value)
                {
                    _ColorCount = value;
                    OnPropertyChanged(nameof(ColorCount));
                }
            }
        }

        public int ColorPlanes
        {
            get => _ColorPlanes;
            set
            {
                if (_ColorPlanes != value)
                {
                    _ColorPlanes = value;
                    OnPropertyChanged(nameof(ColorPlanes));
                }
            }
        }

        public int PixelBits
        {
            get => _PixelBits;
            set
            {
                if (_PixelBits != value)
                {
                    _PixelBits = value;
                    OnPropertyChanged(nameof(PixelBits));
                }
            }
        }

        public int DataSize
        {
            get => _DataSize;
            set
            {
                if (_DataSize != value)
                {
                    _DataSize = value;
                    OnPropertyChanged(nameof(DataSize));
                }
            }
        }

        public int HotspotX
        {
            get => _HotspotX;
            set
            {
                if (_HotspotX != value)
                {
                    _HotspotX = value;
                    OnPropertyChanged(nameof(HotspotX));
                }
            }
        }

        public int HotspotY
        {
            get => _HotspotY;
            set
            {
                if (_HotspotY != value)
                {
                    _HotspotY = value;
                    OnPropertyChanged(nameof(HotspotY));
                }
            }
        }

        public int OffsetInFile
        {
            get => _OffsetInFile;
            set
            {
                if (_OffsetInFile != value)
                {
                    _OffsetInFile = value;
                    OnPropertyChanged(nameof(OffsetInFile));
                }
            }
        }

        public BitmapData RawData
        {
            get => _RawData;
            set
            {
                if (_RawData != value)
                {
                    _RawData = value;
                    OnPropertyChanged(nameof(RawData));
                }
            }
        }

        public BitmapSource Thumb
        {
            get => _Thumb;
            set
            {
                if (_Thumb != value)
                {
                    _Thumb = value;
                    OnPropertyChanged(nameof(Thumb));
                }
            }
        }

        public void RebuildThumb()
        {
            if (RawData == null)
            {
                Thumb = null;
            }
            else
            {
                Thumb = BitmapSource.Create(RawData.Width, RawData.Height, 96, 96, PixelFormats.Bgra32, null, RawData.BitData, RawData.Stride);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
