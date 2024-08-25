namespace WpfUtils.Commands;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

public class SimpleCommand : ICommand
{
	protected Action? action;
	protected Func<Task>? asyncFunc;
	protected Func<bool>? canExecute;
	private bool isExecuting = false;
	private EventHandler? canExecuteChanged;

	public SimpleCommand()
	{
		CommandManager.RequerySuggested += this.OnRequerySuggested;
	}

	public SimpleCommand(Action action, Func<bool> canExecute)
		: this()
	{
		this.action = action;
		this.canExecute = canExecute;
	}

	public SimpleCommand(Action action)
		: this()
	{
		this.action = action;
	}

	public SimpleCommand(Func<Task> func)
		: this()
	{
		this.asyncFunc = func;
	}

	public event EventHandler? CanExecuteChanged
	{
		add
		{
			// yuck
			Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
			this.canExecuteChanged += (s, e) =>
			{
				currentDispatcher?.BeginInvoke(() =>
				{
					value?.Invoke(s, e);
				});
			};
		}

		remove
		{
			this.canExecuteChanged -= value;
		}
	}

	public bool CanExecute(object? parameter)
	{
		if (this.isExecuting)
			return false;

		return this.CanExecute();
	}

	public void Execute(object? parameter)
	{
		this.isExecuting = true;
		this.RaiseCanExecuteChanged();

		this.action?.Invoke();

		Task.Run(async () =>
		{
			try
			{
				if (this.asyncFunc != null)
					await this.asyncFunc.Invoke();

				await this.Execute();
			}
			finally
			{
				this.isExecuting = false;
				this.RaiseCanExecuteChanged();
			}
		});
	}

	protected void RaiseCanExecuteChanged()
	{
		this.canExecuteChanged?.Invoke(this, new());
	}

	protected virtual bool CanExecute()
	{
		return this.canExecute?.Invoke() ?? true;
	}

	protected virtual Task Execute()
	{
		return Task.CompletedTask;
	}

	private void OnRequerySuggested(object? sender, EventArgs e)
	{
		this.canExecuteChanged?.Invoke(sender, e);
	}
}