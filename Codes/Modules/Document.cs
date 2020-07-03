using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bamboo
{
    public class Document : INotifyPropertyChanged
    {
        private string _FileName;
        private DocumentType _Type;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public DocumentType Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        public ObservableCollection<ImageFrame> Frames { get; private set; } = new ObservableCollection<ImageFrame>();

        public static Task<Document> LoadAsync(string filename)
        {
            return Task.Factory.StartNew(() => Load(filename));
        }

        public static Document Load(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var document = Load(stream);
                if (document != null)
                {
                    document.FileName = filename;
                }

                return document;
            }
        }

        public static Document Load(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                if (reader.ReadInt16() != 0)
                {
                    throw new InvalidFileFormatException();
                }

                var typeValue = reader.ReadInt16();
                DocumentType documentType;
                switch (typeValue)
                {
                    case 1:
                        documentType = DocumentType.Icon;
                        break;
                    case 2:
                        documentType = DocumentType.Cursor;
                        break;
                    default:
                        throw new InvalidFileFormatException();
                }

                var document = new Document
                {
                    Type = documentType
                };

                var imagesCount = reader.ReadInt16();
                for (int i = 0; i < imagesCount; i++)
                {
                    var frame = new ImageFrame
                    {
                        Width = reader.ReadByte(),
                        Height = reader.ReadByte(),
                        ColorCount = reader.ReadByte()
                    };

                    if (frame.Width == 0)
                    {
                        frame.Width = 256;
                    }
                    if (frame.Height == 0)
                    {
                        frame.Height = 256;
                    }

                    reader.ReadByte(); // reserved, should be 0

                    if (documentType == DocumentType.Icon)
                    {
                        frame.ColorPlanes = reader.ReadInt16();
                        frame.PixelBits = reader.ReadInt16();
                    }
                    else
                    {
                        frame.HotspotX = reader.ReadInt16();
                        frame.HotspotY = reader.ReadInt16();
                    }
                    frame.DataSize = reader.ReadInt32();
                    frame.OffsetInFile = reader.ReadInt32();

                    document.Frames.Add(frame);
                }

                foreach (var frame in document.Frames)
                {
                    if (frame.Width == 256)
                    {
                        frame.RawData = IconIOHelper.LoadPngIconFrame(stream, frame.OffsetInFile, frame.DataSize);
                        if (frame.RawData != null)
                        {
                            frame.Width = frame.RawData.Width;
                            frame.Height = frame.RawData.Height;
                        }
                    }
                    else
                    {
                        frame.RawData = IconIOHelper.LoadBmpIconFrame(stream, frame.OffsetInFile, frame.DataSize);
                    }
                    //frame.RebuildImageSource();
                }

                return document;
            }
        }

        public async Task SaveAsync(string filename, DocumentType documentType)
        {
            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                await SaveAsync(stream, documentType);
                FileName = filename;
                Type = documentType;
            }
        }

        public Task SaveAsync(Stream stream, DocumentType documentType)
        {
            return Task.CompletedTask;
        }

        public void RebuildThumbs()
        {
            foreach(var frame in Frames)
            {
                frame.RebuildThumb();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
