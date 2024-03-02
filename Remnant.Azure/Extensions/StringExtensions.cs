namespace Remnant.Azure.Extensions;

public static class StringExtensions
{
	public static string RemoveChars(this string str, params string[] removeChars)
	{
		if (!string.IsNullOrEmpty(str))
		{
			foreach (var item in removeChars)
				str = str.Replace(item, string.Empty);
		}

		return str;
	}
}
