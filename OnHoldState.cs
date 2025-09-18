using System;

namespace Assignment1_ONT412
{
    // Concrete State: OnHoldState
    // Represents the state when a call is on hold.
    public class OnHoldState : ICallState
    {
        private DateTime _holdStartTime;
        private TimeSpan _accumulatedDurationBeforeHold;

        public OnHoldState(TimeSpan accumulatedDurationBeforeHold = default)
        {
            _holdStartTime = DateTime.Now;
            _accumulatedDurationBeforeHold = accumulatedDurationBeforeHold;
        }

        public void HandleSpeak(Call call)
        {
            TimeSpan durationOnHold = DateTime.Now - _holdStartTime;
            call.SetState(new OnCallState(_accumulatedDurationBeforeHold + durationOnHold));
        }

        public void HandleHold(Call call)
        {
        }

        public void HandleHangUp(Call call)
        {
            TimeSpan durationOnHold = DateTime.Now - _holdStartTime;
            call.SetState(new HungUpState(_accumulatedDurationBeforeHold + durationOnHold));
        }

        public string GetStatus()
        {
            return "On Hold";
        }

        public TimeSpan GetDuration()
        {
            return _accumulatedDurationBeforeHold + (DateTime.Now - _holdStartTime);
        }
    }
}
