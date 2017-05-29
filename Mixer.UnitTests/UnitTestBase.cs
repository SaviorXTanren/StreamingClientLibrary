using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mixer.Base;
using System;
using System.Threading.Tasks;

namespace Mixer.UnitTests
{
    public abstract class UnitTestBase
    {
        protected void TestWrapper(Func<MixerClient, Task> function)
        {
            try
            {
                MixerClient client = AuthorizationUnitTests.GetMixerClient();
                function(client).Wait();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
