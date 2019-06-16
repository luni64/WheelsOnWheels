using static System.Math;

namespace Functions
{
    public class FarrisWheel 
    {
        public FarrisWheel(string name)
        {
            this.name = name;
        }
        
        public string name { get; private set; }
        public double r { get; set; } = 1;
        public double phi0 { get; set; } = 0;
        public int freq { get; set; } = 1;
        
        public (double x, double y) value(double t)
        {
            double x = r * Cos(2 * PI * freq * t + phi0);
            double y = r * Sin(2 * PI * freq * t + phi0);
            return (x, y);
        }
    }
}
