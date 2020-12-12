// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using Microsoft.Xna.Framework.Content;

namespace Monofoxe.Spriter.Monofoxe.Content
{
	public class SpriterTypeReader : ContentTypeReader<SpriterData>
	{
		public static SpriterReader Reader;

		static SpriterTypeReader()
		{
			Reader = SpriterReader.Default;
		}

		protected override SpriterData Read(ContentReader input, SpriterData existingInstance)
		{
			var data = input.ReadString();
			return Reader.Read(data);
		}
	}
}
