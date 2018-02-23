using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace abxch.WebsiteUITest
{
    public interface IWebSiteTestRunner : IWebSiteTest
    {
        void Run();

        bool RunTest();
    }

    public interface IWebSiteTest
    {
        ConfigurationData Configuration { get; }

        void SetConfiguration(ConfigurationData testConfiguration);
    }
}
