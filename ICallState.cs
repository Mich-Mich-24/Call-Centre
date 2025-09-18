using System;

namespace Assignment1_ONT412
{
    // State Design Pattern: ICallState Interface
    // Defines the contract for different states of a call.
    // Concrete states will implement this interface to provide state-specific behavior.
    public interface ICallState
    {
        void HandleSpeak(Call call);
        void HandleHold(Call call);
        void HandleHangUp(Call call);
        string GetStatus();
        TimeSpan GetDuration();
    }
}
