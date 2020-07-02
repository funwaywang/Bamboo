using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bamboo
{
    public class ImageFrame : DependencyObject
    {
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof(Width), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty ColorCountProperty = DependencyProperty.Register(nameof(ColorCount), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty DataSizeProperty = DependencyProperty.Register(nameof(DataSize), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(nameof(Offset), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty ColorPlanesProperty = DependencyProperty.Register(nameof(ColorPlanes), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty PixelBitsProperty = DependencyProperty.Register(nameof(PixelBits), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty HotspotXProperty = DependencyProperty.Register(nameof(HotspotX), typeof(int), typeof(ImageFrame));
        public static readonly DependencyProperty HotspotYProperty = DependencyProperty.Register(nameof(HotspotY), typeof(int), typeof(ImageFrame));

        public int Width
        {
            get => (int)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public int ColorCount
        {
            get => (int)GetValue(ColorCountProperty);
            set => SetValue(ColorCountProperty, value);
        }

        public int ColorPlanes
        {
            get => (int)GetValue(ColorPlanesProperty);
            set => SetValue(ColorPlanesProperty, value);
        }

        public int PixelBits
        {
            get => (int)GetValue(PixelBitsProperty);
            set => SetValue(PixelBitsProperty, value);
        }

        public int DataSize
        {
            get => (int)GetValue(DataSizeProperty);
            set => SetValue(DataSizeProperty, value);
        }

        public int Offset
        {
            get => (int)GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public int HotspotX
        {
            get => (int)GetValue(HotspotXProperty);
            set => SetValue(HotspotXProperty, value);
        }

        public int HotspotY
        {
            get => (int)GetValue(HotspotYProperty);
            set => SetValue(HotspotYProperty, value);
        }
    }
}
