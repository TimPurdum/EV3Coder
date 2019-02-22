using System;
using System.Threading.Tasks;
using EV3Coder.Core;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EV3Coder.Test
{
    [TestClass]
    public class BrickIntegrationTests
    {
        [TestMethod]
        public void ItCreatesABrick()
        {
            var comm = new BluetoothCommunication("COM4");
            var controller = new BrickController(comm);
            
            Assert.IsNotNull(controller.Brick);
        }


        [TestMethod]
        public void ItCanConnect()
        {
            var comm = new BluetoothCommunication("COM4");
            var controller = new BrickController(comm);
            controller.Connect();
        }


        [TestMethod]
        public async Task ItCanTurnMotor()
        {
            var comm = new BluetoothCommunication("COM4");
            var controller = new BrickController(comm);
            controller.Connect();

            await controller.TurnMotorAsync(OutputPort.C, 50, 1);
        }


        [TestMethod]
        public void ItCanSendTankCommands()
        {
            var comm = new BluetoothCommunication("COM4");
            var controller = new BrickController(comm);
            Console.WriteLine("Connecting...");
            controller.Connect();

            controller.TankMove(50, 50, 2);
            controller.TankMove(50, -50, 2);
            controller.TankMove(-50, -50, 2);
        }
    }
}