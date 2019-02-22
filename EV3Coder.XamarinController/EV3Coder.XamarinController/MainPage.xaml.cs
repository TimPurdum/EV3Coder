using System;
using EV3Coder.Core;
using Lego.Ev3.Desktop;
using Xamarin.Forms;

namespace EV3Coder.XamarinController
{
    public partial class MainPage : ContentPage
    {
        BrickController _controller;

        public MainPage()
        {
            InitializeComponent();
        }


        async void Button_OnClicked(object sender, EventArgs e)
        {
            var comm = DependencyService.Get<IXamarinCommunication>();
            
            var devices = await comm.GetDeviceList();
            var selection = await DisplayActionSheet("Select Bluetooth Device", "Cancel",
                null, devices);
            comm.SelectDevice(selection);
            _controller = new BrickController(comm);
            await _controller.Connect();
        }
    }
}