using System;

namespace Assignment1_ONT412
{
    // Concrete State: HungUpState
    // Represents the state when a call has been hung up.
    public class HungUpState : ICallState
    {
        private TimeSpan _finalDuration;

        public HungUpState(TimeSpan duration)
        {
            _finalDuration = duration;
        }

        public void HandleSpeak(Call call)
        {
        }

        public void HandleHold(Call call)
        {
        }

        public void HandleHangUp(Call call)
        {
        }

        public string GetStatus()
        {
            return "Hung Up";
        }

        public TimeSpan GetDuration()
        {
            return _finalDuration;
        }
    }
}
