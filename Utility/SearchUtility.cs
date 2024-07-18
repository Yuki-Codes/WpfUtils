namespace WpfUtils;

using System.Linq;
using System.Text.RegularExpressions;

public static class SearchUtility
{
	public static string[] ToQuery(string input)
	{
		return input.Split(' ');
	}

	public static bool Matches(object input, string[]? query) => Matches(input.ToString(), query);

	public static bool Matches(string? input, string[]? query)
	{
		if (input == null)
			return false;

		if (query == null)
			return true;

		input = input.ToLower();
		input = Regex.Replace(input, @"[^\w\d\s]", string.Empty);

		bool matchesSearch = true;
		foreach (string str in query)
		{
			string strB = str.ToLower();

			// ignore 'the'
			if (strB == "the")
				continue;

			// ignore all symbols
			strB = Regex.Replace(strB, @"[^\w\d\s]", string.Empty);

			// Parse integers as numbers instead of strings
			if (int.TryParse(str, out int v))
			{
				matchesSearch &= input.Contains(v.ToString());
			}
			else
			{
				matchesSearch &= input.Contains(strB);
			}
		}

		if (!matchesSearch)
		{
			return false;
		}

		return true;
	}

	public static double Search(object input, string[]? query) => Search(input.ToString(), query);

	/// <summary>
	/// Search the given input string for a query array of words.
	/// </summary>
	/// <param name="input">a string to search.</param>
	/// <param name="query">an array of input words.</param>
	/// <returns>a double between 0 and 1, 0 being no match, 1 being an exact match.</returns>
	public static double Search(string? input, string[]? query)
	{
		if (input == null)
			return 0;

		if (query == null)
			return 0;

		input = input.ToLower();
		input = Regex.Replace(input, @"[^\w\d\s]", string.Empty);

		double result = 0;
		foreach (string str in query)
		{
			string strB = str.ToLower();

			// ignore 'the'
			if (strB == "the")
				continue;

			// ignore all symbols
			strB = Regex.Replace(strB, @"[^\w\d\s]", string.Empty);

			// Parse integers as numbers instead of strings
			if (int.TryParse(str, out int v))
			{
				if (input.Contains(v.ToString()))
				{
					result += 1;
				}
			}
			else
			{
				int matchCount = CommonCharacters(strB, input);
				int charCount = input.Length;
				result += (double)matchCount / (double)charCount;
			}
		}

		result /= query.Length;

		return result;
	}

	private static int CommonCharacters(string s1, string s2)
	{
		bool[] matchedFlag = new bool[s2.Length];

		for (int i1 = 0; i1 < s1.Length; i1++)
		{
			for (int i2 = 0; i2 < s2.Length; i2++)
			{
				if (!matchedFlag[i2] && s1.ToCharArray()[i1] == s2.ToCharArray()[i2])
				{
					matchedFlag[i2] = true;
					break;
				}
			}
		}

		return matchedFlag.Count(u => u);
	}
}
