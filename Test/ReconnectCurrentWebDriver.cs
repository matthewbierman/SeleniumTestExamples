using System.ComponentModel;


namespace abxch.WebsiteUITest.Test
{
    [Description("Reconnect Current Web Driver")]
    public class ReconnectCurrentWebDriver : WebSiteTestRunnerBase
    {
        public override bool RunTest()
        {
            bool success = true;

            var browserType = this.Configuration.BrowserType;

            this.Configuration.SetBrowser(browserType);

            return success;
        }

     
    }
}
