using System;
using System.ComponentModel;
using System.Windows;
using WheelsOnWheelsLib;


namespace ViewModel
{
    class IFaceVM : BaseViewModel, IDisposable
    {
        public RelayCommand cmdStart { get; }
        public RelayCommand cmdStop { get; }        

        public string teensyID => iFace?.teensyID;

        public IFaceVM()
        {
            cmdStart = new RelayCommand((o)=>iFace.cmdStart(cmd), o => iFace.isOpen);
            cmdStop = new RelayCommand((o)=>iFace.cmdStop(), o => iFace.isOpen);

            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                return;  // avoid to open a port in the XAML designer! It will never release it...
            }
            iFace = new IFace();
        }

        public void Dispose()
        {
            iFace?.Dispose();
            iFace = null;
        }
        
        public string cmd
        {
            get => _cmd;
            set => setProperty(ref _cmd, value);
        }
        private string _cmd;

        private IFace iFace;
    }
}
