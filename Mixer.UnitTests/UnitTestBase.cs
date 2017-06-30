using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using System;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    public abstract class UnitTestBase
    {
        protected void TestWrapper(Func<MixerConnection, Task> function)
        {
            try
            {
                MixerConnection client = MixerConnectionUnitTests.GetMixerClient();
                function(client).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
