using System.Threading;

namespace ExtraInfo;

public interface IThreadHighlight
{
    bool Enabled { get; set; }
    Thread OpThread { get; set; }
    string Name { get; }
    string ThreadName { get; }
    int Radius { get; }
}