using System.Collections.Generic;
using System.Linq;

namespace Assignment1_ONT412
{
    // Concrete Iterator: CallLogIterator
    // Implements the ICallLogIterator interface and provides the traversal logic for the CallLog.
    public class CallLogIterator : ICallLogIterator
    {
        private List<ICallLogEntry> _calls;
        private int _position = 0;

        public CallLogIterator(List<ICallLogEntry> calls)
        {
            _calls = calls;
        }

        public bool HasNext()
        {
            return _position < _calls.Count;
        }

        public ICallLogEntry Next()
        {
            if (HasNext())
            {
                return _calls[_position++];
            }
            return null;
        }

        public ICallLogEntry GetCurrent()
        {
            if (_position >= 0 && _position < _calls.Count)
            {
                return _calls[_position];
            }
            return null;
        }

        public int GetCurrentIndex()
        {
            return _position;
        }

        public void MoveTo(int index)
        {
            if (index >= 0 && index < _calls.Count)
            {
                _position = index;
            } else if (index < 0) {
                _position = 0; // Clamp to start
            } else if (index >= _calls.Count) {
                _position = _calls.Count - 1; // Clamp to end
            }
        }

        public bool HasPrevious()
        {
            return _position > 0;
        }

        public ICallLogEntry Previous()
        {
            if (HasPrevious())
            {
                return _calls[--_position];
            }
            return null;
        }

        public void Reset()
        {
            _position = 0;
        }

        public List<ICallLogEntry> Search(string searchTerm)
        {
            return _calls.Where(c => c.GetCallDetails().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}
