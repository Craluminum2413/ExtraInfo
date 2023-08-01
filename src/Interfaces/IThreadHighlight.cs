using System.Threading;
using Vintagestory.API.Client;

namespace ExtraInfo;

public interface IHighlightThread
{
    bool Enabled { get; set; }
    Thread OpThread { get; set; }
    string Name { get; }
    string ThreadName { get; }
    int Radius { get; }
}