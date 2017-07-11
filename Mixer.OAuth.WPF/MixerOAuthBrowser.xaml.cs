using Mixer.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Mixer.OAuth.WPF
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Mixer.OAuth.WPF"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Mixer.OAuth.WPF;assembly=Mixer.OAuth.WPF"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:MixerOAuthBrowser/>
    ///
    /// </summary>

    /// <summary>
    /// Interaction logic for MixerOAuthBrowser.xaml
    /// </summary>
    public partial class MixerOAuthBrowser : UserControl
    {
        public MixerOAuthBrowser()
        {
            InitializeComponent();
        }

        public async Task Initialize(string clientID, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            await this.Initialize(clientID, null, scopes, redirectUri);
        }

        public async Task Initialize(string clientID, string clientSecret, IEnumerable<ClientScopeEnum> scopes, string redirectUri)
        {
            string url = await MixerConnection.GetAuthorizationCodeURLForOAuth(clientID, clientSecret, scopes, redirectUri);
            this.Browser.Address = url;
        }

        private void Browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (e.Uri.ToString().Contains("?code="))
            {

            }
        }
    }
}
