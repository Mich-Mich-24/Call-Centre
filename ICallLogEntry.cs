using System;

namespace Assignment1_ONT412
{
    // Composite Design Pattern: ICallLogEntry Interface
    // This interface defines the common operations for both individual calls (Leaf)
    // and call logs (Composite).
    public interface ICallLogEntry
    {
        string GetCallDetails();
        TimeSpan GetTotalDuration();
    }
}
