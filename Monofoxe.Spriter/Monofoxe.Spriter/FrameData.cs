// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using System.Collections.Generic;

namespace Monofoxe.Spriter
{
	public class FrameData
	{
		public readonly List<SpriterObject> SpriteData = new List<SpriterObject>();
		public readonly Dictionary<string, SpriterObject> PointData = new Dictionary<string, SpriterObject>();
		public readonly Dictionary<int, SpriterObject> BoxData = new Dictionary<int, SpriterObject>();
		public readonly Dictionary<string, SpriterVarValue> AnimationVars = new Dictionary<string, SpriterVarValue>();
		public readonly Dictionary<string, Dictionary<string, SpriterVarValue>> ObjectVars = new Dictionary<string, Dictionary<string, SpriterVarValue>>();
		public readonly List<string> AnimationTags = new List<string>();
		public readonly Dictionary<string, List<string>> ObjectTags = new Dictionary<string, List<string>>();
		public readonly List<string> Events = new List<string>();
		public readonly List<SpriterSound> Sounds = new List<SpriterSound>();

		private readonly ObjectPool _pool;

		public FrameData(ObjectPool pool)
		{
			_pool = pool;
		}

		public void Clear()
		{
			_pool.ReturnChildren(SpriteData);
			_pool.ReturnChildren(PointData);
			_pool.ReturnChildren(BoxData);

			var varE = ObjectVars.GetEnumerator();
			while (varE.MoveNext())
			{
				_pool.ReturnChildren(varE.Current.Value);
				_pool.ReturnObject(varE.Current.Value);
			}
			ObjectVars.Clear();

			var tagE = ObjectTags.GetEnumerator();
			while (tagE.MoveNext())
			{
				var list = tagE.Current.Value;
				list.Clear();
				_pool.ReturnObject(list);
			}
			ObjectTags.Clear();

			Sounds.Clear();
			AnimationVars.Clear();
			AnimationTags.Clear();
			Events.Clear();
		}

		public void AddObjectVar(string objectName, string varName, SpriterVarValue value)
		{
			Dictionary<string, SpriterVarValue> values;
			if (!ObjectVars.TryGetValue(objectName, out values))
			{
				values = _pool.GetObject<Dictionary<string, SpriterVarValue>>();
				ObjectVars[objectName] = values;
			}
			values[varName] = value;
		}

		public void AddObjectTag(string objectName, string tag)
		{
			List<string> tags;
			if (!ObjectTags.TryGetValue(objectName, out tags))
			{
				tags = _pool.GetObject<List<string>>();
				ObjectTags[objectName] = tags;
			}
			tags.Add(tag);
		}
	}
}
