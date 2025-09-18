using System;

namespace Assignment1_ONT412
{
    // Proxy: CallProxy
    // The CallProxy class controls access to a RealCallOperation object,
    // adding responsibilities like logging or security checks.
    public class CallProxy : ICallOperation
    {
        private RealCallOperation _realCallOperation;
        private UserRole _currentUserRole = UserRole.None;

        public UserRole CurrentUserRole => _currentUserRole;

        public CallProxy(CallLog callLog, UserRole initialRole)
        {
            _realCallOperation = new RealCallOperation(callLog);
            _currentUserRole = initialRole;
        }

        public void SetUserRole(UserRole role)
        {
            _currentUserRole = role;
            Console.WriteLine($"User role set to: {_currentUserRole}");
        }

        public Call? MakeCall(string caller, string receiver)
        {
            if (_currentUserRole == UserRole.Student || _currentUserRole == UserRole.Technician || _currentUserRole == UserRole.Manager)
            {
                Console.WriteLine($"Proxy: Logging call attempt from {caller} to {receiver} by {_currentUserRole}.");
                return _realCallOperation.MakeCall(caller, receiver, _currentUserRole, _currentUserRole.ToString().ToLower());
            }
            else
            {
                Console.WriteLine("Proxy: Unauthorized. Cannot make call.");
                return null;
            }
        }

        public Call? MakeCall(string caller, string receiver, UserRole initiatedBy, string initiatedByUsername)
        {
            if (_currentUserRole == UserRole.Student || _currentUserRole == UserRole.Technician || _currentUserRole == UserRole.Manager)
            {
                Console.WriteLine($"Proxy: Logging call attempt from {caller} to {receiver} by {initiatedByUsername} ({initiatedBy}).");
                return _realCallOperation.MakeCall(caller, receiver, initiatedBy, initiatedByUsername);
            }
            else
            {
                Console.WriteLine("Proxy: Unauthorized. Cannot make call.");
                return null;
            }
        }

        public void DropCall(Call call)
        {
            if (_currentUserRole == UserRole.Student || _currentUserRole == UserRole.Technician || _currentUserRole == UserRole.Manager)
            {
                Console.WriteLine($"Proxy: Logging call drop for call from {call.Caller} to {call.Receiver} by {_currentUserRole}.");
                _realCallOperation.DropCall(call);
            }
            else
            {
                Console.WriteLine("Proxy: Unauthorized. Cannot drop call.");
            }
        }

        public Call? ReturnCall(Call call)
        {
            if (_currentUserRole == UserRole.Manager)
            {
                Console.WriteLine($"Proxy: Logging return call attempt for {call.Caller} to {call.Receiver} by {_currentUserRole}.");
                return _realCallOperation.ReturnCall(call);
            }
            else
            {
                Console.WriteLine("Proxy: Unauthorized. Only managers can return calls.");
                return null;
            }
        }
    }
}
