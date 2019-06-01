
using OxyPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WheelsOnWheelsLib;

namespace ViewModel
{
    class MainVM : BaseViewModel, IDisposable
    {
        public string title => "Farris Viewer V0.1 - lunOptics";

        public RelayCommand cmdStart { get; private set; }
        public RelayCommand cmdStop { get; private set; }

        public FarrisFuncVM farrisFuncVM { get; }
        public List<DataPoint> functionValues { get; } = new List<DataPoint>();
        public List<string> responses { get; } = new List<string>();
      
        public MainVM()
        {
            cmdStop = new RelayCommand((o)=>iFace.cmdStop(), o => iFace.isOpen);
            cmdStart = new RelayCommand((o)=>iFace.cmdStart(farrisFuncVM.paramString), o => iFace.isOpen);

            farrisFuncVM = new FarrisFuncVM(functionValues);

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))   // avoid to open a port in the XAML designer! It will never release it...
            {
                iFace = new IFace();
                iFace.connectedTeensyChanged += iFace_ConnectedTeensyChanged;
                iFace.newResponse += iFace_newResponse;
            }
        }

        private void iFace_newResponse(object sender, string e)
        {
            Console.WriteLine(e);            
        }

        private void iFace_ConnectedTeensyChanged(object sender, EventArgs e)
        {
            onPropertyChanged("connectedTeensy");
            Application.Current.Dispatcher.Invoke(() => CommandManager.InvalidateRequerySuggested());           
        }

        public string connectedTeensy => iFace?.teensyID;
        public void Dispose() => iFace?.Dispose();

        //private void doStart(object o) => iFace.cmdStart("");
        //private void doStop(object o) => iFace.cmdStop();
        private readonly IFace iFace;
    }
}
