namespace WpfUtils.Extensions;

using System;
using System.Threading.Tasks;
using WpfUtils.Logging;

public static class TaskExtensions
{
	public static void Run(this Task self)
	{
		Task.Run(async () =>
		{
			try
			{
				await self;
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error in Task");
			}
		});
	}
}
