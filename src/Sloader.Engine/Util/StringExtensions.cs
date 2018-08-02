using System;
using System.Linq;

namespace Sloader.Engine.Util
{
	/// <summary>
	/// Utility functions for strings
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Some services may return "bad strings", e.g. strings with control chars in it. This method removes any "bad" chars.
		/// <para>Be aware that linefeeds are "OK"</para>
		/// </summary>
		/// <param name="s">input string</param>
		/// <returns>clean output string or in case of null it returns string.empty</returns>
		public static string ToCleanString(this string s)
		{
			var hChars = new char[] { '\u007f' };

			return s == null ? string.Empty : new string(s.Where(c => !hChars.Contains(c)).ToArray()); ;
		}

	}
}
