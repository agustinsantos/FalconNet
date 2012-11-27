using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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
			return Regex.Split (str.Trim(), @"\W+");
			// @      special verbatim string syntax
			// \W+    one or more non-word characters together
		}
		

		private static Regex regex = new Regex( @"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+)|(?<token>\[[\w]+\]))(\s)*)", RegexOptions.None );
		public static List<string> SplitWords (this string input)
		{
		    var result = (from Match m in regex.Matches( input ) 
		                  where m.Groups[ "token" ].Success
		                  select m.Groups[ "token" ].Value).ToList();
			return result;
		}
	}
}
