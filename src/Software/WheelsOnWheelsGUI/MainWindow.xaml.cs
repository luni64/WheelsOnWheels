using System;
using System.Collections.Generic;
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
using ViewModel;

namespace WheelsOverWheels
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            (DataContext as MainVM).farrisFuncVM.PropertyChanged += FarrisFuncVM_PropertyChanged;
        }

        private void FarrisFuncVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "values":
                    //plot.InvalidatePlot(true);
                    break;
                case "simData":
                    simPlot.InvalidatePlot(true);
                    break;
            }
        }

        private void Close_Closed(object sender, EventArgs e)
        {
            (DataContext as MainVM)?.Dispose();
        }



    }
}
