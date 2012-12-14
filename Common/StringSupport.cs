using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FalconNet.Common
{
	public static class StringExtensions
	{
		/// <summary>
		/// Take all the words in the input string and separate them.
		/// </summary>
		public static string[] SplitSimpleWords (this string str)
		{
			//
			// Split on all non-word characters.
			// ... Returns an array of all the words.
			//
			return Regex.Split (str.Trim (), @"\W+");
			// @      special verbatim string syntax
			// \W+    one or more non-word characters together
		}

		private static Regex regex = new Regex (@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+)|(?<token>\[[\w]+\]))(\s)*)", RegexOptions.None);

		public static List<string> SplitWords (this string input)
		{
			var result = (from Match m in regex.Matches (input) 
		                  where m.Groups ["token"].Success
		                  select m.Groups ["token"].Value).ToList ();
			return result;
		}
		
		public static List<string> Tokenize (this string input)
		{
			var r = new Regex (@"[\s]+");
			var tokens = r.Split (input);
			var tokenList = new List<string> ();
			foreach (var token in tokens) {
				if (token.Trim ().Length == 0)
					continue;
				if (token.EndsWith (";")) {
					var thisToken = token;
					var tokensReplaced = 0;
					while (thisToken.EndsWith(";")) {
						thisToken = thisToken.Substring (0, thisToken.Length - 1);
						tokensReplaced++;
					}
					tokenList.Add (thisToken);
					for (var i = 0; i < tokensReplaced; i++) {
						tokenList.Add (";");
					}
				} else if (token.StartsWith ("//")) {
					var thisToken = token;
					var tokensReplaced = 0;
					while (thisToken.StartsWith("//")) {
						thisToken = thisToken.Substring (2, thisToken.Length - 2);
						tokensReplaced++;
					}
					for (var i = 0; i < tokensReplaced; i++) {
						tokenList.Add ("//");
					}
					tokenList.Add (thisToken);
				} else {
					tokenList.Add (token);
				}
			}
			return tokenList;
		}

		public static byte[] GetBytesInDefaultEncoding (string aString)
		{
			if (aString != null)
				return Encoding.Default.GetBytes (aString);
			return new byte[0];
		}
		
		public static string GetOSPath (this string path)
		{
			switch (Environment.OSVersion.Platform) {
			case PlatformID.Unix:
			case PlatformID.MacOSX:
				return path.Replace ('\\', Path.DirectorySeparatorChar);
				break;
			default:
				return path.Replace ('/', Path.DirectorySeparatorChar);
				break;
			}
		}
	}
}
