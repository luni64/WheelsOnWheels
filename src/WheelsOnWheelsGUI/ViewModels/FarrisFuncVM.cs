
using Functions;
using OxyPlot;
using System.Collections.Generic;
using static System.Math;

namespace ViewModel
{
    public class FarrisFuncVM : BaseViewModel
    {
        public uint symmetry
        {
            get => _symmetry;
            set
            {
                setProperty(ref _symmetry, value);
                setSymmetry(_symmetry, k);
            }
        }

        public uint k
        {
            get => _k;
            set
            {
                setProperty(ref _k, value);
                setSymmetry(_symmetry, k);
            }
        }

        public List<WheelParamVM> wheelParamVMs { get; } = new List<WheelParamVM>();
        public string paramString => func.getParamString();
        
        public FarrisFuncVM(List<DataPoint> functionValues)
        {
            this.values = functionValues;

            foreach (var wheel in func.wheels)
            {
                var wheelParamVM = new WheelParamVM(wheel);
                wheelParamVM.PropertyChanged += (s, e) => calculate();  // recalculate if parameters changed
                wheelParamVMs.Add(wheelParamVM);
            }

            setSymmetry(symmetry, k);
            calculate();
        }
                
        public double max
        {
            get => _max;
            set => setProperty(ref _max, value);
        }
        double _max;

        public double min
        {
            get => _min;
            set => setProperty(ref _min, value);
        }
        double _min;
                
        private void setSymmetry(uint p, uint k)
        {
            foreach (var wheelParamVM in wheelParamVMs)
            {
                wheelParamVM.frequencies.Clear();
                for (int i = -7; i <= 7; i++)
                {
                    wheelParamVM.frequencies.Add((int)(k + p * i));
                }
            }
        }

        private void calculate()
        {
            double dt = 0.001;
            double r_max = 0;

            values.Clear();

            foreach (var (x, y) in func.calculate(0.0, 1.0 + dt, dt))
            {
                values.Add(new DataPoint(x, y));

                double s = x * x + y * y;
                if (r_max < s) r_max = s;
            }

            max = Sqrt(r_max) * 1.1;
            min = -max;

            onPropertyChanged("paramString");
            onPropertyChanged("values");
        }

        private List<DataPoint> values { get; }
        private readonly FarrisFunc func = new FarrisFunc();
        private uint _symmetry = 6;
        private uint _k = 1;
    }
}
