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
        const string ConnectionString = "0C-00-02-00-80-00-00-A3-81-00-81-0F-81-00";

        [TestMethod]
        public void ItCreatesABrick()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            
            Assert.IsNotNull(controller.Brick);
        }


        [TestMethod]
        public void ItCanConnect()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            controller.Connect();
            
            Assert.AreEqual(1, comm.ReceivedData.Count);
            Assert.AreEqual(ConnectionString, BitConverter.ToString(comm.ReceivedData[0]));
        }


        [TestMethod]
        public void ItCanTurnMotor()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            controller.Connect();

            controller.TurnMotor(OutputPort.A, 50, 1);
            
            Assert.AreEqual(2, comm.ReceivedData.Count);
        }


        [TestMethod]
        public void ItCanSendTankCommands()
        {
            var comm = new MockCommunication();
            var controller = new BrickController(comm);
            Console.WriteLine("Connecting...");
            controller.Connect();

            Console.WriteLine("Tank Move...");
            controller.TankMove(50, 50, 1);
            
            Assert.AreEqual(3, comm.ReceivedData.Count);
        }
    }
}