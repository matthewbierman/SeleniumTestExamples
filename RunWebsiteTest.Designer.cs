namespace abxch.WebsiteUITest
{
    partial class RunWebsiteTest
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunWebsiteTest));
            this.label1 = new System.Windows.Forms.Label();
            this.cbWebsite = new System.Windows.Forms.ComboBox();
            this.cbBrowser = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbTest = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStopAllRunningWebDrivers = new System.Windows.Forms.Button();
            this.chkClearLogOnNewTest = new System.Windows.Forms.CheckBox();
            this.txtStartTestAtStep = new System.Windows.Forms.TextBox();
            this.btnStartAtStep = new System.Windows.Forms.Button();
            this.chkPauseOnStep = new System.Windows.Forms.CheckBox();
            this.txtPauseOnStep = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.pbScreenShot = new System.Windows.Forms.PictureBox();
            this.txtStatus = new abxch.WebsiteUITest.TextBoxStatus();
            this.btnOpenLogLocation = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenShot)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Website";
            // 
            // cbWebsite
            // 
            this.cbWebsite.DisplayMember = "Name";
            this.cbWebsite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWebsite.FormattingEnabled = true;
            this.cbWebsite.Location = new System.Drawing.Point(138, 11);
            this.cbWebsite.Name = "cbWebsite";
            this.cbWebsite.Size = new System.Drawing.Size(196, 21);
            this.cbWebsite.TabIndex = 1;
            this.cbWebsite.ValueMember = "Value";
            this.cbWebsite.SelectedIndexChanged += new System.EventHandler(this.cbWebsite_SelectedIndexChanged);
            // 
            // cbBrowser
            // 
            this.cbBrowser.DisplayMember = "Name";
            this.cbBrowser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBrowser.FormattingEnabled = true;
            this.cbBrowser.Location = new System.Drawing.Point(391, 11);
            this.cbBrowser.Name = "cbBrowser";
            this.cbBrowser.Size = new System.Drawing.Size(266, 21);
            this.cbBrowser.TabIndex = 3;
            this.cbBrowser.UseWaitCursor = true;
            this.cbBrowser.ValueMember = "Value";
            this.cbBrowser.SelectedIndexChanged += new System.EventHandler(this.cbBrowser_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Browser";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(21, 143);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new System.Drawing.Point(105, 197);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(75, 23);
            this.btnClearLog.TabIndex = 6;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(186, 143);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(105, 143);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 8;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(267, 143);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 9;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(24, 198);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(70, 18);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "Stopped";
            // 
            // cbTest
            // 
            this.cbTest.DisplayMember = "Name";
            this.cbTest.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTest.FormattingEnabled = true;
            this.cbTest.Location = new System.Drawing.Point(138, 39);
            this.cbTest.Name = "cbTest";
            this.cbTest.Size = new System.Drawing.Size(519, 21);
            this.cbTest.TabIndex = 12;
            this.cbTest.UseWaitCursor = true;
            this.cbTest.ValueMember = "Value";
            this.cbTest.SelectedIndexChanged += new System.EventHandler(this.cbTest_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Test";
            // 
            // btnStopAllRunningWebDrivers
            // 
            this.btnStopAllRunningWebDrivers.Location = new System.Drawing.Point(348, 143);
            this.btnStopAllRunningWebDrivers.Name = "btnStopAllRunningWebDrivers";
            this.btnStopAllRunningWebDrivers.Size = new System.Drawing.Size(143, 23);
            this.btnStopAllRunningWebDrivers.TabIndex = 13;
            this.btnStopAllRunningWebDrivers.Text = "Stop All WebDrivers";
            this.btnStopAllRunningWebDrivers.UseVisualStyleBackColor = true;
            this.btnStopAllRunningWebDrivers.Click += new System.EventHandler(this.btnStopAllRunningWebDrivers_Click);
            // 
            // chkClearLogOnNewTest
            // 
            this.chkClearLogOnNewTest.AutoSize = true;
            this.chkClearLogOnNewTest.Location = new System.Drawing.Point(195, 201);
            this.chkClearLogOnNewTest.Name = "chkClearLogOnNewTest";
            this.chkClearLogOnNewTest.Size = new System.Drawing.Size(125, 17);
            this.chkClearLogOnNewTest.TabIndex = 14;
            this.chkClearLogOnNewTest.Text = "Clear log on new test";
            this.chkClearLogOnNewTest.UseVisualStyleBackColor = true;
            this.chkClearLogOnNewTest.CheckedChanged += new System.EventHandler(this.chkClearLogOnNewTest_CheckedChanged);
            // 
            // txtStartTestAtStep
            // 
            this.txtStartTestAtStep.Location = new System.Drawing.Point(138, 92);
            this.txtStartTestAtStep.Name = "txtStartTestAtStep";
            this.txtStartTestAtStep.Size = new System.Drawing.Size(519, 20);
            this.txtStartTestAtStep.TabIndex = 16;
            this.txtStartTestAtStep.TextChanged += new System.EventHandler(this.txtStartTestAtStep_TextChanged);
            // 
            // btnStartAtStep
            // 
            this.btnStartAtStep.Location = new System.Drawing.Point(21, 91);
            this.btnStartAtStep.Name = "btnStartAtStep";
            this.btnStartAtStep.Size = new System.Drawing.Size(108, 23);
            this.btnStartAtStep.TabIndex = 17;
            this.btnStartAtStep.Text = "Start Test At Step:";
            this.btnStartAtStep.UseVisualStyleBackColor = true;
            this.btnStartAtStep.Click += new System.EventHandler(this.btnStartAtStep_Click);
            // 
            // chkPauseOnStep
            // 
            this.chkPauseOnStep.AutoSize = true;
            this.chkPauseOnStep.Location = new System.Drawing.Point(21, 120);
            this.chkPauseOnStep.Name = "chkPauseOnStep";
            this.chkPauseOnStep.Size = new System.Drawing.Size(98, 17);
            this.chkPauseOnStep.TabIndex = 18;
            this.chkPauseOnStep.Text = "Pause On Step";
            this.chkPauseOnStep.UseVisualStyleBackColor = true;
            this.chkPauseOnStep.CheckedChanged += new System.EventHandler(this.chkPauseOnStep_CheckedChanged);
            // 
            // txtPauseOnStep
            // 
            this.txtPauseOnStep.Location = new System.Drawing.Point(138, 117);
            this.txtPauseOnStep.Name = "txtPauseOnStep";
            this.txtPauseOnStep.Size = new System.Drawing.Size(519, 20);
            this.txtPauseOnStep.TabIndex = 19;
            this.txtPauseOnStep.TextChanged += new System.EventHandler(this.txtPauseOnStep_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Log Path";
            // 
            // txtLogPath
            // 
            this.txtLogPath.Location = new System.Drawing.Point(138, 65);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.Size = new System.Drawing.Size(438, 20);
            this.txtLogPath.TabIndex = 21;
            this.txtLogPath.TextChanged += new System.EventHandler(this.txtLogPath_TextChanged);
            // 
            // pbScreenShot
            // 
            this.pbScreenShot.Location = new System.Drawing.Point(663, 11);
            this.pbScreenShot.Name = "pbScreenShot";
            this.pbScreenShot.Size = new System.Drawing.Size(347, 164);
            this.pbScreenShot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbScreenShot.TabIndex = 22;
            this.pbScreenShot.TabStop = false;
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(24, 226);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(986, 507);
            this.txtStatus.TabIndex = 5;
            // 
            // btnOpenLogLocation
            // 
            this.btnOpenLogLocation.Location = new System.Drawing.Point(582, 63);
            this.btnOpenLogLocation.Name = "btnOpenLogLocation";
            this.btnOpenLogLocation.Size = new System.Drawing.Size(75, 23);
            this.btnOpenLogLocation.TabIndex = 23;
            this.btnOpenLogLocation.Text = "Open";
            this.btnOpenLogLocation.UseVisualStyleBackColor = true;
            this.btnOpenLogLocation.Click += new System.EventHandler(this.btnOpenLogLocation_Click);
            // 
            // RunWebsiteTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 745);
            this.Controls.Add(this.btnOpenLogLocation);
            this.Controls.Add(this.pbScreenShot);
            this.Controls.Add(this.txtLogPath);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPauseOnStep);
            this.Controls.Add(this.chkPauseOnStep);
            this.Controls.Add(this.btnStartAtStep);
            this.Controls.Add(this.txtStartTestAtStep);
            this.Controls.Add(this.chkClearLogOnNewTest);
            this.Controls.Add(this.btnStopAllRunningWebDrivers);
            this.Controls.Add(this.cbTest);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnClearLog);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.cbBrowser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbWebsite);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RunWebsiteTest";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "ABXCH Run Website Test";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.RunWebsiteTest_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreenShot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbWebsite;
        private System.Windows.Forms.ComboBox cbBrowser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRun;
        private TextBoxStatus txtStatus;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cbTest;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnStopAllRunningWebDrivers;
        private System.Windows.Forms.CheckBox chkClearLogOnNewTest;
        private System.Windows.Forms.TextBox txtStartTestAtStep;
        private System.Windows.Forms.Button btnStartAtStep;
        private System.Windows.Forms.CheckBox chkPauseOnStep;
        private System.Windows.Forms.TextBox txtPauseOnStep;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.PictureBox pbScreenShot;
        private System.Windows.Forms.Button btnOpenLogLocation;
    }
}