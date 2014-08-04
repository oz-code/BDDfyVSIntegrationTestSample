using IntegrationTests.BDDfy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.BDDfy;


namespace VisualStudioIntegrationTestsBDDfySample
{
    [TestClass]
    public class StartDebuggingTest : DebuggingSpecifications
    {
        [TestMethod]
        [HostType("VS IDE")]
        public void StartDebugging()
        {

            string program = @"
/* 1  */        class Test
/* 2  */        {            
/* 3  */            public static void Main()
/* 4  */            {
/* 5  */                System.Diagnostics.Debugger.Break();
/* 6  */            }            
/* 7  */        }
";
            
            this.Given(s => s.GivenTheProgram(program))
                .When( s => s.AndWeRunTheProgram())
                .Then( s => s.ThenWeBreakOnLine(5))
                .BDDfy();
        }
    }
}