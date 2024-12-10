namespace WpfUtils.Utils;

using System;
using System.Threading.Tasks;
using WpfUtils.Logging;

public class FuncQueue
{
	private readonly Func<Task> func;
	private int currentDelayValue;
	private Task? task;
	private bool isCanceling;

	public FuncQueue(Func<Task> func, int delay)
	{
		this.Delay = delay;
		this.func = func;
	}

	public FuncQueue(Action func, int delay)
	{
		this.Delay = delay;
		this.func = () =>
		{
			func.Invoke();
			return Task.CompletedTask;
		};
	}

	public int Delay { get; set; }
	public bool Pending { get; private set; }
	public bool Executing { get; private set; }

	public async Task WaitForPendingExecute()
	{
		this.ClearDelay();

		while (this.Pending || this.Executing)
		{
			await Task.Delay(1);
		}
	}

	public void Invoke()
	{
		this.currentDelayValue = this.Delay;
		this.isCanceling = false;

		if (this.task == null || this.task.IsCompleted)
		{
			this.task = Task.Run(this.RunTask);
		}
	}

	public void ClearDelay()
	{
		this.currentDelayValue = 0;
	}

	public void Cancel()
	{
		this.ClearDelay();
		this.isCanceling = true;
	}

	public void InvokeImmediate()
	{
		this.Invoke();
		this.ClearDelay();
	}

	private async Task RunTask()
	{
		// Double loops to handle case where a refresh delay was added
		// while the refresh was running
		while (this.currentDelayValue >= 0)
		{
			lock (this)
				this.Pending = true;

			while (this.currentDelayValue > 0)
			{
				await Task.Delay(10);
				this.currentDelayValue -= 10;
			}

			if (this.isCanceling)
			{
				this.currentDelayValue -= 1;
				this.isCanceling = false;
				return;
			}

			lock (this)
			{
				this.Executing = true;
				this.Pending = false;
			}

			try
			{
				await this.func.Invoke();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error invoking function queue");
				throw;
			}

			this.currentDelayValue -= 1;

			lock (this)
			{
				this.Executing = false;
			}
		}
	}
}
