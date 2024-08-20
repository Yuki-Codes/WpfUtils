namespace System;

using FontAwesome.Sharp;

public static class SpecialFolderExtensions
{
	public static IconChar? ToIcon(this Environment.SpecialFolder self)
	{
		switch (self)
		{
			case Environment.SpecialFolder.Desktop: return IconChar.Desktop;
			case Environment.SpecialFolder.Programs: return IconChar.WindowMaximize;
			case Environment.SpecialFolder.MyDocuments: return IconChar.FileLines;
			case Environment.SpecialFolder.Favorites: return IconChar.Star;
			case Environment.SpecialFolder.MyMusic: return IconChar.Music;
			case Environment.SpecialFolder.MyVideos: return IconChar.Film;
			case Environment.SpecialFolder.DesktopDirectory: return IconChar.Desktop;
			case Environment.SpecialFolder.MyComputer: return IconChar.Computer;
			case Environment.SpecialFolder.NetworkShortcuts: return IconChar.NetworkWired;
			case Environment.SpecialFolder.Fonts: return IconChar.Font;
			case Environment.SpecialFolder.ApplicationData: return IconChar.Server;
			case Environment.SpecialFolder.LocalApplicationData: return IconChar.Server;
			case Environment.SpecialFolder.CommonApplicationData: return IconChar.Server;
			case Environment.SpecialFolder.Windows: return IconChar.Windows;
			case Environment.SpecialFolder.System: return IconChar.Windows;
			case Environment.SpecialFolder.ProgramFiles: return IconChar.WindowMaximize;
			case Environment.SpecialFolder.MyPictures: return IconChar.Images;
			case Environment.SpecialFolder.UserProfile: return IconChar.User;
			case Environment.SpecialFolder.SystemX86: return IconChar.Windows;
			case Environment.SpecialFolder.ProgramFilesX86: return IconChar.WindowMaximize;
			case Environment.SpecialFolder.CommonProgramFiles: return IconChar.WindowMaximize;
			case Environment.SpecialFolder.CommonProgramFilesX86: return IconChar.WindowMaximize;
			case Environment.SpecialFolder.CommonDocuments: return IconChar.File;
			case Environment.SpecialFolder.CommonMusic: return IconChar.Music;
			case Environment.SpecialFolder.CommonPictures: return IconChar.Images;
			case Environment.SpecialFolder.CommonVideos: return IconChar.Film;
		}

		return null;
	}
}
