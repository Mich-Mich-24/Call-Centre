using System;

namespace Assignment1_ONT412
{
    // Context: Call
    // The Call class maintains an instance of a concrete state
    // subclass that defines the current state of the call.
    public class Call : ICallLogEntry
    {
        private ICallState _currentState;
        public string Caller { get; private set; }
        public string Receiver { get; private set; }
        public DateTime StartTime { get; private set; }
        public UserRole InitiatedBy { get; private set; }
        public string InitiatedByUsername { get; private set; }

        public Call(string caller, string receiver)
        {
            Caller = caller;
            Receiver = receiver;
            StartTime = DateTime.Now;
            _currentState = new OnCallState(TimeSpan.Zero);
            InitiatedBy = UserRole.None;
            InitiatedByUsername = string.Empty;
        }

        public Call(string caller, string receiver, DateTime startTime, TimeSpan duration)
        {
            Caller = caller;
            Receiver = receiver;
            StartTime = startTime;
            _currentState = new OnCallState(duration);
            InitiatedBy = UserRole.None;
            InitiatedByUsername = string.Empty;
        }

        public Call(string caller, string receiver, UserRole initiatedBy, string initiatedByUsername)
        {
            Caller = caller;
            Receiver = receiver;
            StartTime = DateTime.Now;
            _currentState = new OnCallState(TimeSpan.Zero);
            InitiatedBy = initiatedBy;
            InitiatedByUsername = initiatedByUsername;
        }

        public Call(string caller, string receiver, DateTime startTime, TimeSpan duration, UserRole initiatedBy, string initiatedByUsername)
        {
            Caller = caller;
            Receiver = receiver;
            StartTime = startTime;
            _currentState = new OnCallState(duration);
            InitiatedBy = initiatedBy;
            InitiatedByUsername = initiatedByUsername;
        }

        public void SetState(ICallState state)
        {
            _currentState = state;
            Console.WriteLine($"Call state changed to: {_currentState.GetStatus()}");
        }

        public void Speak()
        {
            _currentState.HandleSpeak(this);
        }

        public void Hold()
        {
            _currentState.HandleHold(this);
        }

        public void HangUp()
        {
            _currentState.HandleHangUp(this);
        }

        public string GetStatus()
        {
            return _currentState.GetStatus();
        }

        public TimeSpan GetDuration()
        {
            return _currentState.GetDuration();
        }

        // Composite Design Pattern: Call class as a Leaf
        // The Call class implements ICallLogEntry, acting as a leaf in the composite structure.
        public string GetCallDetails()
        {
            string userInfo = string.IsNullOrEmpty(InitiatedByUsername) ? "" : $", Initiated by: {InitiatedByUsername} ({InitiatedBy})";
            return $"Caller: {Caller}, Receiver: {Receiver}, Status: {GetStatus()}, Duration: {GetDuration():hh':'mm':'ss}{userInfo}";
        }

        public TimeSpan GetTotalDuration()
        {
            return GetDuration();
        }
    }
}
