using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using static abxch.WebsiteUITest.ConfigurationData;

namespace abxch.WebsiteUITest.Test
{
    public abstract class WebSiteTestBase : IWebSiteTest
    {

        public WebSiteTestBase(ConfigurationData configurationData)
        {
            SetConfiguration(configurationData);
        }

        public WebSiteTestBase()
        {

        }

        protected void Reset()
        {
            WasCancelled = false;
        }

        protected bool WasCancelled { get; set; } = false;

        private bool IsCancelled
        {
            get
            {
                bool isCancelled = false;

                if (WasCancelled)
                {
                    isCancelled = true;
                }
                else
                {
                    isCancelled = CheckForCancellation();
                }

                return isCancelled;
            }
        }

        private bool CheckForCancellation()
        {
            bool cancelled = false;

            if (Configuration.Cancel != null)
            {
                if (Configuration.Cancel())
                {
                    Configuration.Log("Cancel was called");
                    cancelled = true;
                    WasCancelled = true;
                }
            }

            return cancelled;
        }

        [DebuggerStepThrough]
        protected void ProcessStep(ref bool success, string step, Func<bool> function, bool takeScreenShot = true)
        {
            if (SkipStep(step))
            {
                this.Log($"Skipped:{step}");
            }
            else
            {
                if (Configuration.PauseOnStep != null)
                {
                    Configuration.PauseOnStep(step);
                }

                if (success)
                {
                    success = ProcessStep(step, function, takeScreenShot);
                }
            }
        }

        private void ProcessStepScreenshot(string step)
        {
            string fileName = $"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")} {step}.jpeg";

            CreateScreenshot(fileName);

            LogObject(new { fileName = fileName });
        }

        public void CreateScreenshot(string fileName)
        {
            Screenshot screenshot = GetScreenshot();

            Configuration.ProcessScreenShot(screenshot, fileName);
        }

        private Screenshot GetScreenshot()
        {
            return ((ITakesScreenshot)Driver).GetScreenshot();
        }

        private bool ProcessStep(string step, Func<bool> function, bool takeScreenShot = true)
        {
            bool success;
            this.Log($"Start:{step}");

            success = CanContinue;

            Process(ref success, function);

            if (takeScreenShot && Configuration.ProcessScreenShot != null)
            {
                ProcessStepScreenshot(step);
            }

            Log($"End:{step} success:{success}");

            return success;
        }

        public void ProcessFile(string filePath)
        {
            Configuration.ProcessFile?.Invoke(filePath);
        }

        protected bool SkipStep(string step)
        {
            bool skip = false;

            if (Configuration.Skip != null)
            {
                skip = Configuration.Skip(step);
            }

            return skip;
        }
        /// <summary>
        /// Run the function is success is true, and set success to value returned by function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="function"></param>
        [DebuggerStepThrough]
        protected void Process(ref bool success, Func<bool> function)
        {
            if (success)
            {
                success = function();
            }
        }
        /// <summary>
        /// Run the action if success is true, but don't change the value of success.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="action"></param>
        [DebuggerStepThrough]
        protected void Process(bool success, Action action)
        {
            if (success)
            {
                action();
            }
        }
        /// <summary>
        /// Run action if both runCondition and success are true. Don't change value of success.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="runCondition"></param>
        /// <param name="action"></param>
        [DebuggerStepThrough]
        protected void Process(bool success, bool runCondition, Action action)
        {
            if (runCondition)
            {
                Process(success, action);
            }
        }
        /// <summary>
        /// Run function if both success and runCondition are true; set success to value returned by function.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="runCondition"></param>
        /// <param name="function"></param>
        [DebuggerStepThrough]
        protected void Process(ref bool success, bool runCondition, Func<bool> function)
        {
            if (runCondition)
            {
                Process(ref success, function);
            }
        }

        protected bool CanContinue
        {
            get
            {
                bool canContinue = true;

                WaitForResume();

                if (IsCancelled)
                {
                    canContinue = false;
                }
                return canContinue;
            }
        }

        private void WaitForResume()
        {
            const int waitMiliseconds = 500; //half second

            if (Configuration.Pause != null)
            {
                if (Configuration.Pause())
                {
                    Log("Paused");
                    while (Configuration.Pause())
                    {
                        Thread.Sleep(waitMiliseconds);
                    }
                }

            }
        }

        protected bool OpenPage(string siteRelativeURL = "", string expectedTag = null)
        {
            bool success = false;

            Open(siteRelativeURL);

            if (expectedTag == null)
            {
                expectedTag = siteRelativeURL;
            }

            if (IsOnPage(expectedTag, logResults: false))
            {
                success = true;
            }
            else
            {
                success = false;
            }

            LogObject(new { siteRelativeURL = siteRelativeURL, expectedTag = expectedTag, success = success });


            return success;
        }

        protected string GetLoggedInUser()
        {
            string username = GetElementById("btnUserOptions")?.Text?.Trim();

            return username;

        }

        protected bool LogInAgent(bool failIfAlreadyLoggedIn = false)
        {
            LogStart();

            bool success = true;

            var loginData = Configuration.Server.Logins.Single(l => l.Type == ServerSettings.LoginTypes.Agent);

            Process(ref success, () => Login(loginData, failIfAlreadyLoggedIn));

            LogSuccess(success);

            return success;

        }

        protected bool LoginEmployer(bool failIfAlreadyLoggedIn = false)
        {
            LogStart();

            bool success = true;

            var loginData = Configuration.Server.Logins.Single(l => l.Type == ServerSettings.LoginTypes.Employer);

            Process(ref success, () => Login(loginData, failIfAlreadyLoggedIn));

            LogSuccess(success);

            return success;

        }

        protected bool LogInCarrierAdmin(bool failIfAlreadyLoggedIn = false)
        {
            LogStart();

            bool success = true;

            var loginData = Configuration.Server.Logins.Single(l => l.Type == ServerSettings.LoginTypes.Carrier);

            Process(ref success, () => Login(loginData, failIfAlreadyLoggedIn));

            LogSuccess(success);

            return success;
        }

        /// <summary>
        /// Logs in the user specified in the LoginData object
        /// Will navigate to Login page
        /// Will log off any currently logged in user
        /// </summary>
        /// <param name="loginData"></param>
        /// <param name="failIfAlreadyLoggedIn">Sets if the function will return false (failed) if the user is all ready logged</param>
        /// <returns></returns>
        private bool Login(ServerSettings.Login loginData, bool failIfAlreadyLoggedIn = false)
        {
            LogObject(loginData);

            bool success = true;

            bool isAlreadyLoggedIn = false;

            Process(success, () => Open("/"));

            Process(ref success, () => EnsureNoLoggedInUser(loginData, failIfAlreadyLoggedIn, out isAlreadyLoggedIn));

            Process(
                ref success,
                runCondition: !isAlreadyLoggedIn,
                function: () =>
                {

                    Process(ref success, () => OpenPage("Login"));

                    Process(ref success, () => InputById("txtEmail", loginData.Username, clear: true));

                    Process(ref success, () => InputById("txtPassword", loginData.Password, clear: true));

                    Process(ref success, () => ClickById("btnLogIn"));

                    Process(ref success, () => Wait(() => IsOnPage("/")));

                    return success;
                }
                );

            LogObject(new { failIfAlreadyLoggedIn = failIfAlreadyLoggedIn, isAlreadyLoggedIn = isAlreadyLoggedIn, success = success });

            return success;
        }

        protected bool EnsureNoLoggedInUser(ServerSettings.Login loginData, bool failIfAlreadyLoggedIn)
        {
            bool success = true;

            bool isAlreadyLoggedIn = false;

            Process(ref success, () => EnsureNoLoggedInUser(loginData, failIfAlreadyLoggedIn, out isAlreadyLoggedIn));

            return success;
        }

        protected bool OpenCurrentPageOnNewWindow()
        {
            bool success = true;

            string pageURL = null;

            Process(ref success, () => TryGetPageURL(out pageURL));

            Process(success, () =>
            {
                string javascript = $"$(window.open('', '_blank', 'width=1000'))";

                ((IJavaScriptExecutor)Driver).ExecuteScript(javascript);

            });

            Process(success, () => Driver.SwitchTo().Window(Driver.WindowHandles.Last())); //switch to last window created

            Process(ref success, () => OpenPage(pageURL));

            return success;
        }

        public bool SwitchToWindow(string expectedURL, bool closeCurrentWindow = false)
        {
            bool success = false;

            var windowHandles = Driver.WindowHandles;

            string currentWindowHandle = Driver.CurrentWindowHandle;

            string foundWindowHandle = null;

            int index = 0;

            while (!success && index < windowHandles.Count)
            {
                Driver.SwitchTo().Window(windowHandles[index]);

                if (IsOnPage(expectedURL))
                {
                    foundWindowHandle = Driver.CurrentWindowHandle;
                    success = true;
                }
                index++;
            }

            if (!success)
            {
                Driver.SwitchTo().Window(currentWindowHandle);
            }
            else if (closeCurrentWindow)
            {
                Driver.SwitchTo().Window(currentWindowHandle);
                Driver.Close();
                Driver.SwitchTo().Window(foundWindowHandle);
            }



            LogObject(new { expectedURL = expectedURL, success = success });


            return success;
        }


        protected bool EnsureNoLoggedInUser(ServerSettings.Login loginData, bool failIfAlreadyLoggedIn, out bool isAlreadyLoggedIn)
        {
            bool success = true;

            isAlreadyLoggedIn = false;

            bool hasLoggedInUser = false;

            string loggedInUser = GetLoggedInUser();

            if (!string.IsNullOrEmpty(loggedInUser))
            {
                hasLoggedInUser = true;
            }

            if (hasLoggedInUser)
            {
                if (loggedInUser == loginData.Username) //is user the same?
                {
                    isAlreadyLoggedIn = true;
                }

                if (isAlreadyLoggedIn)
                {
                    if (failIfAlreadyLoggedIn)
                    {
                        success = false;
                    }
                }
                else     //we have a logged in user, but not the one we want to log in
                {
                    success = LogOff();
                }
            }

            return success;
        }

        protected bool EnsureNoLoggedInUser(bool failIfAnyLoggedIn = false)
        {
            bool success = true;

            bool hasLoggedInUser = false;

            string loggedInUser = GetLoggedInUser();

            if (!string.IsNullOrEmpty(loggedInUser))
            {
                hasLoggedInUser = true;
            }

            if (hasLoggedInUser)
            {
                if (failIfAnyLoggedIn)
                {
                    success = false;
                }
                else
                {
                    Process(ref success, LogOff);
                }
            }

            return success;
        }

        protected bool LogOff()
        {
            bool success = true;

            Process(ref success, () => ClickById("btnUserOptions"));

            Process(success, () => Wait(1)); //log off messed up by show animation, will need a better solution

            Process(ref success, () => ClickById("btnLogOff"));

            Process(ref success, () => Wait(() => IsOnPage("Login")));

            LogObject(new { success = success });

            return success;
        }

        /// <summary>
        /// Searches for a validation message on the page, return true if message is found
        /// </summary>
        /// <param name="expectedValidationMessage">The error message to search for</param>
        /// <returns>True if a validation message is on the screen with the errorMessage</returns>
        protected bool ValidationMessageFound(string expectedValidationMessage)
        {
            var validationMessages = GetValidationMessages();

            bool success = ValidationMessageFound(expectedValidationMessage, validationMessages);

            return success;
        }

        private bool ValidationMessageFound(string expectedValidationMessage, IEnumerable<string> validationMessages, bool logResults = true)
        {
            bool success = validationMessages?.Any(m => string.Equals(m, expectedValidationMessage, StringComparison.OrdinalIgnoreCase)) == true;

            if (logResults)
            {
                LogObject(new { expectedValidationMessage = expectedValidationMessage, success = success });
            }

            return success;
        }

        protected bool ValidationMessagesFound(IEnumerable<string> expectedValidationMessages, bool failIfExtra = true)
        {
            bool success = true;

            var foundValidationMessages = GetValidationMessages();

            Process(ref success, () =>
            {
                bool hadNotFoundMessages = false;

                foreach (string expectedMessage in expectedValidationMessages)
                {
                    if (!ValidationMessageFound(expectedMessage, foundValidationMessages, logResults: false))
                    {
                        hadNotFoundMessages = true;

                        LogObject(new { messageNotFound = true, expectedMessage = expectedMessage });
                    }
                }

                if (hadNotFoundMessages)
                {
                    success = false;
                }

                return success;
            }
            );

            Process(success: ref success, runCondition: failIfExtra, function: () => NoExtraValidationMessagesFound(expectedValidationMessages, foundValidationMessages));

            LogObject(new { success = success, failIfExtra = failIfExtra });

            return success;
        }



        private bool NoExtraValidationMessagesFound(IEnumerable<string> expectedValidationMessages, IEnumerable<string> foundValidationMessages)
        {
            bool success = true;

            Process(ref success, () =>
            {
                bool hadNotFoundMessages = false;

                foreach (string foundMessage in foundValidationMessages)
                {
                    if (!ValidationMessageFound(foundMessage, expectedValidationMessages, logResults: false))
                    {
                        hadNotFoundMessages = true;

                        LogObject(new { extraMessageFound = true, foundMessage = foundMessage });
                    }
                }

                if (hadNotFoundMessages)
                {
                    success = false;
                }

                return success;
            }
          );

            LogObject(new { success = success });

            return success;
        }

        private IEnumerable<string> GetValidationMessages()
        {
            //get javascript lobibox objects, not divs
            var validationErrorLobiboxes = GetJavascriptResult("validate.validationErrorLobiboxes") as ReadOnlyCollection<object>;

            //gets the divs, that are the $el property in the lobibox object
            var validationWebElements = validationErrorLobiboxes?.Select(o => ((o as Dictionary<string, object>)["$el"] as ReadOnlyCollection<IWebElement>).First());

            var validationMessages = validationWebElements
                .Select(vm => vm.Text)
                .Select(m => m.Remove(m.LastIndexOf("\r\n×", StringComparison.Ordinal)));

            return validationMessages;
        }

        protected void Log(string message)
        {
            Configuration.Log(message);
        }

        protected void LogObject(object obj, [CallerMemberName] string memberName = "")
        {
            Configuration.LogObject(obj, memberName);
        }

        protected void LogSuccess(bool success, [CallerMemberName] string memberName = "")
        {
            LogObject(new { success = success }, memberName);
        }

        internal void LogStart([CallerMemberName] string memberName = "")
        {
            Log("\t" + memberName);
        }


        /// <summary>
        /// Attempts to find the element and click on it
        /// </summary>
        /// <param name="by">The method to find the element, e.g. by id, name, etc.</param>
        /// <param name="failIfMoreThanOneFound">determines if the method should fail if more than element is found</param>
        /// <returns>True: is able to locate an element that matches the selector, that element is clickable, and it gets clicked, False: fails those tests, e.g. if 0 elements are found matching selector</returns>
        protected bool Click(By by, bool failIfMoreThanOneFound = true, bool logResults = true)
        {
            bool success = true;

            IWebElement element = null;

            if (success)
            {
                success = TryGetElement(by, out element, failIfMoreThanOneFound);
            }

            if (success)
            {
                success = Click(element, logResults);
            }

            return success;
        }

        private bool ElementIsDisplayedAndEnabled(IWebElement element)
        {
            bool success = true;

            Process(ref success, () => element.Displayed);

            Process(ref success, () => element.Enabled);

            return success;

        }

        /// <summary>
        /// Returns true if a click at the location of the element is expected to click on that element (as opposed to an element that is covering it up). 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private bool FoundClickableElementAtLocation(IWebElement element)
        {
            bool success = true;

            Process(ref success, () => ElementIsDisplayedAndEnabled(element));

            if (success)
            {
                var point = ((RemoteWebElement)element).LocationOnScreenOnceScrolledIntoView;
                point.X += (element.Size.Width / 2);
                point.Y += (element.Size.Height / 2);

                IWebElement initialFoundElement = (IWebElement)GetJavascriptResult($"document.elementFromPoint({point.X}, {point.Y})");

                IWebElement foundElement = initialFoundElement;

                //the thing we need may be a parent of the item found, keep crawling up the tree looking for it
                while (!element.Equals(foundElement) && foundElement != null)
                {
                    foundElement = GetParentElement(foundElement);
                }

                if (element.Equals(foundElement))
                {
                    success = true;
                }
                else
                {
                    success = false;
                    LogObject(new { elementTagName = element.TagName, elementText = element.Text, foundElementTag = initialFoundElement.TagName, foundElementText = initialFoundElement.Text, success = success });
                }
            }

            return success;
        }

        private static IWebElement GetParentElement(IWebElement element)
        {
            IWebElement parentElement = null;

            if (element != null && element.TagName != "html")
            {
                parentElement = element.FindElement(By.XPath("./.."));
            }

            return parentElement;
        }
        /// <summary>
        /// Try to get element.
        /// </summary>
        /// <param name="by">the selector for the element</param>
        /// <param name="element">the found element</param>
        /// <param name="failIfMoreThanOneFound">fail to return an element if more than one is found</param>
        /// <param name="logResult">write to the log</param>
        /// <returns></returns>
        protected bool TryGetElement(By by, out IWebElement element, bool failIfMoreThanOneFound = false, bool logResult = false)
        {
            bool success = true;

            element = null;

            List<IWebElement> elements = null;

            Process(ref success, () => TryGetElements(by, out elements)); //get all elements matching criteria

            Process(ref success, () => !(failIfMoreThanOneFound && elements.Count() > 1));

            if (success)
            {
                element = elements.First();
            }

            if (logResult)
            {
                LogObject(new { failIfMoreThanOneFound = failIfMoreThanOneFound, foundElements = elements?.Count(), success = success });
            }

            return success;
        }
        /// <summary>
        /// Click the passed-in element, return true if successful, else false. Log the result.
        /// </summary>
        /// <param name="element">Any element, e.g. a button</param>
        /// <param name="logResults"></param>
        /// <returns>false if element is not clickable, or if it's clickable but still couldn't be clicked. True is element is clicked.</returns>
        protected bool Click(IWebElement element, bool logResults = true, bool tryScrollIfCovered = false)
        {
            bool success = true;

            bool foundClickable = false;

            Process(success, () => foundClickable = FoundClickableElementAtLocation(element));

            if (!foundClickable)
            {
                if (tryScrollIfCovered)
                {
                    Process(ref success, () => ScrollToTop());
                    Process(ref success, () => FoundClickableElementAtLocation(element));
                }
                else
                {
                    success = false;
                }
            }

            string elementTagName = element.TagName;
            string elementText = element.Text;

            if (success)
            {
                element.Click();
            }

            if (logResults)
            {
                LogObject(new { elementTagName = elementTagName, elementText = elementText, success = success });
            }

            return success;
        }

        protected bool CollapseErrorPanelIfExists()
        {
            bool success = true;

            IWebElement collapseErrorPanel = null;

            Process(success, () => TryGetElement(By.CssSelector("[class='lobibox-notify-msg']"), out collapseErrorPanel));

            if (collapseErrorPanel != null && collapseErrorPanel.Text.Equals("click here to hide errors", StringComparison.OrdinalIgnoreCase))
            {
                Process(ref success, () => Click(collapseErrorPanel)); //get the error messages out of the way
            }

            return success;
        }


        /// <summary>
        /// Attempts to find the element with the given id and click on it
        /// </summary>
        /// <param name="id">The id for the element to click</param>
        /// <returns>True is able to locate a single element that matches the id, False if 0 or more than one element is found</returns>
        protected bool ClickById(string id)
        {
            bool success = Click(By.Id(id), logResults: false);

            LogObject(new { id = id, success = success });

            return success;

        }


        /// <summary>
        /// Attempts to find the element with the given id and click on it
        /// </summary>
        /// <param name>The name for the element to click</param>
        /// <param value=null>optional: will narrow down the element to click to one also having this value attribute</param>
        /// <returns>True is able to locate a single element that matches the id, False if 0 or more than one element is found</returns>
        protected bool ClickByName(string name, string value)
        {
            By selector = selector = By.CssSelector($"[name='{name}'][value='{value}']");

            bool success = Click(selector, logResults: false);

            LogObject(new { name = name, value = value, success = success });

            return success;

        }

        /// <summary>
        /// Attempts to find the element with the given id and click on it
        /// </summary>
        /// <param name="id">The id for the element to click</param>
        /// <returns>True is able to locate a single element that matches the id, False if 0 or more than one element is found</returns>
        protected bool ClickByButtonType(string buttonType, string dataId = null)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, runCondition: dataId != null, function: () => TryGetElementByButtonType(buttonType, dataId, out element));

            Process(ref success, runCondition: dataId == null, function: () => TryGetElementByButtonType(buttonType, out element));

            Process(ref success, () => Click(element));

            LogObject(new { buttonType = buttonType, dataId = dataId, success = success });

            return success;

        }

        protected bool InputByName(string name, string value, bool clear = false)
        {
            bool success = Input(By.Name(name), value, clear, logResults: false);

            LogObject(new { name = name, value = value, clear = clear, success = success });

            return success;
        }

        /// <summary>
        /// Selects an option from a select element by text
        /// </summary>
        /// <param name="id">The id of the select element</param>
        /// <param name="value">The value of the option you want to select</param>
        /// <returns>True if the select element was found and the options was able to be selected</returns>
        protected bool SelectDropDownValueById(string id, string value)
        {
            bool success = true;

            IWebElement dropDownElement = null;

            Process(ref success, () => TryGetElementById(id, out dropDownElement));

            Process(ref success, () => SelectDropDownValue(dropDownElement, value));

            LogObject(new { id = id, value = value, success = success });

            return success;
        }

        private bool SelectDropDownValue(IWebElement dropDownElement, string value)
        {
            bool success = true;

            SelectElement dropDownSelectElement = null;

            Process(
                success: ref success,
                function: () =>
                {
                    dropDownSelectElement = new SelectElement(dropDownElement);
                    success = dropDownSelectElement.Options.Any(op => op.Value() == value);
                    return success;
                }
            );

            Process(
                success: ref success,
                function: () =>
                {
                    dropDownSelectElement.SelectByValue(value);
                    success = dropDownSelectElement.SelectedOption.Value() == value;
                    return success;
                }
            );

            return success;
        }

        private bool GetDropDownValue(string id, ref string wantedText)
        {
            bool success = true;
            IWebElement comboBox = null;
            string text = "";
            Process(
               success: ref success,
               function: () =>
               {
                   comboBox = Driver.FindElement(By.Id(id));
                   SelectElement selectedValue = new SelectElement(comboBox);
                   text = selectedValue.SelectedOption.Text;
                   return success;
               }
            );
            wantedText = text;
            return success;
        }

        protected bool TryCheckElementDisplayedById(string id, bool isDisplayed)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElementById(id, out element));

            Process(ref success, () => TryCheckElementDisplayed(element, isDisplayed));

            LogObject(new { success = success });

            return success;
        }

        /// <summary>
        /// checks if the element displayed (ignores if scrolled into view) matches the isDisplayed argument
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isDisplayed"></param>
        /// <returns></returns>
        protected bool TryCheckElementDisplayed(IWebElement element, bool isDisplayed)
        {
            bool success = true;

            Process(ref success, () => element.Displayed == isDisplayed);

            LogObject(new { success = success });

            return success;
        }


        /// <summary>
        /// Selects an option from a select element by text
        /// </summary>
        /// <param name="id">The id of the select element</param>
        /// <param name="value">The value of the option you want to select</param>
        /// <returns>True if the select element was found and the options was able to be selected</returns>
        protected bool SelectDropDownValueByName(string name, string value)
        {
            bool success = true;

            IWebElement dropDownElement = null;
            SelectElement dropDownSelectElement = null;

            Process(ref success, () => TryGetElementByName(name, out dropDownElement));

            Process(ref success, () => FoundClickableElementAtLocation(dropDownElement));

            IWebElement foundOption = null;

            Process(ref success, () =>
            {
                dropDownSelectElement = new SelectElement(dropDownElement);
                foundOption = dropDownSelectElement.Options.FirstOrDefault(op => op.Value() == value);
                return foundOption != null;
            }
            );

            Process(ref success, () =>
            {
                dropDownSelectElement.SelectByIndex(dropDownSelectElement.Options.IndexOf(foundOption));
                return dropDownSelectElement.SelectedOption.Value() == value;
            }
            );

            LogObject(new { name = name, value = value, success = success });

            return success;
        }

        protected bool InputById(string id, string value, bool clear = false)
        {
            bool success = Input(By.Id(id), value, clear, logResults: false);

            LogObject(new { id = id, value = value, clear = clear, success = success });

            return success;

        }

        protected bool Input(By by, string value, bool clear = false, bool logResults = true)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElement(by, out element));

            Process(ref success, () => Input(element, value, clear, logResults));

            return success;

        }

        protected bool Input(IWebElement element, string value, bool clear = false, bool logResults = true)
        {
            bool success = true;

            Process(ref success, () => ElementIsDisplayedAndEnabled(element));

            Process(success, () =>
            {
                if (clear)
                {
                    element.Clear();
                }

                element.SendKeys(value);
            }
            );

            if (logResults)
            {
                LogObject(new { value = value, clear = clear });
            }

            return success;
        }

        protected bool ClearById(string id)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElementById(id, out element));

            Process(ref success, () => Clear(element));

            LogObject(new { id = id, success = success });

            return success;
        }

        protected bool ClearByName(string name)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElementByName(name, out element));

            Process(ref success, () => Clear(element));

            LogObject(new { name = name, success = success });

            return success;
        }

        private bool Clear(IWebElement element)
        {
            bool success = true;

            Process(success, element.Clear);

            string value = null;

            Process(ref success, () => TryGetValue(element, out value));

            Process(ref success, () => { return value.Equals(string.Empty); });

            return success;
        }

        protected bool TryGetPageURL(out string pageURL)
        {
            bool success = true;

            pageURL = null;

            const string selector = "meta[name='pageURL']";

            IWebElement metaTag = null;

            Process(ref success, () => TryGetElement(By.CssSelector(selector), out metaTag, failIfMoreThanOneFound: true, logResult: true));

            string foundValue = null;

            Process(success, () => { foundValue = metaTag.GetAttribute("content"); });

            if (success)
            {
                pageURL = foundValue;
            }

            return success;

        }

        protected bool IsOnPage(string expectedValue, bool logResults = true)
        {
            bool success = true;

            //add a leading "~/" if it does not have one
            if (!expectedValue.StartsWith("~/"))
            {
                if (expectedValue.StartsWith("/"))
                {
                    expectedValue = $"~{expectedValue}";
                }
                else
                {
                    expectedValue = $"~/{expectedValue}";
                }
            }

            string selector = $"meta[name='pageURL'][content='{expectedValue}']";

            IWebElement metaTag = GetElement(By.CssSelector(selector));

            Process(ref success, () => TryGetElement(By.CssSelector(selector), out metaTag));

            string pageTitle = null;

            Process(success, () => { pageTitle = Driver.Title; });

            if (logResults)
            {
                LogObject(new { expected = expectedValue, pageTitle = pageTitle, success = success });
            }

            return success;
        }

        protected bool IsOnPage(IndexPages indexPage)
        {
            return IsOnPage(indexPage.ToString());
        }

        protected bool TryGetItemId(out int id)
        {
            bool success = false;

            id = 0;

            string foundValue = null;

            IWebElement metaTag = GetElementByName("itemId");

            if (metaTag != null)
            {
                foundValue = metaTag.GetAttribute("content");

                success = int.TryParse(foundValue, out id);
            }

            LogObject(new { id = id, success = success });

            return success;

        }

        private IWebElement GetElement(By searchParameter)
        {
            return Driver.FindElements(searchParameter).FirstOrDefault();
        }

        /// <summary>
        /// Get an element by its id attribute.
        /// </summary>
        /// <param name="id">e.g. "btnSearch"</param>
        /// <returns></returns>
        private IWebElement GetElementById(string id)
        {
            return GetElement(By.Id(id));
        }
        /// <summary>
        /// Try to get an element by its id attribute. If not found, return false.
        /// </summary>
        /// <param name="id">The id attribute, e.g. "btnSearch" in the case of id = "btnSearch"</param>
        /// <param name="element">the found element</param>
        /// <returns></returns>
        protected bool TryGetElementById(string id, out IWebElement element)
        {
            element = null;

            bool success = true;

            IWebElement foundElement = null;

            Process(success, () => { foundElement = GetElementById(id); });

            Process(ref success, () => (foundElement != null));

            if (success)
            {
                element = foundElement;
            }

            LogObject(new { id = id, success = success });

            return success;
        }


        protected bool ElementExistsByName(string name)
        {
            bool success = false;

            IWebElement element;

            element = GetElementByName(name);

            if (element != null)
            {
                success = true;
            }

            LogObject(new { name = name, success = success });

            return success;
        }
        protected bool ElementExistsById(string id)
        {
            bool success = false;

            IWebElement element;

            element = GetElementById(id);

            if (element != null)
            {
                success = true;
            }

            LogObject(new { id = id, success = success });

            return success;
        }

        protected bool ElementExistsByButtonType(string buttonType, string dataId)
        {
            bool success = false;

            IWebElement element = null;

            success = TryGetElementByButtonType(buttonType: buttonType, dataId: dataId, button: out element);

            LogObject(new { buttonType = buttonType, id = dataId, success = success });

            return success;
        }



        protected bool ElementDoesNotExistByName(string name)
        {
            bool success = false;

            IWebElement element;

            element = GetElementByName(name);

            if (element == null)
            {
                success = true;
            }

            LogObject(new { name = name, success = success });

            return success;
        }

        protected bool TryGetElementByName(string name, out IWebElement element)
        {
            bool success = false;

            element = GetElementByName(name);

            if (element != null)
            {
                success = true;
            }

            LogObject(new { name = name, success = success });

            return success;
        }

        protected bool ElementIsVisibleById(string id)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElementById(id, out element));

            Process(ref success, () => element.Displayed);

            LogObject(new { id = id, success = success });

            return success;
        }

        protected bool ElementDoesNotExistById(string id)
        {
            bool success = false;

            IWebElement element;

            element = GetElementById(id);

            if (element == null)
            {
                success = true;
            }

            LogObject(new { id = id, success = success });

            return success;
        }

        protected bool ElementDoesNotExistByButtonType(string buttonType, string id)
        {
            bool success = false;

            IWebElement element = null;

            element = GetElementByButtonType(buttonType, id);

            if (element == null)
            {
                success = true;
            }

            LogObject(new { id = id, success = success });

            return success;
        }

        private IWebElement GetElementByName(string name)
        {
            return GetElement(By.Name(name));
        }

        protected void Open(string siteRelativeURL)
        {
            LogObject(new { absoluteURL = siteRelativeURL });
            Uri uri = GetAbsoluteURI(siteRelativeURL);

            Driver.Navigate().GoToUrl(uri);

        }

        protected Uri GetAbsoluteURI(string siteRelativeURL)
        {
            //remove opening tildas
            siteRelativeURL = siteRelativeURL.TrimStart(new char[] { '~' });

            //remove opening /
            siteRelativeURL = siteRelativeURL.TrimStart(new char[] { '/' });

            return new Uri(RootURI, siteRelativeURL);
        }

        protected IWebDriver Driver
        {
            get
            {
                return Configuration.Driver;
            }
        }

        private Uri RootURI
        {
            get
            {
                return new Uri(Configuration.Server.RootURL);
            }
        }

        public ConfigurationData Configuration { get; private set; }

        /// <summary>
        /// Attempts to get the value of the element by the id of the element
        /// </summary>
        /// <param name="id">id of the element</param>
        /// <param name="value">the retrieved value</param>
        /// <returns>true if able to find the element and retrieve it's value</returns>
        protected bool TryGetValueById(string id, out string value)
        {
            bool success = true;

            value = null;

            IWebElement element = null;

            Process(ref success, () => TryGetElementById(id, out element));

            string foundValue = null;

            Process(ref success, () => TryGetValue(element, out foundValue));

            if (success)
            {
                value = foundValue;
            }
            else
            {
                value = null;
            }

            LogObject(new { id = id, value = value, success = success });

            return success;
        }


        /// <summary>
        /// Attempts to get the value of the element by the id of the element
        /// </summary>
        /// <param name="name">name of the element</param>
        /// <param name="value">the retrieved value</param>
        /// <returns>true if able to find the element and retrieve it's value</returns>
        protected bool TryGetValueByName(string name, out string value)
        {
            bool success = true;

            value = null;

            IWebElement element = null;

            Process(ref success, () => TryGetElementByName(name, out element));

            string foundValue = null;

            Process(ref success, () => TryGetValue(element, out foundValue));

            if (success)
            {
                value = foundValue;
            }
            else
            {
                value = null;
            }

            LogObject(new { name = name, value = value, success = success });

            return success;
        }

        private bool TryGetValue(IWebElement element, out string value)
        {
            bool success = true;

            string foundValue = null;

            Process(success, () => { foundValue = element.GetAttribute("value"); });

            if (success)
            {
                value = foundValue;
            }
            else
            {
                value = null;
            }

            return success;
        }
        protected bool IsTextById(string id, string expectedValue, out string foundText)
        {
            bool success = false;
            IWebElement element = Driver.FindElement(By.Id(id));
            foundText = element.Text;
            if (string.Equals(foundText, expectedValue, StringComparison.OrdinalIgnoreCase))
            {
                success = true;
            }
            return success;
        }

        protected bool IsValueByName(string name, string expectedValue)
        {
            bool success = false;

            string foundValue = GetElement(By.Name(name)).GetAttribute("value");

            if (expectedValue.Equals(foundValue))
            {
                success = true;
            }

            LogObject(new { name = name, expected = expectedValue, found = foundValue, success = success });

            return success;
        }

        /// <summary>
        /// Will retry the waitTest every .5 seconds until the result is true or timeoutSeconds is reached
        /// </summary>
        /// <param name="waitTest">A function with a bool result to try</param>
        /// <param name="timeoutSeconds">The number of seconds to retry the test</param>
        /// <returns>True if the waitTest resulted in true before the timeoutSeconds is reached, false if the timeoutSeconds was reached</returns>
        protected static bool Wait(Func<bool> waitTest, int timeoutSeconds = 4)
        {
            const int milisecondsInSeconds = 1000;

            const int sleepMiliseconds = (int)(.5 * milisecondsInSeconds);

            bool success = false;

            bool timeout = false;

            int timeoutMilliseconds = timeoutSeconds * milisecondsInSeconds;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (!timeout && !success)
            {
                success = waitTest();

                if (!success)
                {
                    if (sw.ElapsedMilliseconds > timeoutMilliseconds)
                    {
                        timeout = true;
                    }

                    Thread.Sleep(sleepMiliseconds);
                }
            }

            return success;
        }

        /// <summary>
        /// Will test for running AJAX calls until they are completed or ajaxTimeoutSeconds is reached and then will retry the waitTest every .5 seconds until the result is true or timeoutSeconds is reached
        /// </summary>
        /// <param name="waitTest">A function with a bool result to try</param>
        /// <param name="timeoutSeconds">The number of seconds to retry the test</param>
        /// <param name="ajaxTimeoutSeconds">The number of seconds to test for running AJAX calls</param>
        /// <returns>True if the waitTest resulted in true before the timeoutSeconds is reached, false if the timeoutSeconds was reached</returns>
        protected bool AjaxWait(Func<bool> waitTest, int timeoutSeconds = 4, int ajaxTimeoutSeconds = 10)
        {
            bool success = false;

            success = AjaxWait(ajaxTimeoutSeconds);

            if (success)
            {
                success = Wait(waitTest, timeoutSeconds);
            }

            return success;
        }

        protected bool AjaxWait(int timeoutSeconds = 10)
        {
            bool success = false;

            bool isRunning = false;

            Func<bool> waitTest = () => (GetIsAjaxRunning(out isRunning) && !isRunning);

            success = Wait(waitTest, timeoutSeconds);

            return success;
        }

        protected static void Wait(int seconds)
        {
            const int milisecondsInSeconds = 1000;

            int sleepMiliseconds = seconds * milisecondsInSeconds;

            Thread.Sleep(sleepMiliseconds);
        }

        protected bool GetIsAjaxRunning(out bool isRunning)
        {
            bool success = false;

            isRunning = false;

            string test = "(window.jQuery || { active: 0 }).active";

            object jsResult = GetJavascriptResult(test);

            int result = 0;

            if (jsResult != null)
            {
                success = int.TryParse(jsResult.ToString(), out result);
            }

            if (success)
            {
                isRunning = result == 1;
            }

            Configuration.LogObject(new { isRunning = isRunning, success = success });

            return success;
        }

        protected object GetJavascriptResult(string test)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;

            object jsResult = js.ExecuteScript($"return {test};");
            return jsResult;
        }

        public void SetConfiguration(ConfigurationData configuration)
        {
            this.Configuration = configuration;
        }

        protected bool TryGetElementByButtonType(string buttonType, string dataId, out IWebElement button)
        {
            bool success = false;

            string selector = $"[data-buttontype='{buttonType}'][data-id='{dataId}']";

            var buttons = Driver.FindElements(By.CssSelector(selector));

            button = buttons.SingleOrDefault();

            if (button != null)
            {
                success = true;
            }

            LogObject(new { selector = selector, success = success });

            return success;
        }

        protected bool EnsureSelectedByName(string name, bool select)
        {
            bool success = true;

            IWebElement element = null;

            Process(ref success, () => TryGetElementByName(name, out element));

            Process(ref success, () => EnsureSelected(element, select));

            return success;
        }

        protected bool EnsureSelected(IWebElement selectableWebElement, bool select)
        {
            bool success = true;

            bool isElementSelected = false;

            Process(ref success, () => TryGetIsSelected(selectableWebElement, out isElementSelected));

            Process(ref success, runCondition: isElementSelected != select, function: () =>
            {
                Process(ref success, () => Click(selectableWebElement));

                Process(ref success, () => TryGetIsSelected(selectableWebElement, out isElementSelected));

                return success;
            }
            );

            Process(ref success, () => selectableWebElement.Selected == select);

            string elementName = selectableWebElement.GetAttribute("Name");

            LogObject(new { elementName = elementName, select = select, isElementSelected = isElementSelected, success = success });

            return success;
        }

        protected bool TryGetIsSelected(IWebElement element, out bool isSelected)
        {
            bool success = true;

            Process(ref success, () => element != null);

            isSelected = false;

            if (success)
            {
                isSelected = element.Selected;
            }

            return success;
        }

        protected bool TryGetIsSelectedByName(string name, out bool isSelected)
        {
            bool success = true;

            IWebElement element = null;

            isSelected = false;

            Process(ref success, () => TryGetElementByName(name, out element));

            if (success)
            {
                success = TryGetIsSelected(element, out isSelected);
            }

            return success;
        }

        protected bool TryGetElementByButtonType(string buttonType, out IWebElement button)
        {
            bool success = false;

            string selector = $"[data-buttontype='{buttonType}']";

            var elements = Driver.FindElements(By.CssSelector(selector));

            button = elements.FirstOrDefault();

            if (button != null)
            {
                success = true;
            }

            LogObject(new { selector = selector, success = success });

            return success;
        }

        protected IWebElement GetElementByButtonType(string buttonType, string dataId)
        {
            string selector = $"[data-buttontype='{buttonType}'][data-id='{dataId}']";

            var elements = Driver.FindElements(By.CssSelector(selector));

            IWebElement button;

            button = elements?.FirstOrDefault();

            LogObject(new { buttonType = buttonType, dataId = dataId });

            return button;
        }

        protected bool TryGetLobiBoxButton(string type, out IWebElement button)
        {
            bool success = false;

            string selector = $"[data-type='{type}']";

            var buttons = Driver.FindElements(By.CssSelector(selector));

            button = buttons.FirstOrDefault();

            if (button != null)
            {
                success = true;
            }

            LogObject(new { selector = selector, success = success });

            return success;
        }

        protected bool TryGetElementsByButtonType(string buttonType, out List<IWebElement> buttons, bool failIfNoneFound = true)
        {
            bool success = true;

            buttons = null;

            string selector = $"[data-buttontype='{buttonType}']";

            List<IWebElement> foundButtons = null;

            Process(ref success, () => TryGetElements(By.CssSelector(selector), out foundButtons, failIfNoneFound));

            if (success)
            {
                buttons = foundButtons;
            }

            LogObject(new { selector = selector, success = success, count = foundButtons?.Count() });

            return success;
        }
        /// <summary>
        /// Get a list of elements using they passed-in search method.
        /// </summary>
        /// <param name="by">A search method that searches by an attribute like id, name or something else</param>
        /// <param name="elements">the elements matching the selector</param>
        /// <returns></returns>
        protected bool TryGetElements(By by, out List<IWebElement> elements, bool failIfNoneFound = true)
        {
            bool success = true;

            elements = null;

            List<IWebElement> foundElements = null;

            Process(success, () => { foundElements = Driver.FindElements(by)?.ToList(); });

            Process(ref success, runCondition: failIfNoneFound, function: () => (foundElements?.Any() == true));

            if (success)
            {
                elements = foundElements;
            }

            return success;
        }

        /// <summary>
        /// Will do an index search for the search term
        /// Expectations: You to have already navigated to the index page
        /// </summary>
        /// <param name="searchTerm">The string you want to search the index page for</param>
        /// <returns>True if it was able to perform the search and got at least one result, false if unable to perform search or not results were returned</returns>
        protected bool IndexPageSearchAll(string searchTerm, bool failIfNoResults = true)
        {
            int itemCount = 0;

            bool success = true;

            Process(ref success, () => IndexPageSearchAll(searchTerm, out itemCount));

            Process(
                success: ref success,
                runCondition: failIfNoResults,
                function: () => (itemCount > 0)
                );

            LogObject(new { success = success, failIfNoResults = failIfNoResults, itemCount = itemCount });

            return success;
        }

        /// <summary>
        /// Will do an index search for the search term
        /// Expectations: You to have already navigated to the index page
        /// </summary>
        /// <param name="searchTerm">The string you want to search the index page for</param>
        /// <param name="itemCount">Out that returns the result count</param>
        /// <returns>True if it was able to perform search (regardless if any results, expects you to check the itemCount), false if unable to perform search</returns>
        protected bool IndexPageSearchAll(string searchTerm, out int itemCount)
        {
            bool success = true;

            itemCount = 0;

            int itemCountLocal = 0;

            Process(ref success, () => InputById("txtSearch", searchTerm, clear: true));

            Process(ref success, () => Wait(() => ClickById("btnSearch"), timeoutSeconds: 10));

            Process(ref success, () => AjaxWait(() => SelectAllFilters(), timeoutSeconds: 15, ajaxTimeoutSeconds: 45)); //Note: Tested 1/22/2018. Took 34 seconds.

            Process(ref success, () => ScrollToTop());

            Process(ref success, () => int.TryParse(GetJavascriptResult("asyncLoadList.listInfo.ItemCount")?.ToString(), out itemCountLocal));

            itemCount = itemCountLocal;

            return success;
        }

        protected bool ScrollToTop()
        {
            bool success = true;

            double position = 0;

            Process(ref success, () => TryGetVerticalScrollPosition(out position));

            Process(success, runCondition: position != 0, action: () => SendHomeKey());

            Process(
                success: ref success,
                runCondition: position != 0,
                function: () => Wait(
                    () =>
                    {
                        success = TryGetVerticalScrollPosition(out position);
                        if (success)
                        {
                            success = position == 0;
                        }
                        return success;
                    }
                )
                );

            LogObject(new { position = position, success = success });

            return success;

        }

        private void SendHomeKey()
        {
            Actions actions = new Actions(Driver);
            actions.SendKeys(Keys.Home);
            actions.Perform();
        }

        private bool TryGetVerticalScrollPosition(out double position)
        {
            bool success = true;

            position = 0;

            if (success)
            {
                success = double.TryParse(GetJavascriptResult(" $(window).scrollTop()")?.ToString(), out position);
            }

            return success;
        }

        private bool SelectAllFilters()
        {
            bool success = true;

            int filterGroupCount = 0;

            Process(ref success, () => int.TryParse(GetJavascriptResult("asyncLoadList.listInfo.FilterGroupCount")?.ToString(), out filterGroupCount));

            Process(
                success: ref success,
                runCondition: filterGroupCount > 0,
                function: () =>
                {
                    List<IWebElement> selectAllFiltersButtons = null;

                    Process(ref success, () => TryGetElementsByButtonType("selectAllFilters", out selectAllFiltersButtons));

                    Process(ref success, () => ClickAllButtons(selectAllFiltersButtons));

                    return success;
                }
            );

            return success;
        }

        private bool ClickAllButtons(List<IWebElement> buttons)
        {
            bool success = true;

            foreach (var button in buttons)
            {
                Process(ref success, () => Click(button));
            }

            Process(ref success, () => AjaxWait());

            return success;
        }

        /// <summary>
        /// Will delete a quote from the quotes index page
        /// Expectations: You have already navigated to the quotes index page and performed any searches and the quote delete button is visible on the results page
        /// </summary>
        /// <param name="quoteId">Id of quote to delete</param>
        /// <returns>True if able to delete quote and return to the quotes index page, false if unable to delete quote or unable to return to the quotes index page</returns>
        protected bool DeleteQuote(int quoteId)
        {
            bool success = true;

            IWebElement deleteButton = null;

            IWebElement lobiBoxButton = null;

            Process(ref success, () => TryGetElementByButtonType("delete", quoteId.ToString(), out deleteButton));

            Process(ref success, () => Click(deleteButton));

            Process(ref success, () => TryGetLobiBoxButton("yes", out lobiBoxButton));

            Process(ref success, () => Click(lobiBoxButton));

            Process(ref success, () => AjaxWait());

            LogObject(new { quoteId = quoteId, success = success });

            return success;
        }


        /// <summary>
        /// Will delete a quote from the quotes index page
        /// Expectations: You have already navigated to the quotes index page and performed any searches and the quote delete button is visible on the results page
        /// </summary>
        /// <param name="quoteId">Id of quote to delete</param>
        /// <returns>True if able to delete quote and return to the quotes index page, false if unable to delete quote or unable to return to the quotes index page</returns>
        protected bool DeleteApplication(int applicationId)
        {
            bool success = true;

            IWebElement deleteButton = null;

            IWebElement lobiBoxButton = null;

            Process(ref success, () => TryGetElementByButtonType("delete", applicationId.ToString(), out deleteButton));

            Process(ref success, () => Click(deleteButton));

            Process(ref success, () => TryGetLobiBoxButton("yes", out lobiBoxButton));

            Process(ref success, () => Click(lobiBoxButton));

            Process(ref success, () => AjaxWait());

            LogObject(new { applicationId = applicationId, success = success });

            return success;
        }


        /// <summary>
        /// Will delete a quote from the quotes index page
        /// Expectations: You have already navigated to the quotes index page and performed any searches and the quote delete button is visible on the results page
        /// </summary>
        /// <param name="enrollmentId">Id of quote to delete</param>
        /// <returns>True if able to delete quote and return to the quotes index page, false if unable to delete quote or unable to return to the quotes index page</returns>
        protected bool DeleteEnrollment(int enrollmentId)
        {
            bool success = true;

            IWebElement deleteButton = null;

            IWebElement lobiBoxButton = null;

            Process(ref success, () => TryGetElementByButtonType("delete", enrollmentId.ToString(), out deleteButton));

            Process(ref success, () => Click(deleteButton));

            Process(ref success, () => TryGetLobiBoxButton("yes", out lobiBoxButton));

            Process(ref success, () => Click(lobiBoxButton));

            Process(ref success, () => AjaxWait());

            LogObject(new { enrollmentId = enrollmentId, success = success });

            return success;
        }

        /// <summary>
        /// Will navigate to a parent level index page using the menu
        /// Expectations: You have already logged in
        /// </summary>
        /// <param name="indexPage">The index page you want to navigate to</param>
        /// <returns>True if able to perform navigation, false if not</returns>
        protected bool GoToIndex(IndexPages indexPage)
        {
            bool success = true;

            IWebElement btnMenu = null;

            Process(ref success, () => TryGetElementById("btnMenu", out btnMenu));

            Process(ref success, () => btnMenu.Displayed ? Click(btnMenu) : true); //if menu button is displayed we are in mobile view and the nav menu is not displayed

            Process(ref success, () => ClickById($"aMenu{indexPage}Index"));

            Process(ref success, () => Wait(() => IsOnPage(indexPage), timeoutSeconds: 8));

            LogObject(new { indexPage = indexPage, success = success });

            return success;
        }

        /// <summary>
        /// Will delete a client from the clients index page
        /// Expectations: You have already navigated to the clients index page and performed any searches and the client edit button is visible on the results page
        /// </summary>
        /// <param name="clientId">Id of client to delete</param>
        /// <returns>True if able to delete client and return to the clients index page, false if unable to delete client or unable to return to the clients index page</returns>
        protected bool DeleteClient(int clientId)
        {
            bool success = true;

            IWebElement editAnchor = null;
            IWebElement deleteAnchor = null;
            IWebElement deleteButton = null;

            Process(ref success, () => TryGetElementByButtonType("edit", clientId.ToString(), out editAnchor));

            Process(ref success, () => Wait(() => Click(editAnchor)));

            Process(ref success, () => Wait(() => IsOnPage($"/Clients/{clientId}/Edit")));

            Process(ref success, () => TryGetElementByButtonType("delete", out deleteAnchor));

            Process(ref success, () => Click(deleteAnchor));

            Process(ref success, () => Wait(() => IsOnPage($"/Clients/{clientId}/Delete")));

            Process(ref success, () => TryGetElementById("btnDelete", out deleteButton));

            Process(ref success, () => Click(deleteButton));

            Process(ref success, () => Wait(() => IsOnPage(IndexPages.Clients)));

            return success;
        }

        /// <summary>
        /// Draws a test signature
        /// </summary>
        /// <param name="name">The name of the control (without .Output)</param>
        /// <param name="signature">A string of x and y coordinates like "[{\"lx\":18,\"ly\":16,\"mx\":18..."</param>
        /// <returns></returns>
        protected bool DrawSignature(string name, string signature)
        {
            bool success = true;

            string outputName = $"{name}.Output";

            IWebElement output = null;

            Process(ref success, () => TryGetElementByName(outputName, out output));

            string outputId = null;

            Process(ref success, () =>
            {
                outputId = output.GetAttribute("id");
                return !string.IsNullOrEmpty(outputId);
            }
            );

            Process(success, () =>
            {
                string script = $"$('#{outputId}').parent().signaturePad({{output:'#{outputId}'}}).regenerate({signature});";

                ((IJavaScriptExecutor)Driver).ExecuteScript(script);

            });

            Process(ref success, () =>
            {
                string foundSignature = null;

                foundSignature = output.GetAttribute("value");

                return foundSignature == signature;

            });

            LogObject(new { name = name, success = success });

            return success;
        }


        /// <summary>
        /// Will delete a policy from the policy index page
        /// Expectations: You have already navigated to the policy index page and performed any searches and the client edit button is visible on the results page
        /// </summary>
        /// <param name="policyId">Id of policy to delete</param>
        /// <returns>True if able to delete client and return to the clients index page, false if unable to delete client or unable to return to the clients index page</returns>
        protected bool DeletePolicy(int policyId)
        {
            bool success = true;

            IWebElement editAnchor = null;
            IWebElement deleteAnchor = null;
            IWebElement deleteButton = null;

            if (success && CanContinue)
            {
                success = TryGetElementByButtonType("edit", policyId.ToString(), out editAnchor);
            }

            if (success && CanContinue)
            {
                success = Wait(() => editAnchor.Displayed);
            }

            if (success && CanContinue)
            {
                editAnchor.Click();

                string page = $"/Policies/{policyId}/Edit";

                success = Wait(() => IsOnPage(page));
            }

            if (success && CanContinue)
            {
                success = TryGetElementByButtonType("delete", out deleteAnchor);
            }

            if (success && CanContinue)
            {
                deleteAnchor.Click();

                string page = $"/Policies/{policyId}/Delete";

                success = Wait(() => IsOnPage(page));
            }

            if (success && CanContinue)
            {
                success = TryGetElementById("btnDelete", out deleteButton);
            }

            if (success && CanContinue)
            {
                deleteButton.Click();

                success = IsOnPage(IndexPages.Policies);
            }

            return success;
        }

        protected bool LobiboxFound(string lobiboxMessageText)
        {
            bool success = true;

            List<IWebElement> lobiboxes = null;

            Process(ref success, () => TryGetElements(By.ClassName("lobibox"), out lobiboxes));

            Process(ref success, () =>
            {
                bool foundMessage = false;

                foreach (IWebElement lobibox in lobiboxes)
                {
                    string lobiboxMessage = lobibox.FindElement(By.ClassName("lobibox-body-text")).Text;

                    if (lobiboxMessageText.Equals(lobiboxMessage, StringComparison.OrdinalIgnoreCase))
                    {
                        foundMessage = true;
                        break;
                    }
                }

                return foundMessage;
            }
            );

            return success;
        }

        protected bool LobiboxNotifyFound(string lobiboxMessageText)
        {
            bool success = true;

            List<IWebElement> lobiboxes = null;

            Process(ref success, () => TryGetElements(By.ClassName("lobibox-notify-msg"), out lobiboxes));

            Process(ref success, () =>
            {
                bool foundMessage = false;

                foreach (IWebElement lobibox in lobiboxes)
                {
                    try
                    {
                        string lobiboxMessage = lobibox.Text;

                        if (lobiboxMessageText.Equals(lobiboxMessage, StringComparison.OrdinalIgnoreCase))
                        {
                            foundMessage = true;
                            break;
                        }
                    }
                    catch (StaleElementReferenceException)
                    {
                        LogObject(new { staleElementFound = true });
                    }
                }

                return foundMessage;
            }
            );

            return success;
        }

        public IDCache IDCache { get; set; } = new IDCache();

        protected int LastCreatedQuoteId
        {
            get
            {
                return IDCache.Last(IDCache.Types.Quote);
            }
            set
            {
                IDCache.Add(IDCache.Types.Quote, value);
            }

        }

        protected int LastCreatedClientId
        {
            get
            {
                return IDCache.Last(IDCache.Types.Client);
            }
            set
            {
                IDCache.Add(IDCache.Types.Client, value);
            }
        }

        protected int LastCreatedEnrollmentId
        {
            get
            {
                return IDCache.Last(IDCache.Types.Enrollment);
            }
            set
            {
                IDCache.Add(IDCache.Types.Enrollment, value);
            }
        }

        private DateTime lastCreatedQuoteEffectiveDate = DateTime.MinValue;

        protected DateTime LastCreatedQuoteEffectiveDate
        {
            get
            {
                if (lastCreatedQuoteEffectiveDate == DateTime.MinValue)
                {
                    lastCreatedQuoteEffectiveDate = Properties.Settings.Default.LastCreatedQuoteEffectiveDate;
                }

                return lastCreatedQuoteEffectiveDate;
            }
            set
            {
                lastCreatedQuoteEffectiveDate = value;

                Properties.Settings.Default.LastCreatedQuoteEffectiveDate = lastCreatedQuoteEffectiveDate;

                Properties.Settings.Default.Save();
            }
        }

        protected int LastCreatedSubscriberId
        {
            get
            {
                return IDCache.Last(IDCache.Types.Subscriber);
            }
            set
            {
                IDCache.Add(IDCache.Types.Subscriber, value);
            }

        }

        protected int LastCreatedApplicationId
        {
            get
            {
                return IDCache.Last(IDCache.Types.Application);
            }
            set
            {
                IDCache.Add(IDCache.Types.Application, value);
            }

        }

    }

    public abstract class WebSiteTestRunnerBase : WebSiteTestBase, IWebSiteTestRunner
    {

        public WebSiteTestRunnerBase(ConfigurationData configurationData) : base(configurationData)
        {

        }

        public WebSiteTestRunnerBase() : base()
        {

        }

        public void Run()
        {
            try
            {
                Reset();

                bool success = true;

                //start on window one
                Driver.SwitchTo().Window(Driver.WindowHandles[0]);

                Log("Running test");

                Process(ref success, RunTest);

                if (WasCancelled)
                {
                    Log($"cancelled={true}");
                }
                else
                {
                    Log($"success={success}");
                }
            }
            catch (Exception ex)
            {
                Configuration.LogObject(ex);
            }
            finally
            {
                Configuration.Completed?.Invoke();
            }
        }

        public abstract bool RunTest();
    }

    public class IDCache
    {
        public static class Types
        {
            public const string Quote = "QuoteId";
            public const string Client = "ClientId";
            public const string Application = "ApplicationId";
            public const string Enrollment = "ClientEnrollmentId";
            public const string Subscriber = "SubscriberId";
            public const string Policy = "PolicyId";
            public const string PolicyPayment = "PolicyPaymentId";
            public const string PolicyProduct = "PolicyProductId";
        }

        public IDCache()
        {
            SetupIdList();
        }

        public void Add(string type, int id)
        {
            //need to always add to the top of the list, so remove if exists
            RemoveIfExists(type, id);

            IdList.Add(new IdItem { Type = type, Id = id });

            CleanIdList(type);

            SaveIdList();

        }

        private void RemoveIfExists(string type, int id)
        {
            var foundId = IdList.FirstOrDefault(idItem => idItem.Type == type && idItem.Id == id);

            if (foundId != null)
            {
                IdList.Remove(foundId);
            }
        }

        private const int numberOfIdsToKeep = 10;

        private void CleanIdList(string type)
        {
            while (IdList.Count(idItem => idItem.Type == type) > numberOfIdsToKeep)
            {
                var itemToRemove = IdList.Where(idItem => idItem.Type == type).OrderBy(idItem => idItem.Id).First();

                IdList.Remove(itemToRemove);
            }
        }

        public int Last(string type)
        {

            int maxId = IdList.LastOrDefault(idItem => idItem.Type == type)?.Id ?? 0;

            return maxId;

        }

        private void SaveIdList()
        {
            var serializer = new XmlSerializer(typeof(IdItemList));
            var idListStringBuilder = new StringBuilder();

            using (TextWriter writer = new StringWriter(idListStringBuilder))
            {
                serializer.Serialize(writer, IdList);
            }

            Properties.Settings.Default.IDList = idListStringBuilder.ToString();

            Properties.Settings.Default.Save();
        }

        private static IdItemList IdList = null;

        private void SetupIdList()
        {
            if (IdList == null)
            {
                var idListData = Properties.Settings.Default.IDList;

                if (string.IsNullOrEmpty(idListData))
                {
                    IdList = new IDCache.IdItemList();
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(IdItemList));

                    using (TextReader reader = new StringReader(idListData))
                    {
                        IdList = (IdItemList)serializer.Deserialize(reader);
                    }
                }
            }
        }

        public class IdItemList : List<IdItem>
        {

        }

        public class IdItem
        {
            public string Type { get; set; }

            public int Id { get; set; }
        }
    }

    public enum IndexPages
    {
        Quotes,
        Clients,
        Enrollments,
        Policies,
        Applications,
    }

    public class Subscriber
    {
        public string Name { get; set; } = "Automated Test Employee";
        public DateTime? DateOfBirth { get; set; } = new DateTime(1979, 3, 1);
        public GenderType Gender { get; set; } = GenderType.Undefined;

        public enum GenderType
        {
            Undefined,
            Male,
            Female
        }

        public bool UsesTobacco { get; set; } = false;

        public bool Waive { get; set; } = false;

        public Subscriber Spouse { get; set; }

        public Subscriber[] Dependents { get; set; }


    }
}
