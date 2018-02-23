using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static abxch.WebsiteUITest.ConfigurationData;

namespace abxch.WebsiteUITest
{
    public partial class RunWebsiteTest : Form
    {
        private ConfigurationData testConfiguration;
        private string logFolderPath;
        private StreamWriter logWriter = null;
        private bool loadingControls = false;
        private string tempLogFilePath;
        private States State { get; set; } = States.Stopped;
        public StreamWriter tempLogWriter { get; private set; }


        public RunWebsiteTest()
        {
            InitializeComponent();

            Setup();
        }

        public class ListItem
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private void Setup()
        {
            loadingControls = true;

            SetupTempLogPath();

            LoadLastTempLog();

            SetupTempLogFileWriter();

            SetupWebsiteComboBox();

            SetupBrowserComboBox();

            SetupTestComboBox();

            this.State = States.Stopped;

            LoadLastUsedSettings();

            loadingControls = false;

            Format();
        }

        private void SetupTempLogFileWriter()
        {
            tempLogWriter = System.IO.File.AppendText(tempLogFilePath);
        }

        private void LoadLastTempLog()
        {
            if (System.IO.File.Exists(tempLogFilePath))
            {
                string log = System.IO.File.ReadAllText(tempLogFilePath);
                txtStatus.Text = log;
            }
        }

        private void SetupTempLogPath()
        {
            string fileName = "abxchWebsiteUITestTempLog.txt";

            tempLogFilePath = System.IO.Path.Combine(Environment.GetEnvironmentVariable("TEMP"), fileName);
        }

        private void SetupLogFolder(ServerTypes serverType, BrowserTypes browserType, string testName)
        {
            string folder = $"{DateTime.Now.ToString("yyyy-MM-dd")}-{serverType}-{browserType}-{testName}";

            logFolderPath = System.IO.Path.Combine(txtLogPath.Text, folder);

            System.IO.Directory.CreateDirectory(logFolderPath);
        }

        private void SetupTestComboBox()
        {
            var typeWebSiteTestInterface = typeof(IWebSiteTestRunner);

            var testTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(t=> t.IsAbstract==false && t.GetInterfaces().Contains(typeWebSiteTestInterface))
                ;

            var testTypeItems = testTypes.Select(tt => new ListItem { Name = tt.DisplayName(), Value = tt.FullName })
                .OrderBy(tt => tt.Name)
                .ToArray();

            cbTest.Items.AddRange(testTypeItems);


        }

        private void SetupBrowserComboBox()
        {
            var browserTypes = typeof(BrowserTypes).GetValues<BrowserTypes>()
                .Select(bt => new ListItem { Name = bt.DisplayName(), Value = bt.ToString() })
                .OrderBy(bt => bt.Name)
                .ToArray();

            cbBrowser.Items.AddRange(browserTypes);


        }

        private void SetupWebsiteComboBox()
        {
            var serverTypes = typeof(ServerTypes).GetValues<ServerTypes>()
               .Select(st => new ListItem { Name = st.DisplayName(), Value = st.ToString() })
               .OrderBy(st => st.Name)
               .ToArray();

            cbWebsite.Items.AddRange(serverTypes);

        }

        private void LoadLastUsedSettings()
        {
            SetComboBoxDefault(cbTest, Properties.Settings.Default.Test);
            SetComboBoxDefault(cbWebsite, Properties.Settings.Default.Website);
            SetComboBoxDefault(cbBrowser, Properties.Settings.Default.Browser);
            chkClearLogOnNewTest.Checked = Properties.Settings.Default.ClearLogOnNewTest;
            txtStartTestAtStep.Text = Properties.Settings.Default.StartTestAtStep;
            chkPauseOnStep.Checked = Properties.Settings.Default.PauseOnStep;
            txtPauseOnStep.Text = Properties.Settings.Default.PauseOnStepValue;
            txtLogPath.Text = Properties.Settings.Default.LogPath;
        }

        private static void SetComboBoxDefault(ComboBox comboBox, string lastUsedTest)
        {
            ListItem selectedItem = comboBox.Items.OfType<ListItem>().FirstOrDefault(i => i.Value == lastUsedTest);

            if (selectedItem != null)
            {
                comboBox.SelectedItem = selectedItem;
            }
            else
            {
                comboBox.SelectedIndex = 0;
            }
        }

        public void Log(string message)
        {
            tempLogWriter.WriteLine(message);

            if (logWriter != null)
            {
                logWriter.WriteLine($"{DateTime.Now.ToString("HH:mm:ss")} {message}");
            }

            Debug.WriteLine(message);
            txtStatus.AddStatus(message);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (State == States.Paused)
            {
                State = States.Running;
            }
            else
            {
                InitializeTest();

                Task testTask = Task.Run(new Action(Test.Run));

                this.State = States.Running;
            }

            Format();
        }

        private delegate void formatDelegate();

        private void Format()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new formatDelegate(Format));
            }
            else
            {
                cbBrowser.Enabled = false;
                cbWebsite.Enabled = false;
                cbTest.Enabled = false;
                btnPause.Enabled = false;
                btnNext.Enabled = false;
                btnStop.Enabled = false;
                btnRun.Enabled = false;
                btnStartAtStep.Enabled = false;
                txtLogPath.Enabled = false;

                lblStatus.Text = State.DisplayName();

                switch (State)
                {
                    case States.Stopped:
                    {
                        cbBrowser.Enabled = true;
                        cbWebsite.Enabled = true;
                        cbTest.Enabled = true;
                        btnNext.Enabled = true;
                        btnRun.Enabled = true;
                        btnStartAtStep.Enabled = true;
                        txtLogPath.Enabled = true;
                    }
                    break;
                    case States.Running:
                    {
                        btnPause.Enabled = true;
                        btnStop.Enabled = true;
                    }
                    break;
                    case States.Paused:
                    {
                        btnNext.Enabled = true;
                        btnStop.Enabled = true;
                        btnRun.Enabled = true;
                    }
                    break;
                    case States.Next:
                    {
                        btnStop.Enabled = true;
                        btnRun.Enabled = true;
                    }
                    break;
                    case States.Skipping:
                    {
                        btnPause.Enabled = true;
                        btnStop.Enabled = true;
                        btnRun.Enabled = true;
                    }
                    break;
                }
            }

        }

        private void TestCompleted()
        {
            this.State = States.Stopped;

            CloseLog();

            Format();
        }


     

        private void InitializeTest()
        {
            ServerTypes serverType = (ServerTypes)Enum.Parse(typeof(ServerTypes), ((ListItem)cbWebsite.SelectedItem).Value);

            BrowserTypes browserType = (BrowserTypes)Enum.Parse(typeof(BrowserTypes), ((ListItem)cbBrowser.SelectedItem).Value);

            SetupTestConfiguration(serverType, browserType);

            Type testType = Type.GetType(((ListItem)cbTest.SelectedItem).Value.ToString());

            Test = (IWebSiteTestRunner)Activator.CreateInstance(testType);

            Test.SetConfiguration(testConfiguration);

            if (chkClearLogOnNewTest.Checked)
            {
                ClearTempLog();
            }

            SetupLogFolder(serverType, browserType, testType.DisplayName());

            SetupLogWriter();

        }

        private void SetupTestConfiguration(ServerTypes serverType, BrowserTypes browserType)
        {
            if (testConfiguration == null)
            {
                testConfiguration = new ConfigurationData(serverType, browserType);

                testConfiguration.Log = Log;

                testConfiguration.Completed = TestCompleted;

                testConfiguration.Cancel = delegate () { return State == States.Cancelling; };

                testConfiguration.Pause = TestPause;

                testConfiguration.Skip = TestSkip;

                testConfiguration.PauseOnStep = TestPauseOnStep;

                testConfiguration.ProcessScreenShot = TestProcessScreenShot;

                testConfiguration.ProcessFile = TestProcessFile;
            }
            else
            {
                testConfiguration.SetBrowser(browserType);

                testConfiguration.SetServer(serverType);
            }
        }

        private void TestProcessFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            string newFilePath = Path.Combine(logFolderPath, fileName);

            File.Copy(filePath, newFilePath, true);
        }

        private void SetupLogWriter()
        {
            string logFileName = "abxchWebsiteUITestLog.txt";

            string logFilePath = System.IO.Path.Combine(logFolderPath, logFileName);
            
            logWriter = System.IO.File.AppendText(logFilePath);

        }

        private void TestProcessScreenShot(Screenshot screenshot, string fileName)
        {
            string screenShotPath = System.IO.Path.Combine(logFolderPath, fileName);

            screenshot.SaveAsFile(screenShotPath,ScreenshotImageFormat.Jpeg);

            pbScreenShot.Load(screenShotPath);
        }

        private bool TestSkip(string step)
        {
            bool skip = false;

            if (State == States.Skipping)
            {
                if (txtStartTestAtStep.Text.Equals(step, StringComparison.OrdinalIgnoreCase))
                {
                    skip = false;
                    State = States.Running;
                    Format();
                }
                else
                {
                    skip = true;
                }
            }

            return skip;
        }

        private bool TestPause()
        {
            bool pause = false;

            if (State == States.Paused)
            {
                pause = true;
            }
            else if (State == States.Next)
            {
                pause = false;
                State = States.Paused;
                Format();
            }

            return pause;
        }

        private bool TestPauseOnStep(string step)
        {
            bool pause = false;

            if (State == States.Running)
            {
                if (chkPauseOnStep.Checked && txtPauseOnStep.Text.Equals(step, StringComparison.OrdinalIgnoreCase))
                {
                    pause = true;
                    State = States.Paused;
                    Format();
                }
            }

            return pause;
        }

        private IWebSiteTestRunner Test { get; set; }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            ClearTempLog();
        }

        private void ClearTempLog()
        {
            txtStatus.Clear();

            tempLogWriter.Close();

            System.IO.File.Create(tempLogFilePath).Close(); //create a new empty log file

            SetupTempLogFileWriter();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (State == States.Stopped)
            {
                InitializeTest();

                this.State = States.Paused;

                Task testTask = Task.Run(new Action(Test.Run));
            }

            if (State == States.Paused)
            {
                State = States.Next;
                Format();
            }

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (State == States.Running)
            {
                State = States.Paused;
                Format();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (State == States.Running || State == States.Paused)
            {
                State = States.Cancelling;
                Format();
            }
        }

        public enum States
        {
            Cancelling,
            Stopped,
            Running,
            Paused,
            Next,
            Skipping,
        }

        private void SaveLastSelectionSettings()
        {
            if (!loadingControls)
            {
                bool saveSettings = false;

                string browser = ((ListItem)cbBrowser.SelectedItem)?.Value;

                if (browser != null)
                {
                    Properties.Settings.Default.Browser = browser;
                    saveSettings = true;
                }

                string test = ((ListItem)cbTest.SelectedItem)?.Value;

                if (test != null)
                {
                    Properties.Settings.Default.Test = test;
                    saveSettings = true;
                }

                string website = ((ListItem)cbWebsite.SelectedItem)?.Value;

                if (website != null)
                {
                    Properties.Settings.Default.Website = website;
                    saveSettings = true;
                }

                bool clearLogOnNewTest = chkClearLogOnNewTest.Checked;

                if (Properties.Settings.Default.ClearLogOnNewTest != clearLogOnNewTest)
                {
                    Properties.Settings.Default.ClearLogOnNewTest = clearLogOnNewTest;
                    saveSettings = true;
                }

                string startAtStep = txtStartTestAtStep.Text;

                if (Properties.Settings.Default.StartTestAtStep != startAtStep)
                {
                    Properties.Settings.Default.StartTestAtStep = startAtStep;
                    saveSettings = true;
                }

                bool pauseOnStep = chkPauseOnStep.Checked;

                if (Properties.Settings.Default.PauseOnStep != pauseOnStep)
                {
                    Properties.Settings.Default.PauseOnStep = pauseOnStep;
                    saveSettings = true;
                }

                string pauseOnStepValue = txtPauseOnStep.Text;

                if (Properties.Settings.Default.PauseOnStepValue != pauseOnStepValue)
                {
                    Properties.Settings.Default.PauseOnStepValue = pauseOnStepValue;
                    saveSettings = true;
                }

                string logPath = txtLogPath.Text;

                if (Properties.Settings.Default.LogPath != logPath)
                {
                    Properties.Settings.Default.LogPath = logPath;
                    saveSettings = true;
                }

                if (saveSettings)
                {
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void cbWebsite_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void cbBrowser_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void cbTest_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void btnStopAllRunningWebDrivers_Click(object sender, EventArgs e)
        {
            var results = RemoteWebDriverExtended.KillAllRunningWebDrivers();

            foreach (var result in results)
            {
                Log($"Name: '{result.Name}', Found: '{result.Found}', KIlled: '{result.Killed}', Success: '{result.Success}', ");
            }

        }

        private void chkClearLogOnNewTest_CheckedChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void txtStartTestAtStep_TextChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void btnStartAtStep_Click(object sender, EventArgs e)
        {
            if (State == States.Paused)
            {
                State = States.Skipping;
            }
            else
            {
                InitializeTest();

                Task testTask = Task.Run(new Action(Test.Run));

                this.State = States.Skipping;
            }

            Format();
        }

        private void chkPauseOnStep_CheckedChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void txtPauseOnStep_TextChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void RunWebsiteTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseTempLog();
        }

        private void CloseTempLog()
        {
            tempLogWriter.Close();
        }

        private void CloseLog()
        {
            logWriter.Close();

            logWriter = null;
        }

        private void txtLogPath_TextChanged(object sender, EventArgs e)
        {
            SaveLastSelectionSettings();
        }

        private void btnOpenLogLocation_Click(object sender, EventArgs e)
        {
            string logFolderPath = txtLogPath.Text;

            System.IO.Directory.CreateDirectory(logFolderPath);

            Process.Start(logFolderPath);
        }
    }
}
