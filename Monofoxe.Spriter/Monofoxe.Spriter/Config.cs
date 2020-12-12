// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

namespace Monofoxe.Spriter
{
	public class Config
	{
		/// <summary>
		/// Enables ALL metadata calculations.
		/// </summary>
		public bool MetadataEnabled;

		public bool VarsEnabled;
		public bool TagsEnabled;
		public bool EventsEnabled;
		public bool SoundsEnabled;

		/// <summary>
		/// Enables object pooling.
		/// </summary>
		public bool PoolingEnabled;

		public Config()
		{
			MetadataEnabled = true;
			VarsEnabled = true;
			TagsEnabled = true;
			EventsEnabled = true;
			SoundsEnabled = true;
			PoolingEnabled = true;
		}
	}
}
