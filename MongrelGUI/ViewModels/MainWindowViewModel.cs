using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using WK.Libraries.BetterFolderBrowserNS;
using Yipper;
using static Mongrel.Mongrel;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace MongrelGUI.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel, ICommandSource
    {
        public ICommand Command { get; }
        public object CommandParameter { get; }
        public IInputElement CommandTarget { get; }

        private string _inputDir;
        private string _outputDir;
        private string _outputName;
        private string _reloadShow = "Hidden";
        private string _runShow = "Visible";

        private readonly string _logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MongrelGUILogs");
        private const LogLevel LogLevel = Yipper.LogLevel.Info;

        public void DropPathIn(string path)
        {
            InputDirText = path;
        }
        public void DropPathOut(string path)
        {
            OutputDirText = path;
        }

        public string TitleText
        {
            get
            {
                var versionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return $"Mongrel {versionNumber.Substring(0, versionNumber.LastIndexOf('.'))}";
            }
        }

        public string RunShow
        {
            get => _runShow;
            set
            {
                _runShow = value;
                OnPropertyChanged();
            }
        }

        public string ReloadShow
        {
            get => _reloadShow;
            set
            {
                _reloadShow = value;
                OnPropertyChanged();
            }
        }

        public string InputDirText
        {
            get => _inputDir;
            set
            {
                _inputDir = value;
                OnPropertyChanged();
            }
        }
        public string OutputDirText
        {
            get => _outputDir;
            set
            {
                _outputDir = value;
                OnPropertyChanged();
            }
        }
        public string OutputNameText
        {
            get => _outputName;
            set
            {
                _outputName = value;
                OnPropertyChanged();
            }
        }


        public ICommand inputCommand;
        public ICommand InputBrowseCommand =>
            inputCommand ?? (inputCommand = new RelayCommand(x =>
            {
                var folderBrowser = new BetterFolderBrowser();
                if (folderBrowser.ShowDialog() != DialogResult.OK) return;
                InputDirText = folderBrowser.SelectedFolder;
                OnPropertyChanged(nameof(InputDirText));

            }));
        public ICommand outputCommand;
        public ICommand OutputBrowseCommand =>
            outputCommand ?? (outputCommand = new RelayCommand(x =>
            {
                var folderBrowser = new BetterFolderBrowser();
                if (folderBrowser.ShowDialog() != DialogResult.OK) return;
                OutputDirText = folderBrowser.SelectedFolder;
                OnPropertyChanged(nameof(OutputDirText));

            }));

        public ICommand openOutputDirCommand;

        public ICommand OpenOutputDirCommand =>
            openOutputDirCommand ?? (openOutputDirCommand = new RelayCommand(x =>
            {
                if (string.IsNullOrEmpty(_outputDir))
                    MessageBox.Show("Output Directory box is empty, insert a value to use this button", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (!Directory.Exists(_outputDir))
                    MessageBox.Show("Output Directory not found or has not been created", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    System.Diagnostics.Process.Start(_outputDir);
            }));

        public ICommand openLogDirCommand;

        public ICommand OpenLogDirCommand =>
            openLogDirCommand ?? (openLogDirCommand = new RelayCommand(x =>
            {
                if (!Directory.Exists(_logDir)) Directory.CreateDirectory(_logDir);
                System.Diagnostics.Process.Start(_logDir);
            }));

        public ICommand runCommand;

        public ICommand RunCommand =>
            runCommand ?? (runCommand = new RelayCommand(x =>
            {

                if (string.IsNullOrEmpty(_inputDir) || string.IsNullOrEmpty(_outputDir))
                {
                    MessageBox.Show("Input and Output directories need to have values", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Create a new Task to run the RunMongrel function
                    var task = new Task(() => {
                        if (!Directory.Exists(_logDir)) Directory.CreateDirectory(_logDir);
                        var log = Logger.GetLogger();
                        Logger.SetLogger(log);
                        Logger.InitializeLogger(_logDir, LogLevel);

                        // Update the RunShow property on the GUI thread
                        Application.Current.Dispatcher.Invoke(() => {
                            RunShow = "Hidden";
                            OnPropertyChanged(nameof(RunShow));
                            ReloadShow = "Visible";
                            OnPropertyChanged(nameof(ReloadShow));
                        });

                        var startTime = TimeMeow();
                        try
                        {
                            Logger.Instance.Info($"Starting Mongrel run => inputDir: {_inputDir} outputDir: {_outputDir}");
                            // Call the RunMongrel function
                            RunMongrel(_inputDir, _outputDir, _outputName, log);

                            Logger.Instance.Info("Finished Mongrel run Successfully");

                            MessageBox.Show($"Mongrel has finished running {Environment.NewLine}{Environment.NewLine}" +
                                            $"Start time: {startTime}{Environment.NewLine}" +
                                            $"Stop time: {TimeMeow()}",
                                "Successful Run", MessageBoxButton.OK, MessageBoxImage.Information);

                        }
                        catch (Exception e)
                        {
                            Logger.Instance.Error($"Error in Mongrel run => Exception: {e}");

                            MessageBox.Show($"Error in Mongrel check logs for more info{Environment.NewLine}{Environment.NewLine}" +
                                            $"Start time: {startTime}{Environment.NewLine}" +
                                            $"Stop time: {TimeMeow()}{Environment.NewLine}{Environment.NewLine}" +
                                            $"Exception found: {e.Message}", "Unsuccessful Run",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        finally
                        {
                            Logger.Instance.Flush();
                        }

                        // Update the RunShow property on the GUI thread
                        Application.Current.Dispatcher.Invoke(() => {
                            RunShow = "Visible";
                            OnPropertyChanged(nameof(RunShow));
                            ReloadShow = "Hidden";
                            OnPropertyChanged(nameof(ReloadShow));
                        });
                    });

                    task.Start();
                }


            }));

        private static MainWindowViewModel _instance;
        public static MainWindowViewModel GetInstance => _instance ?? (_instance = new MainWindowViewModel());

        public static string TimeMeow() =>
            DateTime.Now.ToString("dddd, MMMM d, yyyy h:mm:ss tt");
    }
}
