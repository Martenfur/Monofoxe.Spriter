// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using System.Collections.Generic;
using System;
using Monofoxe.Spriter.Models;

namespace Monofoxe.Spriter
{
	/// <summary>
	/// Class responsible for getting a Spriter instance from a string input. It is also responsible for any processing logic such as initialisation.
	/// This class basically contains no parsing / processing logic by itself but has collections of parsers / preprocessors to delegate the work to.
	///
	/// For parsing, it iterates over all registered parses until a parser can parse the input string or until it reaches the end.
	///
	/// For preprocessing, it invokes all the preprocessors in order.
	/// </summary>
	public class SpriterReader
	{
		/// <summary>
		/// An instance of the default Spriter reader.
		/// </summary>
		public static readonly SpriterReader Default;

		static SpriterReader()
		{
			Default = new SpriterReader();
			Default.Parsers.Add(new XmlSpriterParser());
			Default.Preprocessors.Add(new SpriterInitPreprocessor());
		}

		public ICollection<XmlSpriterParser> Parsers = new List<XmlSpriterParser>();
		public ICollection<SpriterInitPreprocessor> Preprocessors = new List<SpriterInitPreprocessor>();

		public virtual SpriterData Read(string data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			data = data.Trim();
			if (string.IsNullOrEmpty(data)) 
			{
				return null;
			}

			SpriterData spriter = null;
			foreach (var parser in Parsers)
			{
				if (!parser.CanParse(data))
				{
					continue;
				}
				spriter = parser.Parse(data);
				break;
			}

			foreach (var preprocessor in Preprocessors)
			{
				preprocessor.Init(spriter);
			}

			return spriter;
		}
	}
}
