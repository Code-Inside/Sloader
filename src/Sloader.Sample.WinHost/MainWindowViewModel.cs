using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Sloader.Crawler;
using Sloader.Crawler.Config;
using Sloader.Sample.WinHost.Annotations;

namespace Sloader.Sample.WinHost
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _output;
        private string _input;

        public ICommand StartSloaderCommand
        {
            get;
            private set;
        }

        private bool CanStartSloaderCommand()
        {
            return true;
        }

        public async void StartSloader()
        {
            SloaderRunner = new SloaderRunner(SloaderConfigLoader.Parse(this.Input, new Dictionary<string,string>()));
            var result = await SloaderRunner.RunAllCrawlers();

            Output = result.ToJson();
        }

        public SloaderRunner SloaderRunner { get; set; }

        public MainWindowViewModel()
        {
            StartSloaderCommand = new RelayCommand(StartSloader,CanStartSloaderCommand);
        }

        public string Input
        {
            get { return _input; }
            set
            {
                _input = value;
                OnPropertyChanged();
            }
        }

        public string Output
        {
            get { return _output; }
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        private Action methodToExecute;
        private Func<bool> canExecuteEvaluator;
        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            this.methodToExecute = methodToExecute;
            this.canExecuteEvaluator = canExecuteEvaluator;
        }
        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        {
        }
        public bool CanExecute(object parameter)
        {
            if (this.canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                bool result = this.canExecuteEvaluator.Invoke();
                return result;
            }
        }
        public void Execute(object parameter)
        {
            this.methodToExecute.Invoke();
        }
    }
}
