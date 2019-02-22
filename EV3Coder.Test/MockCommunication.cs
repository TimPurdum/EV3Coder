using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace EV3Coder.Test
{
    class MockCommunication: ICommunication
    {
        public Task ConnectAsync()
        {
            return Task.CompletedTask;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(byte[] data)
        {
            Console.WriteLine(BitConverter.ToString(data));
            return Task.Run(() => { ReceivedData.Add(data); });
        }
        
        public List<byte[]> ReceivedData = new List<byte[]>();

        public event EventHandler<ReportReceivedEventArgs> ReportReceived;
    }
}