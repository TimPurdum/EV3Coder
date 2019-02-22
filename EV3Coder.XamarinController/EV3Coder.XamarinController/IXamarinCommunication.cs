using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace EV3Coder.XamarinController
{
    public interface IXamarinCommunication: ICommunication
    {
        Task<string[]> GetDeviceList();
        void SelectDevice(string name);
    }
}