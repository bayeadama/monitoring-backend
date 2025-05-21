using Common.Models;

namespace Common.Listener;

public interface IListener
{
    event OnResponseReceivedDelegate OnResponseReceived;
}

public delegate void OnResponseReceivedDelegate(Response response);
