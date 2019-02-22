using System;
using System.Threading;
using System.Threading.Tasks;
using Lego.Ev3.Core;

namespace EV3Coder.Core
{
    public class BrickController
    {
        public Brick Brick { get; set; }
        public InputPort UltrasonicPort { get; set; } = InputPort.One;
        public UltrasonicMode UltrasonicMode { get; set; } = UltrasonicMode.Inches;
        public InputPort ColorPort { get; set; } = InputPort.Two;
        public ColorMode ColorMode { get; set; } = ColorMode.Reflective;
        public float UltrasonicRange => Brick.Ports[UltrasonicPort].SIValue;
        public float ColorValue => Brick.Ports[UltrasonicPort].SIValue;

        public BrickController(ICommunication communication)
        {
            Brick = new Brick(communication, true);
        }

        public async Task Connect()
        {
            await Brick.ConnectAsync();
            Brick.Ports[UltrasonicPort].SetMode(UltrasonicMode);
        }

        public void TurnMotor(OutputPort port, int power, double seconds)
        {
            Task.Run(() => TurnMotorAsync(port, power, seconds));
            Thread.Sleep((int)(seconds * 1000));
        }
        

        public async Task TurnMotorAsync(OutputPort port, int power, double seconds)
        {
            await Brick.DirectCommand
                .TurnMotorAtPowerForTimeAsync(port, power, (uint) (seconds * 1000), false);
        }


        public void TurnMotorDegrees(OutputPort port, int power, int degrees)
        {
            var ready = false;
            Task.Run(async () =>
            {
                await TurnMotorDegreesAsync(port, power, degrees);
                await Brick.DirectCommand.OutputReadyAsync(port);
                ready = true;
            });
            while (!ready)
            {
                Thread.Sleep(250);
            }
        }


        public async Task TurnMotorDegreesAsync(OutputPort port, int power, int degrees)
        {
            if (degrees < 0)
            {
                degrees = Math.Abs(degrees);
                power = power * -1;
            }
            
            await Brick.DirectCommand
                .StepMotorAtPowerAsync(port, power, (uint)degrees, false);
        }

        public void TankMove(int powerB, int powerC, double seconds)
        {
            Parallel.Invoke(async () => { await TurnMotorAsync(OutputPort.B, powerB, seconds); },
                async () => { await TurnMotorAsync(OutputPort.C, powerC, seconds); });
            Thread.Sleep((int)(seconds * 1000));
        }
        
        
        public void TankMoveDegrees(int powerB, int powerC, int degrees)
        {
            var readyB = false;
            var readyC = false;
            Parallel.Invoke(async () => 
                { 
                    await TurnMotorDegreesAsync(OutputPort.B, powerB, degrees);
                    await Brick.DirectCommand.OutputReadyAsync(OutputPort.B);
                    readyB = true;
                },
                async () =>
                {
                    await TurnMotorDegreesAsync(OutputPort.C, powerC, degrees);
                    await Brick.DirectCommand.OutputReadyAsync(OutputPort.C);
                    readyC = true;
                });
            while (!(readyB && readyC))
            {
                Thread.Sleep(250);
            }
        }
    }
}
