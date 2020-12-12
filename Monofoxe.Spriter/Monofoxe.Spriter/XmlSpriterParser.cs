// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Monofoxe.Spriter
{
	public class XmlSpriterParser
	{
		private static readonly string XmlStart = "<";

		public virtual SpriterData Parse(string data)
		{
			data = FixBadNanValue(data);
			var serializer = new XmlSerializer(typeof(SpriterData));
			using (var reader = new StringReader(data))
			{
				var spriter = serializer.Deserialize(reader) as SpriterData;
				return spriter;
			}
		}

		public virtual bool CanParse(string data) =>
			data.StartsWith(XmlStart);
		

		private static string FixBadNanValue(string data)
		{
			var nanRegex = new Regex(@"(a)=""nan""");
			data = nanRegex.Replace(data, @"$1=""0""");
			return data;
		}
	}
}
