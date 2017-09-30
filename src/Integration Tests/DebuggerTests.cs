using System;
using EnvDTE;
using EnvDTE90a;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Shouldly;
using TestStack.BDDfy;
using System.Threading.Tasks;
using System.Threading;
using Nito.AsyncEx;
using System.IO;
using VisualStudioIntegrationTestsBDDfySample.Integration_Tests;
using VisualStudioIntegrationTestsBDDfySample;

namespace IntegrationTests.BDDfy
{
    [TestClass]
    public class DebuggingSpecifications
    {
        protected DTE _dte;

        [TestInitialize()]
        public void Initialize()
        {            
            _dte = (DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(DTE));
        }


        [TestCleanup]
        public void Teardown()
        {
            
        }

        public void Given_the_program(string program)
        {
            CreateConsoleApplication(program);            
        }

        public void GivenTheCode(string code)
        {
            CreateConsoleApplication(@"
using System;
using System.Linq;

class TestClass 
{
    public static void Main()
    {
     " + code + @" 
    }
}
");
        }

        private void ReplaceCodeInCurrentFileWith(string program)
        {
            if (_dte == null) throw new ArgumentNullException("dte");
            if (_dte.Solution == null) throw new ArgumentNullException("solution");

            _dte.Solution.Open(Consts.TestConsoleApp);

            var doc = (TextDocument)_dte.ActiveDocument.Object();
            EditPoint editPoint = doc.StartPoint.CreateEditPoint();
            EditPoint endPoint = doc.EndPoint.CreateEditPoint();
            editPoint.Delete(endPoint);
            editPoint.Insert(program);
        }

        private void CreateConsoleApplication(string program)
        {
            TestUtils testUtils = new TestUtils();
            testUtils.CloseCurrentSolution(__VSSLNSAVEOPTIONS.SLNSAVEOPT_NoSave);
            testUtils.CreateEmptySolution(@"C:\Tests", "EmptySolution");
            //testUtils.CreateProjectFromTemplate("OzCode Tests", "ConsoleApplication.zip", "CSharp", false);
            BDDfyConfiguration.Write("before replace");
            ReplaceCodeInCurrentFileWith(program);
        }
        protected void We_run_the_program()
        {
            _dte.Debugger.Go(WaitForBreakOrEnd: true);
        }

        protected void We_execute_command(string commandName)
        {
            _dte.ExecuteCommand(commandName);
        }

        protected void We_hit_F5_again()
        {
            _dte.Debugger.Go(WaitForBreakOrEnd: true);
        }
        protected void Then_we_break_on_line(int lineNumber)
        {
            var debugger = _dte.Debugger;
            if (debugger == null) throw new InvalidOperationException("no debugger at this time");
            
            var stackFrame = (StackFrame2)debugger.CurrentStackFrame;
            if (stackFrame == null) throw new InvalidOperationException("no stackframe at this time");

            int debuggerLineNumber = ToFakeLineNumber((int)stackFrame.LineNumber);

            debuggerLineNumber.ShouldBe(lineNumber);
        }

        protected int ToFakeLineNumber(int realLineNumber)
        {
            var doc = (TextDocument)_dte.ActiveDocument.Object();

            string line = doc.CreateEditPoint(doc.StartPoint).GetLines(realLineNumber, realLineNumber + 1);

            // Every line should start with a line number comment, such as: /* 5 */
            // Parse it.
            return Int32.Parse(line.Substring(2, 3));
        }

        public void ThenTheCaretShouldBeOnLine(int line)
        {
            CaretLine().ShouldBe(line);
        }

        private int CaretLine()
        {
            var doc = (TextDocument)_dte.ActiveDocument.Object();
            int lineNo = doc.Selection.CurrentLine;
            return ToFakeLineNumber(lineNo);
        }
    }
}

