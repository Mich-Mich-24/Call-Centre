using System;

namespace Assignment1_ONT412
{
    // Concrete State: OnCallState
    // Represents the state when a call is active.
    public class OnCallState : ICallState
    {
        private DateTime _startTime;
        private TimeSpan _accumulatedDuration;

        public OnCallState(TimeSpan initialDuration = default)
        {
            _startTime = DateTime.Now;
            _accumulatedDuration = initialDuration;
        }

        public void HandleSpeak(Call call)
        {
            // Already speaking, no state change needed. Duration is updated via GetDuration().
        }

        public void HandleHold(Call call)
        {
            _accumulatedDuration += (DateTime.Now - _startTime); // Accumulate duration before holding
            call.SetState(new OnHoldState(_accumulatedDuration));
        }

        public void HandleHangUp(Call call)
        {
            _accumulatedDuration += (DateTime.Now - _startTime); // Accumulate duration before hanging up
            call.SetState(new HungUpState(_accumulatedDuration));
        }

        public string GetStatus()
        {
            return "On Call";
        }

        public TimeSpan GetDuration()
        {
            return _accumulatedDuration + (DateTime.Now - _startTime);
        }
    }
}
