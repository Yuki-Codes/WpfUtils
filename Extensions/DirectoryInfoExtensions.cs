namespace System.IO;

using System;
using System.Collections.Generic;

public static class DirectoryInfoExtensions
{
	private static readonly Dictionary<string, Environment.SpecialFolder> SpecialFolderLookup = new();

	static DirectoryInfoExtensions()
	{
		foreach (Environment.SpecialFolder folder in Enum.GetValues<Environment.SpecialFolder>())
		{
			string path = Environment.GetFolderPath(folder).ToLowerInvariant();

			if (SpecialFolderLookup.ContainsKey(path))
				continue;

			SpecialFolderLookup.Add(path, folder);
		}
	}

	public static bool IsSpecialFolder(this DirectoryInfo self)
	{
		return SpecialFolderLookup.ContainsKey(self.FullName.ToLowerInvariant());
	}

	public static Environment.SpecialFolder? GetSpecialFolder(this DirectoryInfo self)
	{
		Environment.SpecialFolder folder;
		if (!SpecialFolderLookup.TryGetValue(self.FullName.ToLowerInvariant(), out folder))
			return null;

		return folder;
	}
}
