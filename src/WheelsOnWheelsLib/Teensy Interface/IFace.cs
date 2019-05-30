using System;
using System.IO.Ports;
using System.Linq;
using TeensySharp;

namespace WheelsOnWheelsLib
{
    public class IFace : IDisposable
    {
        public event EventHandler connectedTeensyChanged;
        public string teensyID { get; private set; }
        public bool isOpen => port != null && port.IsOpen;
                        
        public IFace()
        {
            watcher = new TeensyWatcher();
            watcher.connectionChanged += connectionChanged;
            connectionChanged(null, null);
        }

        public void Dispose()
        {
            if (port != null)
            {
                port.DataReceived -= dataReceived;   // detach handler
                port?.Dispose();                     // dispose non managed resources 
            }
        }

        public void cmdStart(string param)
        {
            if (port != null && port.IsOpen)
            {
                port.Write(param + "\n");
                port.Write("start\n");
            }
        }

        public void cmdStop()
        {
            if (port != null && port.IsOpen)
            {
                port.Write("stop\n");
            }
        }
                
        private void connectionChanged(object sender, ConnectionChangedEventArgs e)  // Handles one teensy on the bus only. Can be extended easily        
        {
            var teensy = watcher.connectedDevices.FirstOrDefault();  // use first teensy found
            if (teensy != null)
            {
                try
                {
                    teensyID = $"Teensy {teensy.serialnumber} on {teensy.port} ";
                    if (port != null)
                    {
                        port.DataReceived -= dataReceived; // detatch handler
                        port.Dispose();                    // get rid of old port
                    }
                    port = new SerialPort(teensy.port);
                    port.DataReceived += dataReceived;     // attach handler for incoming data
                    port.Open();
                }
                catch
                {
                    teensyID = "Error opening Port";
                    port?.Dispose();
                    port = null; 
                }
            }
            else // no teensy on bus
            {
                if (port != null)
                {
                    port.DataReceived -= dataReceived;
                    port.Dispose();
                    port = null;
                }
                teensyID = "not found";
            }
            onTeensyChanged();
        }
        
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string curLine = "";
            int b = port.ReadByte();

            while (b >= 0)
            {
                if (b == '\n')
                {
                    var val = curLine.Split(' ');

                    if (double.TryParse(val[0], out double r) && double.TryParse(val[1], out double phi))
                    {
                        // motorPositions.Add(new OxyPlot.DataPoint(x,y));
                        // f.motorPoint = (r * Math.Cos(phi), r * Math.Sin(phi));
                    }                    
                    break;
                }
                curLine += (char)b;
                b = port.ReadByte();
            }
        }
        
        protected void onTeensyChanged() => connectedTeensyChanged?.Invoke(this, null);
        protected TeensyWatcher watcher;
        SerialPort port;
    }
}
