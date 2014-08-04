using System;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using TestStack.BDDfy;
using TestStack.BDDfy.Configuration;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace VisualStudioIntegrationTestsBDDfySample
{
    [TestClass]
    public class BDDfyConfiguration
    {
        public delegate void ThreadInvoker();

        [AssemblyInitialize]
        public static void Configure(TestContext testContext)
        {
            UIThreadInvoker.Initialize();

            Configurator.StepExecutor = new VisualStudioStepExecutor();
        }

        private class VisualStudioStepExecutor : StepExecutor
        {            
            public override object Execute(Step step, object testObject)
            {
                object result = null;
                UIThreadInvoker.Invoke((ThreadInvoker)(delegate
                {
                    result = base.Execute(step, testObject);
                }));

                WaitForIdle();                

                return result;
            }


            private static void WaitForIdle()
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
            }
        }
    }
}
