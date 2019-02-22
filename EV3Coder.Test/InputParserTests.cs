using System;
using EV3Coder.ConsoleParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EV3Coder.Test
{
    [TestClass]
    public class InputParserTests
    {
        [TestMethod]
        public void ItAcceptsAndStoresStrings()
        {
            var parser = new InputParser();

            parser.Input("New test string");
            
            Assert.AreEqual("New test string", parser.Code);
        }

        [TestMethod]
        public void ItConcatenatesStringsWithNewLine()
        {
            var parser = new InputParser();
            
            parser.Input("test1");
            parser.Input("test2");
            
            Assert.AreEqual($"test1{Environment.NewLine}test2", parser.Code);
        }

        [TestMethod]
        public void ItRunsCode()
        {
            var parser = new InputParser();
            
            parser.Input("Console.WriteLine(\"Hello\");");
            
            
            parser.Run();
        }
    }
}