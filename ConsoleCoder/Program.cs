using System;
using System.Threading.Tasks;
using EV3Coder.Core;
using Lego.Ev3.Desktop;

namespace ConsoleCoder
{
    class Program
    {
        static void Main()
        {
            var comm = new BluetoothCommunication("COM4");
            var controller = new BrickController(comm);

            Task.Run(async () =>
            {
                await controller.Connect();

                for (var i = 0; i < 100; i++)
                {
                    while (controller.UltrasonicRange > 10)
                    {
                        Console.WriteLine(controller.UltrasonicRange);
                        controller.TankMove(50, 50, 0.5);
                    }

                    controller.TankMove(50, -50, 1);
                }
            });
        }
    }
}