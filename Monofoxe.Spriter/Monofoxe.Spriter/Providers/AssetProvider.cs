﻿// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Helpers;
using Monofoxe.Spriter.Models;
using System.Collections.Generic;

namespace Monofoxe.Spriter.Providers
{
	public class AssetProvider<T>
	{
		public SpriterCharacterMap CharacterMap => CharMaps.Count > 0 ? CharMaps.Peek() : null;

		public Dictionary<int, Dictionary<int, T>> AssetMappings { get; protected set; }

		protected Dictionary<T, T> SwappedAssets = new Dictionary<T, T>();
		protected Dictionary<T, KeyValuePair<int, int>> CharMapValues = new Dictionary<T, KeyValuePair<int, int>>();
		protected Stack<SpriterCharacterMap> CharMaps = new Stack<SpriterCharacterMap>();

		public AssetProvider() : this(new Dictionary<int, Dictionary<int, T>>())
		{
		}

		public AssetProvider(Dictionary<int, Dictionary<int, T>> assetMappings)
		{
			AssetMappings = assetMappings;
		}

		public virtual T Get(int folderId, int fileId)
		{
			T asset = GetAsset(folderId, fileId);
			if (asset == null) 
			{ 
				return asset;
			}

			if (CharMapValues.ContainsKey(asset))
			{
				KeyValuePair<int, int> mapping = CharMapValues[asset];
				if (mapping.Key == folderId && mapping.Value == fileId)
				{
					return asset;
				}
				return Get(mapping.Key, mapping.Value);
			}

			return SwappedAssets.ContainsKey(asset) ? SwappedAssets[asset] : asset;
		}

		public virtual KeyValuePair<int, int> GetMapping(int folderId, int fileId)
		{
			T asset = GetAsset(folderId, fileId);
			if (asset == null || !CharMapValues.ContainsKey(asset))
			{
				return new KeyValuePair<int, int>(folderId, fileId);
			}
			return CharMapValues[asset];
		}

		public virtual void Set(int folderId, int fileId, T asset)
		{
			var objectsByFiles = AssetMappings.GetOrCreate(folderId);
			objectsByFiles[fileId] = asset;
		}

		public virtual void Swap(T original, T replacement)
		{
			SwappedAssets[original] = replacement;
		}

		public virtual void Unswap(T original)
		{
			if (SwappedAssets.ContainsKey(original))
			{
				SwappedAssets.Remove(original);
			}
		}

		public virtual void PushCharMap(SpriterCharacterMap charMap)
		{
			ApplyCharMap(charMap);
			CharMaps.Push(charMap);
		}

		public virtual void PopCharMap()
		{
			if (CharMaps.Count == 0)
			{
				return;
			}
			CharMaps.Pop();
			ApplyCharMap(CharMaps.Count > 0 ? CharMaps.Peek() : null);
		}

		protected virtual void ApplyCharMap(SpriterCharacterMap charMap)
		{
			if (charMap == null)
			{
				CharMapValues.Clear();
				return;
			}

			for (int i = 0; i < charMap.Maps.Length; i += 1)
			{
				var map = charMap.Maps[i];
				var sprite = GetAsset(map.FolderId, map.FileId);
				if (sprite == null)
				{
					continue;
				}

				CharMapValues[sprite] = new KeyValuePair<int, int>(map.TargetFolderId, map.TargetFileId);
			}
		}

		protected virtual T GetAsset(int folderId, int fileId)
		{
			AssetMappings.TryGetValue(folderId, out var objectsByFiles);
			if (objectsByFiles == null) 
			{ 
				return default;
			}

			objectsByFiles.TryGetValue(fileId, out var obj);

			return obj;
		}
	}
}
