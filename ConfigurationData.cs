using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using OpenQA.Selenium;

namespace abxch.WebsiteUITest
{
    public class ConfigurationData
    {
        public Action<string> Log { get; set; }
        public Func<bool> Cancel { get; set; }
        public Action Completed { get; set; }
        public Func<bool> Pause { get; set; }
        public Func<string, bool> Skip { get; set; }
        public Func<string, bool> PauseOnStep { get; set; }
        public Action<Screenshot, string> ProcessScreenShot { get; set; }
        public Action<string> ProcessFile { get; set; }

        private static class Agents 
        {
         
        }

        public class Agent
        {
            public string Name { get; set; }

            public int Id { get; set; }
            public string Email { get; set; }
        }

        private static class Logins
        {
           
        }

        public ConfigurationData(ServerTypes serverType, BrowserTypes browser)
        {
            this.Log = LogConsole;

            this.ServerType = serverType;
            this.BrowserType = browser;

            ConfigureSettings();
        }

        private void ConfigureSettings()
        {
            ConfigureServerSettings();

            ConfigureBrowser();

        }

        private void ConfigureBrowser()
        {
            switch (BrowserType)
            {
                case BrowserTypes.InternetExplorer:
                {
                    Driver = RemoteWebDriverExtended.GetInternetExplorerDriver();
                }
                break;
                case BrowserTypes.FireFox:
                {
                    Driver = RemoteWebDriverExtended.GetFireFoxDriver();
                }
                break;
                case BrowserTypes.Chrome:
                {
                    Driver = RemoteWebDriverExtended.GetChromeDriver();
                }
                break;
            }



            LogObject(new { BrowserType = this.BrowserType });
        }

        public void SetBrowser(BrowserTypes browser)
        {
            this.BrowserType = browser;
            ConfigureBrowser();
        }

        public void SetServer(ServerTypes server)
        {
            this.ServerType = server;
            ConfigureServerSettings();
        }

        private void ConfigureServerSettings()
        {
            ServerSettings serverSettings = null;

            switch (this.ServerType)
            {
                case ServerTypes.Google:
                {
                    serverSettings = new ServerSettings
                    {
                        RootURL = "https://google.com/",
                        Logins = new ServerSettings.Login[]
                        {
                         
                        },
                    };
                }
                break;
            }



            this.Server = serverSettings;

            LogObject(new { ServerType = this.ServerType });

            LogObject(serverSettings);
        }

        private void LogConsole(string message)
        {
            Console.WriteLine(message);
        }

        internal void LogObject(object obj, [CallerMemberName] string memberName = "")
        {
            List<string> messages = new List<string>();

            if (!string.IsNullOrEmpty(memberName))
            {
                messages.Add(memberName);
            }

            var properties = obj.GetType().GetProperties();

            foreach (PropertyInfo prop in properties)
            {
                if (IsSimpleType(prop.PropertyType))
                {
                    string name = prop.Name;
                    string value = prop.GetValue(obj, null)?.ToString();

                    messages.Add($"{name}:'{value}'");
                }
            }

            Log("\t" + string.Join(" ", messages));
        }

        private static bool IsSimpleType(Type type)
        {
            bool isSimpleType = true;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                isSimpleType = IsSimpleType(type.GetGenericArguments()[0]);
            }
            else
            {
                isSimpleType =
                    type.IsPrimitive
                    || type.IsEnum
                    || type.Equals(typeof(string))
                    || type.Equals(typeof(DateTime))
                    || type.Equals(typeof(decimal));

            }

            return isSimpleType;
        }

        public enum BrowserTypes
        {
            [Description("Internet Explorer")]
            InternetExplorer,
            [Description("FireFox")]
            FireFox,
            [Description("Chrome")]
            Chrome
        }

        public BrowserTypes BrowserType { get; internal set; }

        public class ServerSettings
        {
            public string RootURL { get; internal set; }

            public Login[] Logins { get; internal set; }

            public Agent DefaultAgent { get; internal set; }

            public enum LoginTypes
            {
                Agent,
                Carrier,
                Employer,
            }

            public class Login
            {
                public LoginTypes Type { get; set; }

                public string Username { get; set; }

                public string Password { get; set; }
            }
        }

        public ServerSettings Server { get; private set; }

        public ServerTypes ServerType { get; internal set; }

        public enum ServerTypes
        {
            Google
        }

        public IWebDriver Driver { get; private set; }
    }
}
