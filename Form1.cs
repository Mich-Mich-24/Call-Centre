using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Assignment1_ONT412
{
    public partial class Form1 : Form
    {
        private CallLog _callLog = null!;
        private ICallOperation _callOperationProxy = null!;
        private Call? _currentCall;
        private ICallLogIterator _callLogIterator = null!;

        // UI Controls
        private readonly GroupBox grpCallInitiation;
        private readonly Label lblCaller;
        private readonly TextBox txtCaller;
        private readonly Label lblReceiver;
        private readonly TextBox txtReceiver;
        private readonly Button btnMakeCall;
        private readonly Button btnDropCall;

        private readonly GroupBox grpCallActions;
        private readonly Button btnSpeak;
        private readonly Button btnHold;
        private readonly Button btnHangUp;

        private readonly GroupBox grpCurrentCallStatus;
        private readonly Label lblCurrentCallStatus;
        private readonly Label lblCurrentCallDuration;

        private readonly GroupBox grpCallLogManagement;
        private readonly ListBox lstCallLog;
        private readonly TextBox txtSearchCallLog;
        private readonly Button btnSearchCallLog;
        private readonly Button btnNextCall;
        private readonly Button btnPreviousCall;
        private readonly Button btnReturnCall;

        private readonly GroupBox grpAuthentication;
        private readonly Label lblUsername;
        private readonly TextBox txtUsername;
        private readonly Label lblPassword;
        private readonly TextBox txtPassword;
        private readonly Button btnLogin;
        private readonly Label lblLoginStatus;
        private readonly Label lblSelectRole;
        private readonly ComboBox cmbSelectRole;
        private readonly Button btnLogout;

        private readonly System.Windows.Forms.Timer timerCallDuration;


        public Form1()
        {
            InitializeComponent();

            grpAuthentication = new GroupBox { Text = "Authentication", Location = new Point(10, 10), Size = new Size(300, 190), BackColor = Color.WhiteSmoke }; 
            
            lblUsername = new Label { Text = "Username :", Location = new Point(20, 25), AutoSize = false, Size = new Size(80, 23) };
            txtUsername = new TextBox { Location = new Point(100, 22), Width = 195, Size = new Size(195, 23) };
            lblPassword = new Label { Text = "Password :", Location = new Point(15, 55), AutoSize = false, Size = new Size(70, 23) };
            txtPassword = new TextBox { Location = new Point(90, 52), Width = 195, Size = new Size(195, 23), UseSystemPasswordChar = true };
            btnLogin = new Button { Text = "Login", Location = new Point(15, 85), Size = new Size(130, 25) };
            btnLogout = new Button { Text = "Logout", Location = new Point(150, 85), Size = new Size(130, 25), Enabled = false }; 
            lblLoginStatus = new Label { Text = "Please log in.", Location = new Point(15, 120), AutoSize = false, Size = new Size(270, 20), ForeColor = Color.Blue }; 

            lblSelectRole = new Label { Text = "Select Role:", Location = new Point(15, 150), AutoSize = false, Size = new Size(70, 23) };
            cmbSelectRole = new ComboBox { Text = "Select Role", Location = new Point(90, 147), Size = new Size(195, 23), DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };

            grpCallActions = new GroupBox { Text = "Current Call Actions", Location = new Point(320, 10), Size = new Size(350, 70), BackColor = Color.WhiteSmoke };
            btnSpeak = new Button { Text = "Speak", Location = new Point(15, 25), Size = new Size(100, 25) };
            btnSpeak.Click += btnSpeak_Click; 
            btnHold = new Button { Text = "Hold", Location = new Point(125, 25), Size = new Size(100, 25) };
            btnHold.Click += btnHold_Click; 
            btnHangUp = new Button { Text = "Hang Up", Location = new Point(235, 25), Size = new Size(100, 25) };
            btnHangUp.Click += btnHangUp_Click; 

            grpCallInitiation = new GroupBox { Text = "Call Initiation", Location = new Point(10, 200), Size = new Size(300, 120), BackColor = Color.WhiteSmoke }; 
            lblCaller = new Label { Text = "Caller:", Location = new Point(15, 25), AutoSize = false, Size = new Size(60, 23) }; 
            txtCaller = new TextBox { Location = new Point(85, 22), Width = 200, Size = new Size(200, 23) };
            lblReceiver = new Label { Text = "Receiver:", Location = new Point(15, 55), AutoSize = false, Size = new Size(60, 23) }; 
            txtReceiver = new TextBox { Location = new Point(85, 52), Width = 200, Size = new Size(200, 23) };
            btnMakeCall = new Button { Text = "Make Call", Location = new Point(15, 85), Size = new Size(130, 25) };
            btnMakeCall.Click += btnMakeCall_Click; 
            btnDropCall = new Button { Text = "Drop Call", Location = new Point(150, 85), Size = new Size(130, 25) };
            btnDropCall.Click += btnDropCall_Click; 

            grpCurrentCallStatus = new GroupBox { Text = "Current Call Info", Location = new Point(320, 200), Size = new Size(350, 120), BackColor = Color.WhiteSmoke }; 
            lblCurrentCallStatus = new Label { Text = "Status: None", Location = new Point(15, 25), AutoSize = false, Size = new Size(320, 20) };
            lblCurrentCallDuration = new Label { Text = "Duration: 00:00:00", Location = new Point(15, 50), AutoSize = false, Size = new Size(320, 20) };

            grpCallLogManagement = new GroupBox { Text = "Call Log", Location = new Point(10, 330), Size = new Size(660, 200), BackColor = Color.WhiteSmoke }; 
            lstCallLog = new ListBox { Location = new Point(10, 25), Size = new Size(640, 120) };
            txtSearchCallLog = new TextBox { Text = "Search term...", Location = new Point(10, 155), Width = 180, Size = new Size(180, 23) };
            btnSearchCallLog = new Button { Text = "Search", Location = new Point(195, 155), Size = new Size(80, 25) };
            btnNextCall = new Button { Text = "Next Call", Location = new Point(300, 155), Size = new Size(100, 25) };
            btnPreviousCall = new Button { Text = "Previous Call", Location = new Point(410, 155), Size = new Size(100, 25), Enabled = true }; 
            btnReturnCall = new Button { Text = "Return Call", Location = new Point(530, 25), Size = new Size(100, 25), Enabled = false }; 

            timerCallDuration = new System.Windows.Forms.Timer();


            InitializeDesignPatterns();
            InitializeUIComponents();
            _callLog.LoadCallLog();
            UpdateCallLogDisplay();
            UpdateCurrentCallDisplay();
            UpdateControlStates();

            this.FormClosing += Form1_FormClosing; 
            this.Text = "Call Center Management System";
            this.Size = new Size(700, 560); 
            this.MinimumSize = new Size(715, 560);
            this.BackColor = Color.LightGray; 
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _callLog.SaveCallLog();
        }

        private void InitializeDesignPatterns()
        {
            _callLog = new CallLog("Main Call Log");
            _callOperationProxy = new CallProxy(_callLog, UserRole.None);
            _callLogIterator = _callLog.CreateIterator();
        }

        private void InitializeUIComponents()
        {
            grpAuthentication.Controls.Add(lblUsername);
            grpAuthentication.Controls.Add(txtUsername);
            grpAuthentication.Controls.Add(lblPassword);
            grpAuthentication.Controls.Add(txtPassword);
            grpAuthentication.Controls.Add(btnLogin);
            grpAuthentication.Controls.Add(lblLoginStatus);
            grpAuthentication.Controls.Add(lblSelectRole);
            grpAuthentication.Controls.Add(cmbSelectRole);
            grpAuthentication.Controls.Add(btnLogout);
            this.Controls.Add(grpAuthentication);

            grpCallActions.Controls.Add(btnSpeak);
            grpCallActions.Controls.Add(btnHold);
            grpCallActions.Controls.Add(btnHangUp);
            this.Controls.Add(grpCallActions);

            grpCallInitiation.Controls.Add(lblCaller);
            grpCallInitiation.Controls.Add(txtCaller);
            grpCallInitiation.Controls.Add(lblReceiver);
            grpCallInitiation.Controls.Add(txtReceiver);
            grpCallInitiation.Controls.Add(btnMakeCall);
            grpCallInitiation.Controls.Add(btnDropCall);
            this.Controls.Add(grpCallInitiation);

            grpCurrentCallStatus.Controls.Add(lblCurrentCallStatus);
            grpCurrentCallStatus.Controls.Add(lblCurrentCallDuration);
            this.Controls.Add(grpCurrentCallStatus);

            grpCallLogManagement.Controls.Add(lstCallLog);
            grpCallLogManagement.Controls.Add(txtSearchCallLog);
            grpCallLogManagement.Controls.Add(btnSearchCallLog);
            grpCallLogManagement.Controls.Add(btnPreviousCall);
            grpCallLogManagement.Controls.Add(btnNextCall);
            grpCallLogManagement.Controls.Add(btnReturnCall);
            this.Controls.Add(grpCallLogManagement);

            cmbSelectRole.Items.AddRange(Enum.GetNames(typeof(UserRole)));
            cmbSelectRole.SelectedItem = UserRole.None.ToString(); 
            cmbSelectRole.SelectedIndexChanged += cmbUserRole_SelectedIndexChanged;
            btnLogin.Click += btnLogin_Click;
            btnLogout.Click += btnLogout_Click; 
            btnSearchCallLog.Click += btnSearchCallLog_Click;
            btnNextCall.Click += btnNextCall_Click;
            btnPreviousCall.Click += btnPreviousCall_Click;
            btnReturnCall.Click += btnReturnCall_Click; 
            timerCallDuration.Interval = 1000; 
            timerCallDuration.Tick += timerCallDuration_Tick;
        }

        private void btnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            UserRole authenticatedRole = UserRole.None;

            if (username == "student" && password == "pass")
            {
                authenticatedRole = UserRole.Student;
            }
            else if (username == "tech" && password == "pass")
            {
                authenticatedRole = UserRole.Technician;
            }
            else if (username == "manager" && password == "pass")
            {
                authenticatedRole = UserRole.Manager;
            }

            if (authenticatedRole != UserRole.None)
            {
                (_callOperationProxy as CallProxy)?.SetUserRole(authenticatedRole);
                lblLoginStatus.Text = $"Login Successful as {authenticatedRole}!";
                lblLoginStatus.ForeColor = Color.Green;
                cmbSelectRole.SelectedItem = authenticatedRole.ToString();
                cmbSelectRole.Enabled = false;
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
                btnLogin.Enabled = false;
                btnLogout.Enabled = true; 
            }
            else
            {
                (_callOperationProxy as CallProxy)?.SetUserRole(UserRole.None);
                lblLoginStatus.Text = "Invalid Credentials.";
                lblLoginStatus.ForeColor = Color.Red;
                cmbSelectRole.SelectedItem = UserRole.None.ToString();
                cmbSelectRole.Enabled = false;
                btnLogout.Enabled = false; 
            }
            UpdateControlStates();
        }

        private void btnLogout_Click(object? sender, EventArgs e)
        {
            (_callOperationProxy as CallProxy)?.SetUserRole(UserRole.None);
            lblLoginStatus.Text = "Logged out.";
            lblLoginStatus.ForeColor = Color.Red;
            cmbSelectRole.SelectedItem = UserRole.None.ToString();
            cmbSelectRole.Enabled = false;
            txtUsername.Enabled = true;
            txtPassword.Enabled = true;
            btnLogin.Enabled = true;
            btnLogout.Enabled = false;

            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtCaller.Text = string.Empty;
            txtReceiver.Text = string.Empty;
            txtSearchCallLog.Text = "Search term..."; 
            lblCurrentCallStatus.Text = "Status: None";
            lblCurrentCallDuration.Text = "Duration: 00:00:00";
            _currentCall = null; 

            UpdateCallLogDisplay(); 
            UpdateControlStates();
        }

        private void timerCallDuration_Tick(object? sender, EventArgs e)
        {
            UpdateCurrentCallDisplay();
        }

        private void btnMakeCall_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCaller.Text) || string.IsNullOrWhiteSpace(txtReceiver.Text))
            {
                return;
            }

            _currentCall = (_callOperationProxy as CallProxy)?.MakeCall(txtCaller.Text, txtReceiver.Text);
            
            UpdateCallLogDisplay();
            UpdateCurrentCallDisplay();
            UpdateControlStates();
            timerCallDuration.Start();
        }

        private void btnDropCall_Click(object? sender, EventArgs e)
        {
            if (_currentCall != null)
            {
                _callOperationProxy.DropCall(_currentCall);
                UpdateCurrentCallDisplay();
                UpdateControlStates();
                timerCallDuration.Stop();
            }
        }

        private void btnSpeak_Click(object? sender, EventArgs e)
        {
            if (_currentCall != null)
            {
                _currentCall.Speak();
                UpdateCurrentCallDisplay();
                UpdateControlStates();
                timerCallDuration.Start();
            }
        }

        private void btnHold_Click(object? sender, EventArgs e)
        {
            if (_currentCall != null)
            {
                _currentCall.Hold();
                UpdateCurrentCallDisplay();
                UpdateControlStates();
                timerCallDuration.Stop();
            }
        }

        private void btnHangUp_Click(object? sender, EventArgs e)
        {
            if (_currentCall != null)
            {
                _currentCall.HangUp();
                UpdateCurrentCallDisplay();
                UpdateCallLogDisplay(); 
                UpdateControlStates();
                timerCallDuration.Stop();
            }
        }

        private void UpdateControlStates()
        {
            bool callActive = (_currentCall != null && _currentCall.GetStatus() != "Hung Up");
            bool onCall = (_currentCall != null && _currentCall.GetStatus() == "On Call");
            bool onHold = (_currentCall != null && _currentCall.GetStatus() == "On Hold");

            UserRole currentUserRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            
            bool canMakeCall = (currentUserRole == UserRole.Student || currentUserRole == UserRole.Technician || currentUserRole == UserRole.Manager);
            bool canDropCall = (currentUserRole == UserRole.Student || currentUserRole == UserRole.Technician || currentUserRole == UserRole.Manager);
            bool canSearchLog = (currentUserRole == UserRole.Student || currentUserRole == UserRole.Technician || currentUserRole == UserRole.Manager);
            bool canNavigateLog = (currentUserRole == UserRole.Manager);

            btnMakeCall.Enabled = canMakeCall && !callActive;
            txtCaller.Enabled = canMakeCall && !callActive;
            txtReceiver.Enabled = canMakeCall && !callActive;

            btnDropCall.Enabled = canDropCall && callActive;
            btnSpeak.Enabled = onCall || onHold;
            btnHold.Enabled = onCall;
            btnHangUp.Enabled = canDropCall && callActive && (onCall || onHold);

            btnSearchCallLog.Enabled = canSearchLog;
            btnSearchCallLog.Enabled = canSearchLog;
            btnNextCall.Enabled = canNavigateLog && _callLogIterator.HasNext();
            btnPreviousCall.Enabled = canNavigateLog && _callLogIterator.HasPrevious();
            btnReturnCall.Enabled = canNavigateLog && lstCallLog.SelectedIndex != -1 && currentUserRole == UserRole.Manager;

            bool isLoggedIn = (currentUserRole != UserRole.None);
            txtUsername.Enabled = !isLoggedIn;
            txtPassword.Enabled = !isLoggedIn;
            btnLogin.Enabled = !isLoggedIn;
            btnLogout.Enabled = isLoggedIn; 
            lblSelectRole.Enabled = isLoggedIn && currentUserRole == UserRole.Manager; 
            cmbSelectRole.Enabled = isLoggedIn && currentUserRole == UserRole.Manager;

            if (!isLoggedIn)
            {
                btnMakeCall.Enabled = false;
                txtCaller.Enabled = false;
                txtReceiver.Enabled = false;
                btnDropCall.Enabled = false;
                btnSpeak.Enabled = false;
                btnHold.Enabled = false;
                btnHangUp.Enabled = false;
                txtSearchCallLog.Enabled = false;
                btnSearchCallLog.Enabled = false;
                btnNextCall.Enabled = false;
                btnPreviousCall.Enabled = false; 
                btnReturnCall.Enabled = false;
            }
        }

        private void btnSearchCallLog_Click(object? sender, EventArgs e)
        {
            string searchTerm = txtSearchCallLog.Text;
            List<ICallLogEntry> searchResults = _callLogIterator.Search(searchTerm);
            lstCallLog.Items.Clear();
            foreach (var call in searchResults)
            {
                lstCallLog.Items.Add(call.GetCallDetails());
            }
            if (searchResults.Count == 0)
            {
                lstCallLog.Items.Add("No matching calls found.");
            }
        }

        private void btnNextCall_Click(object? sender, EventArgs e)
        {
            if (_callLogIterator.HasNext())
            {
                _callLogIterator.Next();
                HighlightCurrentCallInLog();
            }
            UpdateControlStates(); 
        }

        private void btnPreviousCall_Click(object? sender, EventArgs e)
        {
            if (_callLogIterator.HasPrevious())
            {
                _callLogIterator.Previous();
                HighlightCurrentCallInLog();
            }
            UpdateControlStates(); 
        }

        private void btnReturnCall_Click(object? sender, EventArgs e)
        {
            if (_callLog.GetEntries().Count == 0)
            {
                return;
            }

            if (lstCallLog.SelectedIndex != -1)
            {
                int selectedIndex = lstCallLog.SelectedIndex;
                _callLogIterator.MoveTo(selectedIndex); 
                ICallLogEntry? selectedEntry = _callLogIterator.GetCurrent();

                if (selectedEntry is Call callToReturn)
                {
                    (_callOperationProxy as CallProxy)?.ReturnCall(callToReturn);
                    _currentCall = callToReturn; 
                    UpdateCallLogDisplay();
                    UpdateCurrentCallDisplay();
                    UpdateControlStates();
                }
            }
        }

        private void HighlightCurrentCallInLog()
        {
            int currentIndex = _callLogIterator.GetCurrentIndex();
            if (currentIndex >= 0 && currentIndex < lstCallLog.Items.Count)
            {
                lstCallLog.SelectedIndex = currentIndex;
            }
            else if (lstCallLog.Items.Count > 0)
            {
                lstCallLog.SelectedIndex = -1; 
            }
        }

        private void cmbUserRole_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cmbSelectRole.SelectedItem is string selectedRoleName)
            {
                if (Enum.TryParse(selectedRoleName, out UserRole selectedRole))
                {
                    (_callOperationProxy as CallProxy)?.SetUserRole(selectedRole);
                }
            }
            UpdateControlStates(); 
        }

        private void UpdateCallLogDisplay()
        {
            lstCallLog.Items.Clear();
            _callLogIterator.Reset(); 
            while (_callLogIterator.HasNext())
            {
                lstCallLog.Items.Add(_callLogIterator.Next().GetCallDetails());
            }
            HighlightCurrentCallInLog(); 
        }

        private void UpdateCurrentCallDisplay()
        {
            if (_currentCall != null)
            {
                lblCurrentCallStatus.Text = $"Status: {_currentCall.GetStatus()}";
                lblCurrentCallDuration.Text = $"Duration: {_currentCall.GetDuration():hh':'mm':'ss}";
            }
            else
            {
                lblCurrentCallStatus.Text = "Status: None";
                lblCurrentCallDuration.Text = "Duration: 00:00:00";
            }
        }
    }
}
