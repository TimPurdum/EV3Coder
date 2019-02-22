using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoreBluetooth;
using CoreFoundation;
using EV3Coder.XamarinController.iOS;
using ExternalAccessory;
using Foundation;
using Lego.Ev3.Core;
using Xamarin.Forms;

[assembly: Dependency(typeof(IOsBluetoothCommunication))]
namespace EV3Coder.XamarinController.iOS
{
    public class IOsBluetoothCommunication: NSStreamDelegate, IXamarinCommunication
    {
        EAAccessoryManager _manager;
        EASession _session;
        private NSMutableData _readData;

        public event EventHandler<ReportReceivedEventArgs> ReportReceived;


        public IOsBluetoothCommunication()
        {
            _manager = EAAccessoryManager.SharedAccessoryManager;
        }
        public Task ConnectAsync()
        {
            return Task.Run(() =>
            {
                SelectedDevice.WeakDelegate = this;
                _session = new EASession(SelectedDevice, "COM.LEGO.MINDSTORMS.EV3");
                
                _session.InputStream.Delegate = this;
                _session.InputStream.Schedule(NSRunLoop.Current, NSRunLoopMode.Default);
                _session.InputStream.Open();

                _session.OutputStream.Delegate = this;
                _session.OutputStream.Schedule(NSRunLoop.Current, NSRunLoopMode.Default);
                _session.OutputStream.Open();
            });
        }
        

        public void Disconnect()
        {
            _session.InputStream.Unschedule(NSRunLoop.Current, NSRunLoopMode.Default);
            _session.InputStream.Delegate = null;
            _session.InputStream.Close();

            _session.OutputStream.Unschedule(NSRunLoop.Current, NSRunLoopMode.Default);
            _session.OutputStream.Delegate = null;
            _session.OutputStream.Close();

            _session = null;
        }

        public Task WriteAsync(byte[] data)
        {
            return Task.Run(() =>
            {
                NextCommand = data;
            });
        }

        /// <summary>
        /// Low level read method - read data while there is data and space in input buffer, then post notification to observer
        /// </summary>
        void ReadData()
        {

            nuint bufferSize = 128;
            byte[] buffer = new byte[bufferSize];

            while (_session.InputStream.HasBytesAvailable())
            {
                _session.InputStream.Read(buffer, bufferSize);
            }
            ReportReceived(this, new ReportReceivedEventArgs { Report = buffer });
        }


        /// <summary>
        /// Handle the events occurring with the external accessory
        /// </summary>
        /// <param name="theStream"></param>
        /// <param name="streamEvent"></param>
        public override void HandleEvent(NSStream theStream, NSStreamEvent streamEvent)
        {

            switch (streamEvent)
            {

                case NSStreamEvent.None:
                    Console.WriteLine("StreamEventNone");
                    break;
                case NSStreamEvent.HasBytesAvailable:
                    Console.WriteLine("StreamEventHasBytesAvailable");
                    ReadData();
                    break;
                case NSStreamEvent.HasSpaceAvailable:
                    Console.WriteLine("StreamEventHasSpaceAvailable");
                    WriteData();
                    break;
                case NSStreamEvent.OpenCompleted:
                    Console.WriteLine("StreamEventOpenCompleted");
                    break;
                case NSStreamEvent.ErrorOccurred:
                    Console.WriteLine("StreamEventErroOccurred");
                    break;
                case NSStreamEvent.EndEncountered:
                    Console.WriteLine("StreamEventEndEncountered");
                    break;
                default:
                    Console.WriteLine("Stream present but no event");
                    break;

            }
        }

        private void WriteData()
        {
            if (NextCommand != null)
            {
                _session.OutputStream.Write(NextCommand);
                NextCommand = null;
            }
        }

        public Task<string[]> GetDeviceList()
        {
            return Task.Run(() =>
            {
                return _manager.ConnectedAccessories.Select(a => a.Name).ToArray();
            });
        }

        public void SelectDevice(string name)
        {
            SelectedDevice = _manager.ConnectedAccessories.FirstOrDefault(a => a.Name == name);
        }

        public EAAccessory SelectedDevice { get; set; }
        public byte[] NextCommand { get; private set; }
    }
}