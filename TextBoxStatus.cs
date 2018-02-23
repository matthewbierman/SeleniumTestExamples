

using System.Windows.Forms;

namespace abxch.WebsiteUITest
{
    public partial class TextBoxStatus : TextBox
    {
        private const string newLine = "\r\n";

        private delegate void StringParameterDelegate(string value);

        private delegate void NoParameterDelegate();

        public TextBoxStatus()
        {
            InitializeComponent();
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringParameterDelegate(UpdateStatus), new object[] { message });
            }
            else
            {
                string currentText = this.Text;

                if (currentText.IndexOf(newLine) != -1)
                {
                    currentText = currentText.Substring(0, currentText.LastIndexOf(newLine));
                    this.Text = currentText;
                }
                else
                {
                    this.Text = string.Empty;
                }

                if (this.Text != string.Empty)
                {
                    message = newLine + message;
                }

                this.AppendText(message);
                this.Update();
            }
        }

        [System.Diagnostics.DebuggerStepThrough()]
        public void AddStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringParameterDelegate(AddStatus), new object[] { message });
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    if (this.Text == string.Empty)
                    {
                        this.Text = message;
                        this.Update();
                    }
                    else
                    {
                        message = newLine + message;
                        this.AppendText(message);
                        this.Update();
                    }
                }
            }
        }

        public void AddEmptyLine()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new NoParameterDelegate(AddEmptyLine));
                return;
            }

            string message = newLine;
            this.AppendText(message);
            this.Update();
        }

        public void AddStatus(string message, params object[] args)
        {
            AddStatus(string.Format(message, args));
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TextBoxStatus
            // 
            this.Multiline = true;
            this.ReadOnly = true;
            this.ResumeLayout(false);

        }
    }
}

