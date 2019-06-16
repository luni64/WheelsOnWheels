using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using TeensySharp;

namespace WheelsOnWheelsLib
{
    public class IFace : IDisposable
    {
        public string teensyID { get; private set; }        // ID of the found Teensy (serial number and com port) 
        public bool isOpen => port != null && port.IsOpen;  // Do we have an open connection to the Teensy?

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
                port?.Dispose();                     // dispose non managed resources! Otherwise the port will be kept even after closing the app
            }
        }

        // Sends function parameters and starts drawing 
        public void cmdStart(string param)
        {
            if (port != null && port.IsOpen)
            {
                port.Write($"farris {param}\n");
                port.Write("start\n");
            }
        }

        // stops the drawing
        public void cmdStop()
        {
            if (port != null && port.IsOpen)
            {
                port.Write("stop\n");
            }
        }

        // Inform listners that a Teensy was attached or removed from USB bus
        public event EventHandler connectedTeensyChanged;
        public event EventHandler<string> newResponse;      // Inform listeners that a a new response was received 


        // called whenever a Teensy is added / removed from the USB bus
        private void connectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            var teensy = watcher.connectedDevices.FirstOrDefault();  // use first teensy found
            if (teensy != null)
            {
                try
                {
                    teensyID = $"Teensy {teensy.serialnumber} on {teensy.port} ";
                    if (port != null)
                    {
                        port.DataReceived -= dataReceived; // detatch incomming data handler
                        port.Dispose();                    // get rid of old port
                    }
                    port = new SerialPort(teensy.port);
                    port.DataReceived += dataReceived;  // attach handler for incoming data
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
            connectedTeensyChanged?.Invoke(this, null); // inform listeners about the change
        }

        // handle incoming data
        private void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int b;
            while ((b = port.ReadByte()) >= 0)
            {
                if (b == '\n')
                {
                    newResponse?.Invoke(this, responseLine.ToString());  // inform listeners about the new response
                    responseLine.Clear();
                    break;
                }
                responseLine.Append((char)b);
            }
        }
                
        SerialPort port;
        protected TeensyWatcher watcher;
        StringBuilder responseLine = new StringBuilder();
    }
}
