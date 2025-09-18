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
        private string _currentUsername = string.Empty;

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

            // 🎨 Modern Blue-Gray Theme
            this.BackColor = Color.FromArgb(240, 248, 255); // Light blue-gray background

            grpAuthentication = new GroupBox {
                Text = "🔐 Authentication",
                Location = new Point(10, 10),
                Size = new Size(300, 190),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41), // Dark gray text
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            }; 
            
            lblUsername = new Label { Text = "Username :", Location = new Point(20, 25), AutoSize = false, Size = new Size(80, 23) };
            txtUsername = new TextBox { Location = new Point(100, 22), Width = 195, Size = new Size(195, 23) };
            lblPassword = new Label { Text = "Password :", Location = new Point(15, 55), AutoSize = false, Size = new Size(70, 23) };
            txtPassword = new TextBox { Location = new Point(90, 52), Width = 195, Size = new Size(195, 23), UseSystemPasswordChar = true };
            btnLogin = new Button { Text = "🔑 Login", Location = new Point(15, 85), Size = new Size(130, 25) };
            btnLogout = new Button { Text = "🚪 Logout", Location = new Point(150, 85), Size = new Size(130, 25), Enabled = false }; 
            lblLoginStatus = new Label { Text = "Please log in.", Location = new Point(15, 120), AutoSize = false, Size = new Size(270, 20), ForeColor = Color.Blue }; 

            lblSelectRole = new Label { Text = "Select Role:", Location = new Point(15, 150), AutoSize = false, Size = new Size(70, 23) };
            cmbSelectRole = new ComboBox { Text = "Select Role", Location = new Point(90, 147), Size = new Size(195, 23), DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };

            grpCallActions = new GroupBox {
                Text = "📞 Current Call Actions",
                Location = new Point(320, 10),
                Size = new Size(350, 70),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSpeak = new Button { Text = "🔊 Speak", Location = new Point(15, 25), Size = new Size(100, 25) };
            btnSpeak.Click += btnSpeak_Click;
            btnHold = new Button { Text = "⏸️ Hold", Location = new Point(125, 25), Size = new Size(100, 25) };
            btnHold.Click += btnHold_Click;
            btnHangUp = new Button { Text = "📵 Hang Up", Location = new Point(235, 25), Size = new Size(100, 25) };
            btnHangUp.Click += btnHangUp_Click;

            grpCallInitiation = new GroupBox {
                Text = "📱 Call Initiation",
                Location = new Point(10, 200),
                Size = new Size(300, 120),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            lblCaller = new Label { Text = "Caller:", Location = new Point(15, 25), AutoSize = false, Size = new Size(60, 23) };
            txtCaller = new TextBox { Location = new Point(85, 22), Width = 200, Size = new Size(200, 23) };
            lblReceiver = new Label { Text = "Receiver:", Location = new Point(15, 55), AutoSize = false, Size = new Size(60, 23) };
            txtReceiver = new TextBox { Location = new Point(85, 52), Width = 200, Size = new Size(200, 23) };
            btnMakeCall = new Button { Text = "📞 Make Call", Location = new Point(15, 85), Size = new Size(130, 25) };
            btnMakeCall.Click += btnMakeCall_Click;
            btnDropCall = new Button { Text = "❌ Drop Call", Location = new Point(150, 85), Size = new Size(130, 25) };
            btnDropCall.Click += btnDropCall_Click;

            grpCurrentCallStatus = new GroupBox {
                Text = "📊 Current Call Info",
                Location = new Point(320, 200),
                Size = new Size(350, 120),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            lblCurrentCallStatus = new Label { Text = "Status: None", Location = new Point(15, 25), AutoSize = false, Size = new Size(320, 20) };
            lblCurrentCallDuration = new Label { Text = "Duration: 00:00:00", Location = new Point(15, 50), AutoSize = false, Size = new Size(320, 20) };

            grpCallLogManagement = new GroupBox {
                Text = "📋 Call Log",
                Location = new Point(10, 330),
                Size = new Size(690, 210),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(33, 37, 41),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            }; 
            lstCallLog = new ListBox { Location = new Point(10, 25), Size = new Size(670, 120) };
            txtSearchCallLog = new TextBox { Text = "Search term...", Location = new Point(10, 155), Width = 180, Size = new Size(180, 23) };
            btnSearchCallLog = new Button { Text = "🔍 Search", Location = new Point(195, 155), Size = new Size(80, 25) };
            btnNextCall = new Button { Text = "▶️ Next Call", Location = new Point(320, 155), Size = new Size(100, 25) };
            btnPreviousCall = new Button { Text = "◀️ Previous Call", Location = new Point(430, 155), Size = new Size(100, 25), Enabled = true };
            btnReturnCall = new Button { Text = "↩️ Return Call", Location = new Point(550, 155), Size = new Size(100, 25), Enabled = false };

            timerCallDuration = new System.Windows.Forms.Timer();


            InitializeDesignPatterns();
            InitializeUIComponents();
            _callLog.LoadCallLog();
            UpdateCallLogDisplay();
            UpdateCurrentCallDisplay();
            UpdateControlStates();

            this.FormClosing += Form1_FormClosing;
            this.Text = "🎯 Call Center Management System";
            this.Size = new Size(720, 580);
            this.MinimumSize = new Size(720, 580);

            // Apply modern button styling
            ApplyModernButtonStyling();
        }

        private void ApplyModernButtonStyling()
        {
            // Green buttons for positive actions
            StyleButton(btnLogin, Color.FromArgb(40, 167, 69), Color.FromArgb(34, 139, 34));
            StyleButton(btnMakeCall, Color.FromArgb(40, 167, 69), Color.FromArgb(34, 139, 34));

            // Red buttons for destructive actions
            StyleButton(btnLogout, Color.FromArgb(220, 53, 69), Color.FromArgb(200, 35, 51));
            StyleButton(btnHangUp, Color.FromArgb(220, 53, 69), Color.FromArgb(200, 35, 51));
            // Gray button for drop call
            StyleButton(btnDropCall, Color.FromArgb(128, 128, 128), Color.FromArgb(169, 169, 169));

            // Blue buttons for informational actions
            StyleButton(btnSpeak, Color.FromArgb(0, 123, 255), Color.FromArgb(0, 105, 217));
            StyleButton(btnSearchCallLog, Color.FromArgb(0, 123, 255), Color.FromArgb(0, 105, 217));

            // Yellow/Orange buttons for secondary actions
            StyleButton(btnHold, Color.FromArgb(255, 193, 7), Color.FromArgb(255, 175, 0));
            // Light blue buttons for navigation actions
            StyleButton(btnNextCall, Color.FromArgb(173, 216, 230), Color.FromArgb(135, 206, 250));
            StyleButton(btnPreviousCall, Color.FromArgb(173, 216, 230), Color.FromArgb(135, 206, 250));
            StyleButton(btnReturnCall, Color.FromArgb(255, 193, 7), Color.FromArgb(255, 175, 0));
        }

        private void StyleButton(Button button, Color backColor, Color hoverColor)
        {
            button.BackColor = backColor;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Add hover effects
            button.MouseEnter += (sender, e) => {
                if (sender is Button btn) btn.BackColor = hoverColor;
            };
            button.MouseLeave += (sender, e) => {
                if (sender is Button btn) btn.BackColor = backColor;
            };
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
            txtSearchCallLog.Click += txtSearchCallLog_Click;
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
                _currentUsername = username; // Store username for session management
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
                _currentUsername = string.Empty; // Clear username on failed login
                (_callOperationProxy as CallProxy)?.SetUserRole(UserRole.None);
                lblLoginStatus.Text = "Invalid Credentials.";
                lblLoginStatus.ForeColor = Color.Red;
                cmbSelectRole.SelectedItem = UserRole.None.ToString();
                cmbSelectRole.Enabled = false;
                btnLogout.Enabled = false;
            }
            UpdateControlStates();
            UpdateCallLogDisplay(); // Update call log display after login to show filtered calls
        }

        private void btnLogout_Click(object? sender, EventArgs e)
        {
            (_callOperationProxy as CallProxy)?.SetUserRole(UserRole.None);
            _currentUsername = string.Empty; // Clear username for privacy

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

            // Hide call log section for privacy
            grpCallLogManagement.Visible = false;
            lstCallLog.Items.Clear(); // Clear call log display

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

            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            _currentCall = (_callOperationProxy as CallProxy)?.MakeCall(txtCaller.Text, txtReceiver.Text, currentRole, _currentUsername);

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

            // Enable navigation buttons based on visible calls, not raw iterator
            if (canNavigateLog && currentUserRole != UserRole.None)
            {
                var visibleCalls = GetVisibleCallsForUser(currentUserRole, _currentUsername);
                int currentVisibleIndex = FindCurrentVisibleCallIndex(visibleCalls);

                btnNextCall.Enabled = currentVisibleIndex >= 0 && currentVisibleIndex < visibleCalls.Count - 1;
                btnPreviousCall.Enabled = currentVisibleIndex > 0;
                btnReturnCall.Enabled = lstCallLog.SelectedIndex != -1 && currentUserRole == UserRole.Manager;
            }
            else
            {
                btnNextCall.Enabled = false;
                btnPreviousCall.Enabled = false;
                btnReturnCall.Enabled = false;
            }

            bool isLoggedIn = (currentUserRole != UserRole.None);
            txtUsername.Enabled = !isLoggedIn;
            txtPassword.Enabled = !isLoggedIn;
            btnLogin.Enabled = !isLoggedIn;
            btnLogout.Enabled = isLoggedIn;
            lblSelectRole.Enabled = isLoggedIn && currentUserRole == UserRole.Manager;
            cmbSelectRole.Enabled = isLoggedIn && currentUserRole == UserRole.Manager;

            // Control call log section visibility and functionality
            if (!isLoggedIn)
            {
                // Hide call log section completely when logged out
                grpCallLogManagement.Visible = false;
                lstCallLog.Enabled = false;
                txtSearchCallLog.Enabled = false;
                btnSearchCallLog.Enabled = false;
                btnNextCall.Enabled = false;
                btnPreviousCall.Enabled = false;
                btnReturnCall.Enabled = false;

                btnMakeCall.Enabled = false;
                txtCaller.Enabled = false;
                txtReceiver.Enabled = false;
                btnDropCall.Enabled = false;
                btnSpeak.Enabled = false;
                btnHold.Enabled = false;
                btnHangUp.Enabled = false;
            }
            else
            {
                // Show call log section when logged in
                grpCallLogManagement.Visible = true;
                lstCallLog.Enabled = true;
                txtSearchCallLog.Enabled = canSearchLog;
                btnSearchCallLog.Enabled = canSearchLog;
                btnNextCall.Enabled = canNavigateLog && _callLogIterator.HasNext();
                btnPreviousCall.Enabled = canNavigateLog && _callLogIterator.HasPrevious();
                btnReturnCall.Enabled = canNavigateLog && lstCallLog.SelectedIndex != -1 && currentUserRole == UserRole.Manager;
            }
        }

        private void btnSearchCallLog_Click(object? sender, EventArgs e)
        {
            string searchTerm = txtSearchCallLog.Text;

            // If search term is empty or just placeholder, show all filtered calls
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm == "Search term...")
            {
                UpdateCallLogDisplay();
                return;
            }

            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            string currentUsername = _currentUsername;

            // If logged out, don't show any calls
            if (currentRole == UserRole.None)
            {
                lstCallLog.Items.Clear();
                lstCallLog.Items.Add("Please log in to search calls.");
                return;
            }

            List<ICallLogEntry> searchResults = _callLogIterator.Search(searchTerm);
            lstCallLog.Items.Clear();

            bool foundMatches = false;
            foreach (var entry in searchResults)
            {
                if (entry is Call call && CanUserViewCall(call, currentRole, currentUsername))
                {
                    lstCallLog.Items.Add(call.GetCallDetails());
                    foundMatches = true;
                }
            }

            if (!foundMatches)
            {
                lstCallLog.Items.Add("No matching calls found.");
            }
        }

        private void btnNextCall_Click(object? sender, EventArgs e)
        {
            NavigateCall(1);
            UpdateControlStates();
        }

        private void btnPreviousCall_Click(object? sender, EventArgs e)
        {
            NavigateCall(-1);
            UpdateControlStates();
        }

        private void NavigateCall(int direction)
        {
            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            string currentUsername = _currentUsername;

            // If logged out, don't navigate
            if (currentRole == UserRole.None)
            {
                return;
            }

            // Get current visible calls for this user
            var visibleCalls = GetVisibleCallsForUser(currentRole, currentUsername);

            if (visibleCalls.Count == 0)
            {
                return;
            }

            // Find current position in the visible calls
            int currentVisibleIndex = FindCurrentVisibleCallIndex(visibleCalls);

            // Navigate to next/previous visible call
            int newVisibleIndex = currentVisibleIndex + direction;

            if (newVisibleIndex >= 0 && newVisibleIndex < visibleCalls.Count)
            {
                // Find the corresponding index in the full call list
                int fullIndex = FindFullCallIndex(visibleCalls[newVisibleIndex]);
                if (fullIndex >= 0)
                {
                    _callLogIterator.MoveTo(fullIndex);
                    HighlightCurrentCallInLog();
                }
            }
        }

        private List<Call> GetVisibleCallsForUser(UserRole currentRole, string currentUsername)
        {
            var visibleCalls = new List<Call>();
            var allCalls = _callLog.GetEntries();

            foreach (var entry in allCalls)
            {
                if (entry is Call call && CanUserViewCall(call, currentRole, currentUsername))
                {
                    visibleCalls.Add(call);
                }
            }

            return visibleCalls;
        }

        private int FindCurrentVisibleCallIndex(List<Call> visibleCalls)
        {
            if (lstCallLog.SelectedIndex >= 0 && lstCallLog.SelectedIndex < lstCallLog.Items.Count)
            {
                string selectedDetails = lstCallLog.Items[lstCallLog.SelectedIndex].ToString();
                for (int i = 0; i < visibleCalls.Count; i++)
                {
                    if (visibleCalls[i].GetCallDetails() == selectedDetails)
                    {
                        return i;
                    }
                }
            }
            return -1; // No selection or not found
        }

        private int FindFullCallIndex(Call targetCall)
        {
            var allCalls = _callLog.GetEntries();
            for (int i = 0; i < allCalls.Count; i++)
            {
                if (allCalls[i] is Call call && call == targetCall)
                {
                    return i;
                }
            }
            return -1;
        }

        private void btnReturnCall_Click(object? sender, EventArgs e)
        {
            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            string currentUsername = _currentUsername;

            // Only managers can return calls
            if (currentRole != UserRole.Manager)
            {
                return;
            }

            if (lstCallLog.SelectedIndex != -1 && lstCallLog.SelectedIndex < lstCallLog.Items.Count)
            {
                // Get the selected call details from the filtered display
                string selectedDetails = lstCallLog.Items[lstCallLog.SelectedIndex].ToString();

                // Find the corresponding call in the full list
                var allCalls = _callLog.GetEntries();
                Call? callToReturn = null;

                foreach (var entry in allCalls)
                {
                    if (entry is Call call && call.GetCallDetails() == selectedDetails)
                    {
                        callToReturn = call;
                        break;
                    }
                }

                if (callToReturn != null)
                {
                    // Return the call (create a new call from receiver to caller)
                    Call? returnedCall = (_callOperationProxy as CallProxy)?.ReturnCall(callToReturn);

                    // Start the timer for the returned call automatically
                    if (returnedCall != null)
                    {
                        _currentCall = returnedCall;
                        timerCallDuration.Start();
                    }

                    // Update the display to show the new returned call
                    UpdateCallLogDisplay();
                    UpdateCurrentCallDisplay();
                    UpdateControlStates();

                    // Try to select the new returned call in the list
                    SelectReturnedCall(callToReturn);
                }
            }
        }

        private void SelectReturnedCall(Call originalCall)
        {
            // Look for the returned call in the current display
            for (int i = 0; i < lstCallLog.Items.Count; i++)
            {
                string itemDetails = lstCallLog.Items[i].ToString();
                // The returned call will have caller and receiver swapped
                if (itemDetails.Contains($"Caller: {originalCall.Receiver}") &&
                    itemDetails.Contains($"Receiver: {originalCall.Caller}"))
                {
                    lstCallLog.SelectedIndex = i;
                    break;
                }
            }
        }

        private void HighlightCurrentCallInLog()
        {
            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            string currentUsername = _currentUsername;

            // If logged out, clear selection
            if (currentRole == UserRole.None)
            {
                lstCallLog.SelectedIndex = -1;
                return;
            }

            // Get the current call from the iterator
            ICallLogEntry? currentEntry = _callLogIterator.GetCurrent();
            if (currentEntry is Call currentCall)
            {
                // Check if current user can see this call
                if (CanUserViewCall(currentCall, currentRole, currentUsername))
                {
                    // Find this call in the current filtered display
                    for (int i = 0; i < lstCallLog.Items.Count; i++)
                    {
                        if (lstCallLog.Items[i].ToString() == currentCall.GetCallDetails())
                        {
                            lstCallLog.SelectedIndex = i;
                            return;
                        }
                    }
                }
            }

            // If we can't find the current call in the filtered display, clear selection
            lstCallLog.SelectedIndex = -1;
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

            UserRole currentRole = (_callOperationProxy as CallProxy)?.CurrentUserRole ?? UserRole.None;
            string currentUsername = _currentUsername;

            // If logged out, don't show any calls
            if (currentRole == UserRole.None)
            {
                HighlightCurrentCallInLog();
                return;
            }

            _callLogIterator.Reset();
            while (_callLogIterator.HasNext())
            {
                ICallLogEntry entry = _callLogIterator.Next();
                if (entry is Call call)
                {
                    // Apply user-based filtering
                    if (CanUserViewCall(call, currentRole, currentUsername))
                    {
                        lstCallLog.Items.Add(call.GetCallDetails());
                    }
                }
                else
                {
                    // For non-Call entries (like CallLog composites), show them
                    lstCallLog.Items.Add(entry.GetCallDetails());
                }
            }

            // Initialize navigation by selecting the first visible call if available
            if (lstCallLog.Items.Count > 0)
            {
                lstCallLog.SelectedIndex = 0;
                // Update iterator to point to the first visible call
                InitializeIteratorToFirstVisibleCall(currentRole, currentUsername);
            }
            else
            {
                lstCallLog.SelectedIndex = -1;
            }
        }

        private void InitializeIteratorToFirstVisibleCall(UserRole currentRole, string currentUsername)
        {
            // Find the first visible call and position the iterator there
            _callLogIterator.Reset();
            while (_callLogIterator.HasNext())
            {
                ICallLogEntry entry = _callLogIterator.Next();
                if (entry is Call call && CanUserViewCall(call, currentRole, currentUsername))
                {
                    // Found the first visible call, iterator is now positioned here
                    return;
                }
            }
        }

        private bool CanUserViewCall(Call call, UserRole currentRole, string currentUsername)
        {
            // Students can only see their own calls
            if (currentRole == UserRole.Student)
            {
                return call.InitiatedByUsername == currentUsername;
            }

            // Technicians can see their own calls + all student calls
            if (currentRole == UserRole.Technician)
            {
                return call.InitiatedBy == UserRole.Student ||
                       call.InitiatedByUsername == currentUsername;
            }

            // Managers can see ALL calls
            if (currentRole == UserRole.Manager)
            {
                return true;
            }

            // Default: no access
            return false;
        }

        private void txtSearchCallLog_Click(object? sender, EventArgs e)
        {
            if (txtSearchCallLog.Text == "Search term...")
            {
                txtSearchCallLog.Text = string.Empty;
                txtSearchCallLog.ForeColor = Color.Black;
            }
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
