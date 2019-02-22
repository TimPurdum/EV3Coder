using System;
using System.Threading.Tasks;
using EV3Coder.XamarinController.UWP;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using Xamarin.Forms;

[assembly: Dependency(typeof(WinBluetoothCommunication))]
namespace EV3Coder.XamarinController.UWP
{
    public class WinBluetoothCommunication : IXamarinCommunication
    {
        private BluetoothCommunication _ev3Comm;

        public string SelectedDevice { get; private set; }

        public event EventHandler<ReportReceivedEventArgs> ReportReceived;
        

        public async Task ConnectAsync()
        {
            _ev3Comm = new BluetoothCommunication(SelectedDevice);
            await _ev3Comm.ConnectAsync();
            _ev3Comm.ReportReceived += BaseCommReportReceived;
        }

        private void BaseCommReportReceived(object sender, ReportReceivedEventArgs e)
        {
            var handler = ReportReceived;

            handler?.Invoke(sender, e);
        }

        public void Disconnect()
        {
            _ev3Comm.Disconnect();
        }

        public Task<string[]> GetDeviceList()
        {
            return Task.Run(() => new string[] { "COM1", "COM2", "COM3", "COM4" });
        }

        public void SelectDevice(string name)
        {
            SelectedDevice = name;
        }

        public async Task WriteAsync(byte[] data)
        {
            await _ev3Comm.WriteAsync(data);
        }
    }
}
