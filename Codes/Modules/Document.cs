using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bamboo
{
    public class Document : DependencyObject
    {
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register(nameof(FileName), typeof(string), typeof(Document));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(DocumentType), typeof(Document));

        public string FileName
        {
            get => (string)GetValue(FileNameProperty);
            set => SetValue(FileNameProperty, value);
        }

        public DocumentType Type
        {
            get => (DocumentType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public ObservableCollection<ImageFrame> Frames { get; private set; } = new ObservableCollection<ImageFrame>();

        public static async Task<Document> LoadAsync(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                var document = await LoadAsync(stream);
                if (document != null)
                {
                    document.FileName = filename;
                }

                return document;
            }
        }

        public static Task<Document> LoadAsync(Stream stream)
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
                    frame.Offset = reader.ReadInt32();

                    document.Frames.Add(frame);
                }

                return Task.FromResult(document);
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
    }
}
