using System.Windows.Input;


namespace MiniMvvm;

public sealed class MiniCommand<T> : MiniCommand, ICommand
{
    private bool _busy;
    private readonly Action<T?>? _commandBody;
    private readonly Func<T?, Task>? _asyncCommandBody;
        
    
    public MiniCommand(Action<T?> cb)
    {
        _commandBody = cb ?? throw new ArgumentNullException(nameof(cb));
    }
    

    public MiniCommand(Func<T?, Task> cb)
    {
        _asyncCommandBody = cb ?? throw new ArgumentNullException(nameof(cb));
    }

    
    private bool Busy
    {
        get => _busy;
        set
        {
            _busy = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

        
    public override event EventHandler? CanExecuteChanged;
    public override bool CanExecute(object? parameter) => !_busy;

    
    public override async void Execute(object? parameter)
    {
        if (Busy)
        {
            return;
        }

        try
        {
            Busy = true;
            
            if (_commandBody != null)
            {
                _commandBody((T?)parameter);
            }
            else if (_asyncCommandBody != null)
            {
                await _asyncCommandBody((T?)parameter);
            }
        }
        finally
        {
            Busy = false;
        }
    }
}
    

public abstract class MiniCommand : ICommand
{
    public static MiniCommand Create(Action cb) => new MiniCommand<object>(_ => cb());
    public static MiniCommand Create<TArg>(Action<TArg?> cb) => new MiniCommand<TArg>(cb);
    public static MiniCommand CreateFromTask(Func<Task> cb) => new MiniCommand<object>(_ => cb());
        
    public abstract bool CanExecute(object? parameter);
    public abstract void Execute(object? parameter);
    public abstract event EventHandler? CanExecuteChanged;
}
