using Functions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class WheelParamVM : BaseViewModel
    {
        public WheelParamVM(FarrisWheel wheel)
        {
            this.wheel = wheel;
        }

        public int freq
        {
            get => wheel.freq;
            set
            {
                wheel.freq = value;
                onPropertyChanged();
            }
        }
        public double r
        {
            get => wheel.r;
            set
            {
                wheel.r = value;
                onPropertyChanged();
            }
        }
        public double phi0
        {
            get => wheel.phi0 / Math.PI * 180.0;
            set
            {
                wheel.phi0 = value / 180.0 * Math.PI;
                onPropertyChanged();
            }
        }
        public ObservableCollection<int> frequencies { get; } = new ObservableCollection<int>();

        public string name => wheel.name;

        private FarrisWheel wheel;
    }
}
