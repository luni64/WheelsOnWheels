using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace TeensySharp
{
    // From  https://github.com/luni64/TeensySharp

    public class TeensyWatcher : IDisposable
    { 
        const uint vid = 0x16C0;
        const uint serPid = 0x483;
        const uint halfKayPid = 0x478;
        readonly string vidStr = "'%USB_VID[_]"  + vid.ToString("X") + "%'";

        #region Properties and Events -----------------------------------------------

        public List<USB_Device> connectedDevices { get; private set; }
        public event EventHandler<ConnectionChangedEventArgs> connectionChanged;

        #endregion

        #region Construction / Destruction ------------------------------------------

        public TeensyWatcher()
        {
            connectedDevices = new List<USB_Device>();

            using (var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE " + vidStr))
            {
                foreach (var mgmtObject in searcher.Get())
                {
                    var device = makeDevice(mgmtObject);
                    if (device != null)
                    {
                        connectedDevices.Add(device);
                    }
                }
            }
            startWatching();
        }

        public void Dispose()
        {
            stopWatching();
        }

        #endregion

        #region Port Watching  ------------------------------------------------------

        protected ManagementEventWatcher createWatcher = null;
        protected ManagementEventWatcher deleteWatcher = null;

        protected void startWatching()
        {
            stopWatching(); // Just to make sure 

            deleteWatcher = new ManagementEventWatcher
            {
                Query = new WqlEventQuery
                {
                    EventClassName = "__InstanceDeletionEvent",
                    Condition = "TargetInstance ISA 'Win32_PnPEntity'",
                    WithinInterval = new TimeSpan(0, 0, 1), //Todo: make the interval settable
                },
            };
            deleteWatcher.EventArrived += portsChanged;
            deleteWatcher.Start();

            createWatcher = new ManagementEventWatcher
            {
                Query = new WqlEventQuery
                {
                    EventClassName = "__InstanceCreationEvent",
                    Condition = "TargetInstance ISA 'Win32_PnPEntity'",
                    WithinInterval = new TimeSpan(0, 0, 1), //Todo: make the interval settable
                },
            };
            createWatcher.EventArrived += portsChanged;
            createWatcher.Start();
        }

        protected void stopWatching()
        {
            if (createWatcher != null)
            {
                createWatcher.Stop();
                createWatcher.Dispose();
            }
            if (deleteWatcher != null)
            {
                deleteWatcher.Stop();
                deleteWatcher.Dispose();
            }
        }

        public enum ChangeType
        {
            add,
            remove
        }

        void portsChanged(object sender, EventArrivedEventArgs e)
        {
            var device = makeDevice((ManagementBaseObject)e.NewEvent["TargetInstance"]);
            if (device != null)
            {
                ChangeType type = e.NewEvent.ClassPath.ClassName == "__InstanceCreationEvent" ? ChangeType.add : ChangeType.remove;

                if (type == ChangeType.add)
                {
                    connectedDevices.Add(device);
                    onConnectionChanged(type, device);
                }
                else
                {
                    var rd = connectedDevices.Find(d => d.serialnumber == device.serialnumber);
                    connectedDevices.Remove(rd);
                    onConnectionChanged(type, rd);
                }
            }
        }

        #endregion

        #region Helpers

        protected USB_Device makeDevice(ManagementBaseObject mgmtObj)
        {
            var DeviceIdParts = ((string)mgmtObj["PNPDeviceID"]).Split("\\".ToArray());

            if (DeviceIdParts[0] != "USB") return null;

            int start = DeviceIdParts[1].IndexOf("PID_") + 4;
            uint pid = Convert.ToUInt32(DeviceIdParts[1].Substring(start, 4), 16);

            if (pid == serPid)
            {
                uint serNum = Convert.ToUInt32(DeviceIdParts[2]);
                string port = (((string)mgmtObj["Caption"]).Split("()".ToArray()))[1];

                return new USB_Device
                {
                    type = USB_Device.Type.UsbSerial,
                    port = port,
                    serialnumber = serNum
                };
            }
            else if (pid == halfKayPid)
            {
                uint serNum = Convert.ToUInt32(DeviceIdParts[2], 16);
                if (serNum != 0xFFFFFFFF)
                {
                    serNum *= 10;
                }

                return new USB_Device
                {
                    type = USB_Device.Type.HalfKay,
                    port = "",
                    serialnumber = serNum,
                };
            }
            return null;
        }

        #endregion

        #region EventHandler --------------------------------------------------------

        protected void onConnectionChanged(ChangeType type, USB_Device changedDevice)
        {
            connectionChanged?.Invoke(this, new ConnectionChangedEventArgs(type, changedDevice));
        }

        #endregion
    }

    public class USB_Device
    {
        public enum Type
        {
            UsbSerial,
            HalfKay, 
            HID,
            //...
        }

        public Type type;
        public uint serialnumber { get; set; }
        public string port { get; set; }
    }

    public class ConnectionChangedEventArgs : EventArgs
    {
        public readonly TeensyWatcher.ChangeType changeType;
        public readonly USB_Device changedDevice;

        public ConnectionChangedEventArgs(TeensyWatcher.ChangeType type, USB_Device changedDevice)
        {
            this.changeType = type;
            this.changedDevice = changedDevice;
        }
    }
}



