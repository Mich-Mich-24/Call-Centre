using System;

namespace Assignment1_ONT412
{
    // RealSubject: RealCallOperation
    // The RealCallOperation class contains the core business logic for making and dropping calls.
    public class RealCallOperation : ICallOperation
    {
        private CallLog _callLog;

        public RealCallOperation(CallLog callLog)
        {
            _callLog = callLog;
        }

        public Call? MakeCall(string caller, string receiver)
        {
            Call newCall = new Call(caller, receiver);
            _callLog.AddEntry(newCall);
            Console.WriteLine($"Call initiated by {caller} to {receiver}.");
            return newCall;
        }

        public Call? MakeCall(string caller, string receiver, UserRole initiatedBy, string initiatedByUsername)
        {
            Call newCall = new Call(caller, receiver, initiatedBy, initiatedByUsername);
            _callLog.AddEntry(newCall);
            Console.WriteLine($"Call initiated by {caller} to {receiver} by {initiatedByUsername} ({initiatedBy}).");
            return newCall;
        }

        public void DropCall(Call call)
        {
            if (call != null && (call.GetStatus() == "On Call" || call.GetStatus() == "On Hold"))
            {
                call.HangUp();
                Console.WriteLine($"Call from {call.Caller} to {call.Receiver} dropped.");
            }
            else if (call != null)
            {
                Console.WriteLine($"Call from {call.Caller} to {call.Receiver} is already hung up.");
            }
            else
            {
                Console.WriteLine("No active call to drop.");
            }
        }

        public Call? ReturnCall(Call call)
        {
            Console.WriteLine($"RealOperation: Manager returning call from {call.Caller} to {call.Receiver}.");

            Call returnedCall = new Call(call.Receiver, call.Caller);
            _callLog.AddEntry(returnedCall);
            return returnedCall;
        }
    }
}
