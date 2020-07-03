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
    public partial class MainWindow : Window
    {
        private OpenFileDialog openFileDialog;
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register(nameof(ActiveDocument), typeof(Document), typeof(MainWindow));
        public static readonly DependencyProperty DocumentViewProperty = DependencyProperty.Register(nameof(DocumentView), typeof(DocumentView), typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();

            DocumentView = new DocumentView();
        }

        public Document ActiveDocument
        {
            get => (Document)GetValue(ActiveDocumentProperty);
            set => SetValue(ActiveDocumentProperty, value);
        }

        public DocumentView DocumentView
        {
            get => (DocumentView)GetValue(DocumentViewProperty);
            set => SetValue(DocumentViewProperty, value);
        }

        private void Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string filename)
            {
                OpenFileAsync(filename);
            }
            else
            {
                if (openFileDialog == null)
                {
                    openFileDialog = new OpenFileDialog
                    {
                        Filter = $"All Supported Files (*.ico;*.cur)|*.ico;*.cur|Icon File (*.ico)|*.ico|Cursor File (*.cur)|*.cur"
                    };
                }

                openFileDialog.Title = "Open…";
                if (openFileDialog.ShowDialog(this) == true && openFileDialog.FileNames != null)
                {
                    OpenFileAsync(openFileDialog.FileName);
                }
            }
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentView != null;
        }

        private async void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentView != null)
            {
                try
                {
                    await DocumentView.SaveAsync();
                }
                catch (Exception ex)
                {
                    this.ShowMessageBox(ex);
                }
            }
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DocumentView != null;
        }

        private async void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DocumentView != null)
            {
                try
                {
                    await DocumentView.SaveAsAsync();
                }
                catch (Exception ex)
                {
                    this.ShowMessageBox(ex);
                }
            }
        }

        public async void OpenFileAsync(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            try
            {
                if (!File.Exists(filename))
                {
                    throw new Exception("The specified file does not exist." + "\n" + filename);
                }

                if (ActiveDocument != null && string.Equals(ActiveDocument.FileName, filename, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                var document = await Document.LoadAsync(filename);
                if (document != null)
                {
                    document.RebuildThumbs();
                    DocumentView.Document = document;
                    Settings.Default.AddRecentFile(new FileSummary(document));
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageBox(ex);
            }
        }
    }
}
