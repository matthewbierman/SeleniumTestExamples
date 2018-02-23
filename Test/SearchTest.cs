using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static abxch.WebsiteUITest.ConfigurationData;


namespace abxch.WebsiteUITest.Test
{
    [Description("Search Test")]
    public class SearchTest : WebSiteTestRunnerBase
    {
        public SearchTest() : base()
        {

        }

        public SearchTest(ConfigurationData configuration) : base(configuration)
        {

        }

        public override bool RunTest()
        {
            bool success = true;

            ProcessStep(ref success, "Search for GitHub Page", () => SearchForGitHubPage());

            ProcessStep(ref success, "Count Results", () => CountResults());

            return success;
        }

        private bool CountResults()
        {
            bool success = true;

            List<IWebElement> elements = null;

            Process(ref success, () => TryGetElements(By.CssSelector("div[class='g']"), out elements, failIfNoneFound: true));

            int count = 0;

            Process(success, () => { count = elements.Count(); });

            Process(ref success, () => { return count > 0; });

            LogObject( new { count } );

            return success;
        }

        private bool SearchForGitHubPage()
        {
            bool success = true;

            Process(success, () => Open("/"));

            Process(ref success, () => InputByName(name: "q", value: "matthew bierman iowa github"));

            Process(ref success, () => ClickByName(name: "btnK", value: "Google Search"));

            return success;
        }
    }
}
