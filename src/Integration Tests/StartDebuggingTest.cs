using IntegrationTests.BDDfy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System;
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
            this.Given(s => s.Given_the_program(program))
                .When( s => s.We_run_the_program())
                .Then( s => s.Then_we_break_on_line(5))
                .BDDfy();
        }













        [TestMethod]
        [HostType("VS IDE")]
        public void TestAddBreakpointToEveryMethod()
        {

            string program = @"
/* 1  */        class Test
/* 2  */        {            
/* 3  */            public static void Main()
/* 4  */            {
/* 5  */                Method1();
/* 6  */            }
/* 7  */            
/* 8  */            public static void Method1()
/* 9  */            {
/* 10 */            }
/* 11 */        }
";


            this.Given(s => s.Given_the_program(program))
                 .When(s => s.We_execute_command("OzCode.AddBreakpointToEveryMemberInClass"))
                  .And(s => s.We_run_the_program())
                 .Then(s => s.Then_we_break_on_line(4))
                 .When(s => s.We_hit_F5_again())
                 .Then(s => s.Then_we_break_on_line(9))
                .BDDfy();
        }
    }
}