using System;
using System.Collections.Generic;
using System.Text;

namespace Functions
{
    public class FarrisFunc
    {
        public IReadOnlyList<FarrisWheel> wheels { get; } = new List<FarrisWheel>()
        {
            new FarrisWheel("Wheel 1") { freq = 1, r = 1 },
            new FarrisWheel("Wheel 2") { freq = 7, r = 0.5 },
            new FarrisWheel("Wheel 3") { freq = -17, r = 0.33, phi0 = -90 },
            new FarrisWheel("Wheel 4") { freq = 19, r = 0 }
        };

        public List<(double x, double y)> calculate(double t_0, double t_e, double dt)
        {
            if (t_e <= t_0 || dt <= 0) throw (new ArgumentException());

            var result = new List<(double x, double y)>();
            for (double t = t_0; t < t_e; t += dt)
            {
                result.Add(value(t));
            }
            return result;
        }

        protected (double x, double y) value(double t)
        {
            (double x, double y) result = (0, 0);

            foreach (var wheel in wheels)
            {
                var (x, y) = wheel.value(t);

                result.x += x;
                result.y += y;
            }
            return result;
        }

        public string getParamString()
        {
            var p = new StringBuilder();
            foreach (var wheel in wheels)
            {
                p.Append($"{wheel.freq} {wheel.r:0.00} {wheel.phi0:0.00} ");
            }
            return p.ToString();
        }
    }
}


