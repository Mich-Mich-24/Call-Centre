using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Assignment1_ONT412
{
    // Composite: CallLog
    // The CallLog class acts as a composite, holding a collection of ICallLogEntry objects.
    // It also implements ICallLogEntry itself, allowing clients to treat individual calls
    // and call logs uniformly.
    public class CallLog : ICallLogEntry, ICallLogAggregate
    {
        private List<ICallLogEntry> _callEntries = new List<ICallLogEntry>();
        private const string _logFilePath = "calllog.csv";

        public string Name { get; private set; }

        public CallLog(string name)
        {
            Name = name;
        }

        public void AddEntry(ICallLogEntry entry)
        {
            _callEntries.Add(entry);
        }

        public void RemoveEntry(ICallLogEntry entry)
        {
            _callEntries.Remove(entry);
        }

        public string GetCallDetails()
        {
            string details = $"Call Log: {Name}\n";
            foreach (var entry in _callEntries)
            {
                details += $"  - {entry.GetCallDetails()}\n";
            }
            return details;
        }

        public TimeSpan GetTotalDuration()
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (var entry in _callEntries)
            {
                totalDuration += entry.GetTotalDuration();
            }
            return totalDuration;
        }

        public List<ICallLogEntry> GetEntries()
        {
            return _callEntries;
        }

        public ICallLogIterator CreateIterator()
        {
            return new CallLogIterator(_callEntries);
        }

        public void SaveCallLog()
        {
            List<string> lines = new List<string>();
            foreach (var entry in _callEntries)
            {
                if (entry is Call call)
                {
                    lines.Add($"{call.Caller},{call.Receiver},{call.StartTime.ToString("o")},{call.GetDuration().TotalSeconds}");
                }
            }
            File.WriteAllLines(_logFilePath, lines);
        }

        public void LoadCallLog()
        {
            _callEntries.Clear(); // Clear existing entries before loading
            if (File.Exists(_logFilePath))
            {
                string[] lines = File.ReadAllLines(_logFilePath);
                foreach (string line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        string caller = parts[0];
                        string receiver = parts[1];
                        DateTime startTime = DateTime.Parse(parts[2]);
                        TimeSpan duration = TimeSpan.FromSeconds(double.Parse(parts[3]));

                        Call loadedCall = new Call(caller, receiver, startTime, duration);
                        _callEntries.Add(loadedCall);
                    }
                }
            }
        }
    }
}
