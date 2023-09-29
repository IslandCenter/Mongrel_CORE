using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongrelGUI.ViewModels;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace MongrelGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainWindowViewModel.GetInstance;
        }

        private new void PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private new void DragOver(object sender, DragEventArgs e)
        {

            if (DataContext is MainWindowViewModel viewModel)
            {
                //viewModel._parent.SelectItem(viewModel);
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effects = DragDropEffects.Copy;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
            }

            e.Handled = true;
        }
        private new void DropIn(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (docPath.Length > 1)
                {
                    MessageBox.Show("Multiple directories were dropped into a field that only accepts one.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!System.IO.Directory.Exists(docPath[0]))
                {
                    MessageBox.Show($"Could not find the directory {docPath[0]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    ((MainWindowViewModel)DataContext).DropPathIn(docPath[0]);
                }
                catch (NullReferenceException ex)
                {
                    throw new ArgumentException("DataContext not MainMenu", ex);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var docPath = (string)e.Data.GetData(DataFormats.Text);

                try
                {
                    ((MainWindowViewModel)DataContext).DropPathIn(docPath);
                }
                catch (NullReferenceException ex)
                {
                    throw new ArgumentException("DataContext not InputDirectoryViewModel", ex);
                }
            }
        }

        private new void DropOut(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var docPath = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (docPath.Length > 1)
                {
                    MessageBox.Show("Multiple directories were dropped into a field that only accepts one.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!System.IO.Directory.Exists(docPath[0]))
                {
                    MessageBox.Show($"Could not find the directory {docPath[0]}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    ((MainWindowViewModel)DataContext).DropPathOut(docPath[0]);
                }
                catch (NullReferenceException ex)
                {
                    throw new ArgumentException("DataContext not MainMenu", ex);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                var docPath = (string)e.Data.GetData(DataFormats.Text);

                try
                {
                    ((MainWindowViewModel)DataContext).DropPathOut(docPath);
                }
                catch (NullReferenceException ex)
                {
                    throw new ArgumentException("DataContext not InputDirectoryViewModel", ex);
                }
            }
        }
    }
}
