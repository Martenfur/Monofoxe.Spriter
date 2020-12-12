// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Helpers;
using Monofoxe.Spriter.Models;
using Microsoft.Xna.Framework.Audio;
using Monofoxe.Engine.Drawing;
using System.Collections.Generic;

namespace Monofoxe.Spriter.Providers
{
	public delegate void AnimationModifierDelegate(SpriterAnimation first);

	public class ResourceProviderFactory
	{
		protected Dictionary<SpriterEntity, FrameDataProvider> _animProviders = new Dictionary<SpriterEntity, FrameDataProvider>();
		protected Dictionary<SpriterData, AssetProvider<Sprite>> _spriteProviders = new Dictionary<SpriterData, AssetProvider<Sprite>>();
		protected Dictionary<SpriterData, AssetProvider<SoundEffect>> _soundProviders = new Dictionary<SpriterData, AssetProvider<SoundEffect>>();

		protected Config _config;
		protected ObjectPool _pool;

		public ResourceProviderFactory(Config config)
		{
			_config = config;
			_pool = new ObjectPool(config);
		}

		public AssetProvider<Sprite> GetSpriteProvider(SpriterEntity entity)
		{
			var provider = _spriteProviders.GetOrCreate(entity.Spriter);
			return new AssetProvider<Sprite>(provider.AssetMappings);
		}

		public AssetProvider<SoundEffect> GetSoundProvider(SpriterEntity entity)
		{
			var provider = _soundProviders.GetOrCreate(entity.Spriter);
			return new AssetProvider<SoundEffect>(provider.AssetMappings);
		}

		public void SetSprite(SpriterData spriter, SpriterFolder folder, SpriterFile file, Sprite sprite)
		{
			var provider = _spriteProviders.GetOrCreate(spriter);
			provider.Set(folder.Id, file.Id, sprite);
		}

		public void SetSound(SpriterData spriter, SpriterFolder folder, SpriterFile file, SoundEffect sound)
		{
			var provider = _soundProviders.GetOrCreate(spriter);
			provider.Set(folder.Id, file.Id, sound);
		}
	}
}
