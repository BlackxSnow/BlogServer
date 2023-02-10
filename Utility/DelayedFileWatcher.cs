using System.Timers;
using Timer = System.Timers.Timer;

namespace BlogServer.Utility;

public class DelayedFileWatcher
{
    public int Delay = 250; 

    private FileSystemWatcher _Watcher = new FileSystemWatcher();

    public event Action<object, FileSystemEventArgs> Created;
    public event Action<object, FileSystemEventArgs> Deleted;
    public event Action<object, RenamedEventArgs> Renamed;
    public event Action<object, FileSystemEventArgs> Changed;

    private (object, FileSystemEventArgs)? _PendingCreated;
    private (object, FileSystemEventArgs)? _PendingDeleted;
    private (object, RenamedEventArgs)? _PendingRenamed;
    private (object, FileSystemEventArgs)? _PendingChanged;

    private readonly Timer _CreatedTimer = new Timer();
    private readonly Timer _DeletedTimer = new Timer();
    private readonly Timer _RenamedTimer = new Timer();
    private readonly Timer _ChangedTimer = new Timer();
    
    public DelayedFileWatcher(string path, string filter, NotifyFilters notifyFilters)
    {
        _Watcher.Path = path;
        _Watcher.NotifyFilter = notifyFilters;
        _Watcher.Filter = filter;
        _Watcher.IncludeSubdirectories = true;
        _Watcher.Changed += WatcherOnChanged;
        _Watcher.Renamed += WatcherOnRenamed;
        _Watcher.Deleted += WatcherOnDeleted;
        _Watcher.Created += WatcherOnCreated;

        _DeletedTimer.Elapsed += InvokeDeleted;
        _DeletedTimer.AutoReset = false;
        
        _RenamedTimer.Elapsed += InvokeRenamed;
        _RenamedTimer.AutoReset = false;
        
        _ChangedTimer.Elapsed += InvokeChanged;
        _ChangedTimer.AutoReset = false;

        _CreatedTimer.Elapsed += InvokeCreated;
        _CreatedTimer.AutoReset = false;
    }



    public void Start()
    {
        _Watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        _Watcher.EnableRaisingEvents = false;
        _DeletedTimer.Stop();
        _RenamedTimer.Stop();
        _ChangedTimer.Stop();
    }
    
    private void WatcherOnCreated(object sender, FileSystemEventArgs e)
    {
        // Console.WriteLine($"onCreate: {e.FullPath}");
        _PendingCreated = (sender, e);
        _CreatedTimer.Interval = Delay;
        _CreatedTimer.Start();
    }

    private void InvokeCreated(object? sender, ElapsedEventArgs e)
    {
        // Console.WriteLine($"CreateInvoke: {_PendingCreated!.Value.Item2.FullPath}");
        Created?.Invoke(_PendingCreated!.Value.Item1, _PendingCreated.Value.Item2);
        _PendingCreated = null;
    }
    
    private void WatcherOnDeleted(object sender, FileSystemEventArgs e)
    {
        // Console.WriteLine($"onDelete: {e.FullPath}");
        _PendingDeleted = (sender, e);
        _DeletedTimer.Interval = Delay;
        _DeletedTimer.Start();
    }

    private void InvokeDeleted(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        // Console.WriteLine($"DeleteInvoke: {_PendingCreated!.Value.Item2.FullPath}");
        Deleted?.Invoke(_PendingDeleted!.Value.Item1, _PendingDeleted.Value.Item2);
        _PendingDeleted = null;
    }
    
    private void WatcherOnRenamed(object sender, RenamedEventArgs e)
    {
        // Console.WriteLine($"onRename: {e.FullPath}");
        _PendingRenamed = (sender, e);
        _RenamedTimer.Interval = Delay;
        _RenamedTimer.Start();
    }
    private void InvokeRenamed(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        // Console.WriteLine($"RenameInvoke: {_PendingCreated!.Value.Item2.FullPath}");
        Renamed?.Invoke(_PendingRenamed!.Value.Item1, _PendingRenamed.Value.Item2);
        _PendingRenamed = null;
    }
    
    private void WatcherOnChanged(object sender, FileSystemEventArgs e)
    {
        // Console.WriteLine($"onChange: {e.FullPath}");
        _PendingChanged = (sender, e);
        _ChangedTimer.Interval = Delay;
        _ChangedTimer.Start();
    }
    private void InvokeChanged(object? sender, ElapsedEventArgs elapsedEventArgs)
    {
        // Console.WriteLine($"ChangeInvoke: {_PendingCreated!.Value.Item2.FullPath}");
        Changed?.Invoke(_PendingChanged!.Value.Item1, _PendingChanged.Value.Item2);
        _PendingChanged = null;
    }
}