using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using KMBox.NET.Structures;

namespace KMBox.NET;

/// <summary>
/// Report listener. <br/>
/// Allows listening to keyboard/mouse inputs.
/// </summary>
[PublicAPI]
public sealed class ReportListener : IDisposable
{
    /// <summary>
    /// Is this listener stopped?
    /// </summary>
    public bool Stopped { get; private set; }
    
    /// <summary>
    /// Your custom event handler goes here.
    /// </summary>
    public Action<CompositeReport>? EventListener { get; set; }
    
    private readonly KmBoxClient _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private Task? _runningTask;
    private ManualResetEventSlim? _resetEvent;

    internal ReportListener(KmBoxClient client)
    {
        _client = client;
    }

    /// <summary>
    /// Starts listening to incoming events and enables 'monitor' mode.
    /// </summary>
    /// <param name="resetEvent">Custom event which will be signalled when <see cref="ReportListener"/> will stop.</param>
    /// <exception cref="InvalidOperationException">This listener is already active!</exception>
    public void Start(ManualResetEventSlim? resetEvent = null)
    {
        if (_runningTask != null)
            throw new InvalidOperationException("Listener is already running!");
        
        _resetEvent = resetEvent;
        _runningTask = Task.Run(async () => await ListenerThread());
    }
    
    private async Task ListenerThread()
    {
        try
        {
            var localEndpoint = new IPEndPoint(IPAddress.Any, _client.Port + 1);
            using var udpClient = new UdpClient(localEndpoint);
            
            await _client.EnableMonitor(true);
            
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var receivedData = await udpClient.ReceiveAsync(_cancellationTokenSource.Token);
                
                var buffer = receivedData.Buffer;
                var report = StructHelper.ByteArrayToStruct<CompositeReport>(buffer);
                
                EventListener?.Invoke(report);
            }
        }
        finally
        {
            Stopped = true;
        }
    }

    /// <summary>
    /// Stop listening and disable 'monitor' mode.
    /// </summary>
    public void Stop()
    {
        if(Stopped)
            return;
        
        _client.EnableMonitor(false).Wait();
        _cancellationTokenSource.Cancel();
        Stopped = true; // just in case yk
    }
    
    /// <summary>
    /// Same as <see cref="Stop"/>.
    /// </summary>
    public void Dispose()
    {
        Stop();
    }
}