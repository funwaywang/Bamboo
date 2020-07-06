using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bamboo
{
    public partial class DocumentView : UserControl
    {
        private SaveFileDialog saveFileDialog;
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register(nameof(Document), typeof(Document), typeof(DocumentView));
        public static readonly DependencyProperty SelectedFrameProperty = DependencyProperty.Register(nameof(SelectedFrame), typeof(ImageFrame), typeof(DocumentView));

        public DocumentView()
        {
            InitializeComponent();

            Document = new Document
            {
                FileName = "New Icon.ico",
                Type = DocumentType.Icon
            };
        }

        public Document Document
        {
            get => (Document)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public ImageFrame SelectedFrame
        {
            get => (ImageFrame)GetValue(SelectedFrameProperty);
            set => SetValue(SelectedFrameProperty, value);
        }

        public Task<bool> SaveAsync()
        {
            return SaveAsAsync();
        }

        public async Task<bool> SaveAsAsync()
        {
            if (Document == null)
            {
                return false;
            }

            if (saveFileDialog == null)
            {
                saveFileDialog = new SaveFileDialog
                {
                    Filter = $"Icon File (*.ico)|*.ico|Cursor File (*.cur)|*.cur"
                };
            }

            saveFileDialog.FilterIndex = Document.Type == DocumentType.Icon ? 1 : 2;
            saveFileDialog.FileName = Document.FileName;
            if (saveFileDialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                try
                {
                    var documentType = Document.Type;
                    var filename = saveFileDialog.FileName;
                    var extension = System.IO.Path.GetExtension(filename).ToLower();
                    if (extension == ".ico")
                    {
                        documentType = DocumentType.Icon;
                    }
                    else if (extension == ".cur")
                    {
                        documentType = DocumentType.Cursor;
                    }
                    else if (saveFileDialog.FilterIndex == 2)
                    {
                        documentType = DocumentType.Cursor;
                    }
                    else if (saveFileDialog.FilterIndex == 1)
                    {
                        documentType = DocumentType.Icon;
                    }

                    await Document.SaveAsync(filename, documentType);
                    Settings.Default.AddRecentFile(new FileSummary(Document));

                    return true;
                }
                catch (Exception ex)
                {
                    this.ShowMessageBox(ex);
                }
            }

            return false;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == DocumentProperty)
            {
                SelectedFrame = (e.NewValue as Document)?.Frames.FirstOrDefault();
            }
        }
    }
}
