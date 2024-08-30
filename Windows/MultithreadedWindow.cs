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
		return await new PanelWindowThread().Start(typeof(T)) as T;
	}

	private class PanelWindowThread
	{
		private static readonly object CreateInstanceLock = new();

		private Window? window;
		private Type? windowType;

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
					Logging.Log.Error(null, $"Failed to create window {this.windowType}");

				return this.window;
			}
			catch (Exception ex)
			{
				Logging.Log.Error(ex, $"Failed to create window {this.windowType}");
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
				Logging.Log.Error(ex, $"Unhandled Exception in window: {this.windowType}");
			};

			System.Windows.Threading.Dispatcher.CurrentDispatcher.UnhandledException += (s, e) =>
			{
				Logging.Log.Error(e.Exception, $"Unhandled Exception in window: {this.windowType}");
			};

			try
			{
				// Even though we're doing this on another thread, we can still only do one panel window
				// at a time since WPF's LoadComponent system isn't thread safe.
				lock (PanelWindowThread.CreateInstanceLock)
				{
					this.window = Activator.CreateInstance(this.windowType) as Window;
				}
			}
			catch (Exception ex)
			{
				Logging.Log.Error(ex, $"Exception during window construction: {this.windowType}");
				return;
			}

			Logging.Log.Message($"Window: {this.windowType} has started");

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
					Logging.Log.Error(ex, $"Error in {this.windowType} thread");
				}
			}

			Logging.Log.Message($"Window: {this.windowType} has shutdown");
		}
	}
}
