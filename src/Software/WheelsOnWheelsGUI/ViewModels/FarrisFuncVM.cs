
using Functions;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using static System.Math;

namespace ViewModel
{
    public class FarrisFuncVM : BaseViewModel
    {
        public RelayCommand cmdStartSim { get; private set; }
        public RelayCommand cmdStopSim { get; private set; }

        public List<List<DataPoint>> simdata { get; } = new List<List<DataPoint>>();


        public double simMax
        {
            get => _simMax;
            private set => setProperty(ref _simMax, value);
        }
        double _simMax;

        public double simMin
        {
            get => _simMin;
            private set => setProperty(ref _simMin, value);
        }
        double _simMin;

        public double ts
        {
            get => _ts;
            set
            {
                setProperty(ref _ts, value);
                simulate();
            }
        }
        double _ts = 1;

        public bool showWheels
        {
            get => _showWheels;
            set => setProperty(ref _showWheels, value);
        }
        bool _showWheels = false;

        public bool showGraph
        {
            get => _showGraph;
            set => setProperty(ref _showGraph, value);
        }
        bool _showGraph = true;


        DispatcherTimer timer;// = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(25) };

        private void doStartSim(object o)
        {
            if (ts >= 1.0)
            {
                ts = 0;
            }

            timer.Start();
        }


        private void doStopSim(object o)
        {
            timer.Stop();
        }


        private void simulate()
        {
            double x0 = 0;
            double y0 = 0;

            simdata.Clear();

            simMax = func.wheels.Sum(w => w.r) * 1.1;
            simMin = -simMax;

            foreach (var wheel in func.wheels)
            {
                var data = new List<DataPoint>();

                double dt = 0.01 / Abs(wheel.freq);
                double tmax = 1.0 / Abs(wheel.freq) + dt;

                for (double t = 0.0; t <= tmax; t += dt)
                {
                    var val = wheel.value(t);
                    data.Add(new DataPoint(x0 + val.x, y0 + val.y));
                }

                simdata.Add(data);

                var v = wheel.value(ts);
                x0 = x0 + v.x;
                y0 = y0 + v.y;

                simdata.Add(new List<DataPoint>() { new DataPoint(x0, y0) });
            }


            var graphData = new List<DataPoint>();

            foreach (var (x, y) in func.calculate(0.0, ts, 0.001))
            {
                graphData.Add(new DataPoint(x, y));
            }
            simdata.Add(graphData);

            onPropertyChanged("simData");
        }

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

            cmdStartSim = new RelayCommand(doStartSim);
            cmdStopSim = new RelayCommand(doStopSim);

            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(20), IsEnabled = false };
            timer.Tick += (s, e) =>
            {
                double dt = 0.002;
                ts += dt;
                if (ts > 1 + dt)
                {
                    timer.Stop();
                }
            };


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
            //onPropertyChanged("paramString");
            //onPropertyChanged("values");

            simulate();
        }

        private List<DataPoint> values { get; }
        private readonly FarrisFunc func = new FarrisFunc();
        private uint _symmetry = 6;
        private uint _k = 1;
    }
}
