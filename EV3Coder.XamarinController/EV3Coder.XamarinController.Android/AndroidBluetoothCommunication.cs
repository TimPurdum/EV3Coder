using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using EV3Coder.XamarinController.Android;
using Java.Util;
using Lego.Ev3.Core;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBluetoothCommunication))]
namespace EV3Coder.XamarinController.Android
{
    public class AndroidBluetoothCommunication: IXamarinCommunication
    {
        readonly BluetoothAdapter _adapter;
        readonly UUID _myUuid;
        BluetoothSocket _socket;
        Handler _handler;
        
        public event EventHandler<ReportReceivedEventArgs> ReportReceived;

        public AndroidBluetoothCommunication()
        {
            _adapter = BluetoothAdapter.DefaultAdapter;
            _myUuid = UUID.RandomUUID();
        }
        public async Task ConnectAsync()
        {
            try
            {
                _socket = SelectedDevice.CreateRfcommSocketToServiceRecord(_myUuid);
                await _socket.ConnectAsync();
                await Task.Run(() =>
                {
                    var reader = new BinaryReader(_socket.InputStream);
                    
                    while (true)
                    {
                        try
                        {
                            int size = reader.ReadInt16();
                            var b = reader.ReadBytes(size);

                            ReportReceived?.Invoke(this, new ReportReceivedEventArgs {Report = b});
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Disconnect();
                Console.WriteLine(ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                _socket?.Close();
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.Message);
            }
        }

        public Task WriteAsync(byte[] data)
        {
            return Task.Run(() =>
            {
                try
                {
                    _socket.OutputStream.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }

        public Task<string[]> GetDeviceList()
        {
            return Task.Run(() => _adapter.BondedDevices.Select(d => d.Name).ToArray());
        }

        public void SelectDevice(string name)
        {
            SelectedDevice = _adapter.BondedDevices.FirstOrDefault(d => d.Name == name);
        }

        BluetoothDevice SelectedDevice { get; set; }
    }
}