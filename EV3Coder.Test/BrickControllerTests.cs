using System;
using System.Threading.Tasks;
using EV3Coder.Core;
using Lego.Ev3.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EV3Coder.Test
{
    [TestClass]
    public class BrickControllerTests
    {
        [TestMethod]
        public void ItCreatesABrick()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            
            Assert.IsNotNull(controller.Brick);
        }


        [TestMethod]
        public async Task ItCanConnect()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            await controller.Connect();
            
            Assert.AreEqual(1, comm.ReceivedData.Count);
        }


        [TestMethod]
        public async Task ItCanTurnMotor()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            await controller.Connect();

            await controller.TurnMotorAsync(OutputPort.A, 50, 1);
            
            Assert.AreEqual(2, comm.ReceivedData.Count);
        }


        [TestMethod]
        public async Task ItCanSendTankCommands()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            Console.WriteLine("Connecting...");
            await controller.Connect();

            Console.WriteLine("Tank Move...");
            controller.TankMove(50, 50, 1);
            controller.TankMove(-50, -50, 1);
        }
    }
}