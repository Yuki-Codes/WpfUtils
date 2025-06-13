﻿// WPF
// https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media/Animation/EasingFunctionBase.cs
// https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationCore/System/Windows/Media/Animation/SineEase.cs

namespace WpfUtils.Animation;
using System;

// reimplements WPF's animation easing, but optimized for floats, and without any dependency properties.
public abstract class EasingFunctionBase
{
	public enum EasingModes
	{
		EaseInOut,
		EaseIn,
		EaseOut,
	}

	public float Ease(float normalizedTime, EasingModes mode)
	{
		switch (mode)
		{
			case EasingModes.EaseIn:
			{
				return this.Ease(normalizedTime);
			}

			case EasingModes.EaseOut:
			{
				// EaseOut is the same as EaseIn, except time is reversed & the result is flipped.
				return 1.0f - this.Ease(1.0f - normalizedTime);
			}

			case EasingModes.EaseInOut:
			{
				// EaseInOut is a combination of EaseIn & EaseOut fit to the 0-1, 0-1 range.
				return (normalizedTime < 0.5f) ? this.Ease(normalizedTime * 2.0f) * 0.5f : ((1.0f - this.Ease((1.0f - normalizedTime) * 2.0f)) * 0.5f) + 0.5f;
			}
		}

		return 0;
	}

	protected abstract float Ease(float normalizedTime);
}

public class SineEase : EasingFunctionBase
{
	protected override float Ease(float normalizedTime)
	{
		return 1.0f - MathF.Sin(MathF.PI * 0.5f * (1 - normalizedTime));
	}
}
