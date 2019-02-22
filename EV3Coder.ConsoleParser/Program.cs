using System;

namespace EV3Coder.ConsoleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var parser = new InputParser();
            
                Console.WriteLine("Enter robot commands!");
                Console.WriteLine("When done, type \"run\" on it's own line to compile.");
                Console.WriteLine("Type \"help\" for a list of commands");
                Console.WriteLine("Or type \"exit\" to quit");

                var done = false;

                while (!done)
                {
                    var newLine = Console.ReadLine();
                    if (newLine.Contains("exit"))
                    {
                        return;
                    }

                    if (newLine.Contains("help"))
                    {
                        PrintHelp();
                    }
                    else if (!newLine.Contains("run"))
                    {
                        parser.Input(newLine);
                    }
                    else
                    {
                        done = true;
                    }
                }

                parser.Run();
            }
            
        }

        static void PrintHelp()
        {
            Console.WriteLine("Commands: ");
            Console.WriteLine("controller.TankMove(bPower, cPower, seconds);");
            Console.WriteLine("example: controller.TankMove(50, 50, 1);");
        }
    }
}