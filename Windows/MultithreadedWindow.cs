namespace WpfUtils.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

public class MultithreadedWindow : Window
{
	public static async Task<T?> CreateInstanceAsync<T>()
		where T : MultithreadedWindow
	{
		return await new WindowThread().Start(typeof(T)) as T;
	}

	private class WindowThread
	{
		private static readonly object CreateInstanceLock = new();

		private Window? window;
		private Type? windowType;

		protected string Description
		{
			get
			{
				if (this.window != null)
					return this.window.ToString();

				if (this.windowType != null)
					return this.windowType.ToString();

				return "Unknown";
			}
		}

		public async Task<Window?> Start(Type windowType)
		{
			try
			{
				this.windowType = windowType;

				Thread panelMainThread = new Thread(this.PanelMainThread);
				panelMainThread.SetApartmentState(ApartmentState.STA);
				panelMainThread.Start(this);

				// Wait for the panel to load for up to 5 seconds.
				int timeOut = 5000;
				while (this.window == null && timeOut > 0)
				{
					await Task.Delay(10);
					timeOut -= 10;
				}

				if (this.window == null)
					Logging.Log.Error(null, $"Failed to create window {this.Description}");

				return this.window;
			}
			catch (Exception ex)
			{
				Logging.Log.Error(ex, $"Failed to create window {this.Description}");
			}

			return null;
		}

		private void PanelMainThread(object? param)
		{
			if (this.windowType == null)
				throw new Exception("No panel type in thread");

			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
			{
				Exception? ex = e.ExceptionObject as Exception;
				Logging.Log.Error(ex, $"Unhandled Exception in domain: {this.Description}");
			};

			System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += (s, e) =>
			{
				Logging.Log.Error(e.Exception, $"Unhandled Exception in window: {this.Description}");
				e.Handled = true;
			};

			try
			{
				// Even though we're doing this on another thread, we can still only do one panel window
				// at a time since WPF's LoadComponent system isn't thread safe.
				lock (WindowThread.CreateInstanceLock)
				{
					this.window = Activator.CreateInstance(this.windowType) as Window;
				}
			}
			catch (Exception ex)
			{
				Logging.Log.Error(ex, $"Exception during window construction: {this.Description}");
				return;
			}

			Logging.Log.Message($"Window: {this.Description} has started");

			bool run = true;
			while (run)
			{
				try
				{
					System.Windows.Threading.Dispatcher.Run();
					run = false;
				}
				catch (Exception ex)
				{
					Logging.Log.Error(ex, $"Error in {this.Description} thread");
				}
			}

			Logging.Log.Message($"Window: {this.Description} has shutdown");
		}
	}
}
