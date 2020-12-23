using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Glimesh.Base.UnitTests
{
    public abstract class UnitTestBase
    {
        public const string clientID = "86cbc95fe6e583fd72654af65c68a0a2cea8890cde4464de26c0f946d24fae1a";
        public const string clientSecret = "8562d68a91a1913e558b333115801b95f3db05699ee4c1f0300cef19f54fd839";

        public static readonly List<OAuthClientScopeEnum> scopes = new List<OAuthClientScopeEnum>()
        {
            OAuthClientScopeEnum.publicinfo,
            OAuthClientScopeEnum.chat,
            OAuthClientScopeEnum.email,
            OAuthClientScopeEnum.streamkey
        };

        private static GlimeshConnection connection;

        public static GlimeshConnection GetGlimeshClient()
        {
            if (UnitTestBase.connection == null)
            {
                UnitTestBase.connection = GlimeshConnection.ConnectViaLocalhostOAuthBrowser(clientID, clientSecret, scopes).Result;
            }

            Assert.IsNotNull(UnitTestBase.connection);
            return UnitTestBase.connection;
        }

        protected static void TestWrapper(Func<GlimeshConnection, Task> function)
        {
            try
            {
                GlimeshConnection connection = UnitTestBase.GetGlimeshClient();
                function(connection).Wait();
            }
            catch (AggregateException aex)
            {
                Assert.Fail(aex.InnerException.ToString());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
        }
    }
}
