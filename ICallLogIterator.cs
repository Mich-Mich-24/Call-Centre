using System.Collections.Generic;

namespace Assignment1_ONT412
{
    // Iterator Design Pattern: ICallLogIterator Interface
    // Defines the interface for accessing and traversing elements of the call log.
    public interface ICallLogIterator
    {
        bool HasNext();
        ICallLogEntry Next();
        void Reset();
        ICallLogEntry GetCurrent();
        int GetCurrentIndex();
        void MoveTo(int index);
        bool HasPrevious();
        ICallLogEntry Previous();
        List<ICallLogEntry> Search(string searchTerm);
    }

    // Iterator Design Pattern: ICallLogAggregate Interface
    // Declares the interface for objects that can create an iterator.
    public interface ICallLogAggregate
    {
        ICallLogIterator CreateIterator();
    }
}
