// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Helpers;
using Monofoxe.Spriter.Models;
using System;

namespace Monofoxe.Spriter.Providers
{
	/// <summary>
	/// Default IFrameDataProvider implementation. It simply calculates the frame data for every frame.
	/// </summary>
	public class FrameDataProvider
	{
		internal AnimationModifierDelegate AnimationModifier;

		protected FrameData _frameData;
		protected Config _config;
		protected ObjectPool _pool;


		public FrameDataProvider()
		{
			_config = new Config();
			_pool = new ObjectPool(_config);
			_frameData = new FrameData(_pool);
		}

		public FrameDataProvider(Config config, ObjectPool pool)
		{
			_config = config;
			_pool = pool;
			_frameData = new FrameData(_pool);
		}

		public FrameData GetFrameData(float time, float deltaTime, float factor, SpriterAnimation first, SpriterAnimation second = null)
		{
			if (second == null)
			{
				return GetFrameData(first, time, deltaTime);
			}
			return GetFrameData(first, second, time, deltaTime, factor);
		}


		private FrameData GetFrameData(SpriterAnimation first, SpriterAnimation second, float targetTime, float deltaTime, float factor)
		{
			_frameData.Clear();

			if (first == second)
			{
				GetFrameData(first, targetTime, deltaTime);
				return _frameData;
			}

			if (AnimationModifier != null)
			{
				AnimationModifier(first);
				AnimationModifier(second);
			}

			float targetTimeSecond = 0;//targetTime / first.Length * second.Length;

			GetMainlineKeys(first.MainlineKeys, targetTime, out var firstKeyA, out var firstKeyB);

			GetMainlineKeys(second.MainlineKeys, targetTimeSecond, out var secondKeyA, out var secondKeyB);

			if (!SpriterHelper.WillItBlend(firstKeyA, secondKeyA) || !SpriterHelper.WillItBlend(firstKeyB, secondKeyB))
			{
				GetFrameData(first, targetTime, deltaTime);
				return _frameData;
			}

			float adjustedTimeFirst = SpriterHelper.AdjustTime(targetTime, firstKeyA, firstKeyB, first.Length);
			float adjustedTimeSecond = SpriterHelper.AdjustTime(targetTimeSecond, secondKeyA, secondKeyB, second.Length);

			var boneInfosA = GetBoneInfos(firstKeyA, first, adjustedTimeFirst);
			var boneInfosB = GetBoneInfos(secondKeyA, second, adjustedTimeSecond);
			SpriterSpatial[] boneInfos = null;
			
			if (boneInfosA != null && boneInfosB != null)
			{
				boneInfos = _pool.GetArray<SpriterSpatial>(boneInfosA.Length);
				for (int i = 0; i < boneInfosA.Length; i += 1)
				{
					var boneA = boneInfosA[i];
					var boneB = boneInfosB[i];
					var interpolated = Interpolate(boneA, boneB, factor, 1);
					interpolated.Angle = MathHelper.CloserAngleLinear(boneA.Angle, boneB.Angle, factor);
					boneInfos[i] = interpolated;
				}
			}

			var baseKey = factor < 0.5f ? firstKeyA : firstKeyB;
			var currentAnimation = factor < 0.5f ? first : second;

			for (var i = 0; i < baseKey.ObjectRefs.Length; i += 1)
			{
				var objectRefFirst = baseKey.ObjectRefs[i];
				var interpolatedFirst = GetObjectInfo(objectRefFirst, first, adjustedTimeFirst);

				var objectRefSecond = secondKeyA.ObjectRefs[i];
				var interpolatedSecond = GetObjectInfo(objectRefSecond, second, adjustedTimeSecond);

				var info = Interpolate(interpolatedFirst, interpolatedSecond, factor, 1);
				info.Angle = MathHelper.CloserAngleLinear(interpolatedFirst.Angle, interpolatedSecond.Angle, factor);
				info.PivotX = MathHelper.Linear(interpolatedFirst.PivotX, interpolatedSecond.PivotX, factor);
				info.PivotY = MathHelper.Linear(interpolatedFirst.PivotY, interpolatedSecond.PivotY, factor);


				if (boneInfos != null && objectRefFirst.ParentId >= 0)
				{
					info.ApplyParentTransform(boneInfos[objectRefFirst.ParentId]);
				}


				AddSpatialData(info, currentAnimation.Timelines[objectRefFirst.TimelineId], currentAnimation.Entity.Spriter, deltaTime);

				_pool.ReturnObject(interpolatedFirst);
				_pool.ReturnObject(interpolatedSecond);
			}

			_pool.ReturnObject(boneInfosA);
			_pool.ReturnObject(boneInfosB);
			_pool.ReturnObject(boneInfos);

			if (_config.MetadataEnabled)
			{
				UpdateMetadata(currentAnimation, targetTime, deltaTime);
			}

			return _frameData;
		}

		private FrameData GetFrameData(SpriterAnimation animation, float targetTime, float deltaTime, SpriterSpatial parentInfo = null)
		{
			if (parentInfo == null)
			{
				_frameData.Clear();
			}

			AnimationModifier?.Invoke(animation);


			GetMainlineKeys(animation.MainlineKeys, targetTime, out var keyA, out var keyB);

			float adjustedTime = SpriterHelper.AdjustTime(targetTime, keyA, keyB, animation.Length);

			var boneInfos = GetBoneInfos(keyA, animation, adjustedTime, parentInfo);

			if (keyA.ObjectRefs == null)
			{
				_pool.ReturnObject(boneInfos);
				return _frameData;
			}

			for (var i = 0; i < keyA.ObjectRefs.Length; i += 1)
			{
				var objectRef = keyA.ObjectRefs[i];
				var interpolated = GetObjectInfo(objectRef, animation, adjustedTime);
				if (boneInfos != null && objectRef.ParentId >= 0)
				{
					interpolated.ApplyParentTransform(boneInfos[objectRef.ParentId]);
				}
				else
				{
					if (parentInfo != null)
					{
						interpolated.ApplyParentTransform(parentInfo);
					}
				}

				AddSpatialData(interpolated, animation.Timelines[objectRef.TimelineId], animation.Entity.Spriter, deltaTime);
			}

			_pool.ReturnObject(boneInfos);

			if (_config.MetadataEnabled)
			{
				UpdateMetadata(animation, targetTime, deltaTime);
			}

			return _frameData;
		}

		protected void UpdateMetadata(SpriterAnimation animation, float targetTime, float deltaTime)
		{
			if (_config.VarsEnabled || _config.TagsEnabled)
			{
				AddVariableAndTagData(animation, targetTime);
			}
			if (_config.EventsEnabled)
			{
				AddEventData(animation, targetTime, deltaTime);
			}
			if (_config.SoundsEnabled)
			{
				AddSoundData(animation, targetTime, deltaTime);
			}
		}

		protected void AddVariableAndTagData(SpriterAnimation animation, float targetTime)
		{
			if (animation.Meta == null)
			{
				return;
			}

			if (_config.VarsEnabled && animation.Meta.Varlines != null && animation.Meta.Varlines.Length > 0)
			{
				for (var i = 0; i < animation.Meta.Varlines.Length; i += 1)
				{
					var varline = animation.Meta.Varlines[i];
					var variable = animation.Entity.Variables[varline.Def];
					_frameData.AnimationVars[variable.Name] = GetVariableValue(animation, variable, varline, targetTime);
				}
			}

			var tags = animation.Entity.Spriter.Tags;
			var tagline = animation.Meta.Tagline;
			if (_config.TagsEnabled && tagline != null && tagline.Keys != null && tagline.Keys.Length > 0)
			{
				var key = tagline.Keys.GetLastKey(targetTime);
				if (key != null && key.Tags != null)
				{
					for (var i = 0; i < key.Tags.Length; i += 1)
					{
						var tag = key.Tags[i];
						_frameData.AnimationTags.Add(tags[tag.TagId].Name);
					}
				}
			}

			for (int i = 0; i < animation.Timelines.Length; i += 1)
			{
				var timeline = animation.Timelines[i];
				var meta = timeline.Meta;
				if (meta == null)
				{
					continue;
				}

				var objInfo = GetObjectInfo(animation, timeline.Name);

				if (_config.VarsEnabled && meta.Varlines != null && meta.Varlines.Length > 0)
				{
					for (var j = 0; j < timeline.Meta.Varlines.Length; j += 1)
					{
						var varline = timeline.Meta.Varlines[j];
						var variable = objInfo.Variables[varline.Def];
						_frameData.AddObjectVar(objInfo.Name, variable.Name, GetVariableValue(animation, variable, varline, targetTime));
					}
				}

				if (_config.TagsEnabled && meta.Tagline != null && meta.Tagline.Keys != null && meta.Tagline.Keys.Length > 0)
				{
					var key = tagline.Keys.GetLastKey(targetTime);
					if (key != null && key.Tags != null)
					{
						for (var j = 0; j < key.Tags.Length; j += 1)
						{
							var tag = key.Tags[j];
							_frameData.AddObjectTag(objInfo.Name, tags[tag.TagId].Name);
						}
					}
				}
			}
		}

		protected SpriterObjectInfo GetObjectInfo(SpriterAnimation animation, string name)
		{
			SpriterObjectInfo objInfo = null;
			for (var i = 0; i < animation.Entity.ObjectInfos.Length; i += 1)
			{
				var info = animation.Entity.ObjectInfos[i];
				if (info.Name == name)
				{
					objInfo = info;
					break;
				}
			}

			return objInfo;
		}

		protected SpriterVarValue GetVariableValue(SpriterAnimation animation, SpriterVarDef varDef, SpriterVarline varline, float targetTime)
		{
			var keys = varline.Keys;
			if (keys == null)
			{
				return varDef.VariableValue;
			}

			var keyA = keys.GetLastKey(targetTime) ?? keys[keys.Length - 1];

			if (keyA == null)
			{
				return varDef.VariableValue;
			}

			var keyB = varline.Keys.GetNextKey(keyA, animation.Looping);

			if (keyB == null)
			{
				return keyA.VariableValue;
			}

			var adjustedTime = keyA.Time == keyB.Time ? targetTime : SpriterHelper.AdjustTime(targetTime, keyA, keyB, animation.Length);
			var factor = SpriterHelper.GetFactor(keyA, keyB, animation.Length, adjustedTime);

			var varVal = _pool.GetObject<SpriterVarValue>();
			varVal.Interpolate(keyA.VariableValue, keyB.VariableValue, factor);
			return varVal;
		}

		protected void AddEventData(SpriterAnimation animation, float targetTime, float deltaTime)
		{
			if (animation.Eventlines == null)
			{
				return;
			}

			float previousTime = targetTime - deltaTime;
			for (var i = 0; i < animation.Eventlines.Length; i += 1)
			{
				var eventline = animation.Eventlines[i];
				for (var j = 0; j < eventline.Keys.Length; j += 1)
				{
					var key = eventline.Keys[j];
					if (IsTriggered(key, targetTime, previousTime, animation.Length))
					{
						_frameData.Events.Add(eventline.Name);
					}
				}
			}
		}

		protected void AddSoundData(SpriterAnimation animation, float targetTime, float deltaTime)
		{
			if (animation.Soundlines == null)
			{
				return;
			}

			var previousTime = targetTime - deltaTime;
			for (var i = 0; i < animation.Soundlines.Length; i += 1)
			{
				var soundline = animation.Soundlines[i];
				for (var j = 0; j < soundline.Keys.Length; j += 1)
				{
					var key = soundline.Keys[j];
					var sound = key.SoundObject;
					if (sound.Trigger && IsTriggered(key, targetTime, previousTime, animation.Length))
					{
						_frameData.Sounds.Add(sound);
					}
				}
			}
		}

		protected bool IsTriggered(SpriterKey key, float targetTime, float previousTime, float animationLength)
		{
			var timeA = Math.Min(previousTime, targetTime);
			var timeB = Math.Max(previousTime, targetTime);
			if (timeA > timeB)
			{
				if (timeA < key.Time)
				{
					timeB += animationLength;
				}
				else
				{
					timeA -= animationLength;
				}
			}
			return timeA <= key.Time && timeB >= key.Time;
		}

		protected void AddSpatialData(SpriterObject info, SpriterTimeline timeline, SpriterData spriter, float deltaTime)
		{
			switch (timeline.ObjectType)
			{
				case SpriterObjectType.Sprite:
					_frameData.SpriteData.Add(info);
					break;
				case SpriterObjectType.Entity:
					var newAnim = spriter.Entities[info.EntityId].Animations[info.AnimationId];
					var newTargetTime = info.T * newAnim.Length;
					GetFrameData(newAnim, newTargetTime, deltaTime, info);
					break;
				case SpriterObjectType.Point:
					info.PivotX = 0.0f;
					info.PivotY = 1.0f;
					_frameData.PointData[timeline.Name] = info;
					break;
				case SpriterObjectType.Box:
					_frameData.BoxData[timeline.ObjectId] = info;
					break;
			}
		}

		protected SpriterSpatial[] GetBoneInfos(SpriterMainlineKey key, SpriterAnimation animation, float targetTime, SpriterSpatial parentInfo = null)
		{
			if (key.BoneRefs == null)
			{
				return null;
			}
			var ret = _pool.GetArray<SpriterSpatial>(key.BoneRefs.Length);

			for (var i = 0; i < key.BoneRefs.Length; i += 1)
			{
				var boneRef = key.BoneRefs[i];
				var interpolated = GetBoneInfo(boneRef, animation, targetTime);

				if (boneRef.ParentId >= 0)
				{
					interpolated.ApplyParentTransform(ret[boneRef.ParentId]);
				}
				else
				{
					if (parentInfo != null)
					{
						interpolated.ApplyParentTransform(parentInfo);
					}
				}
				ret[i] = interpolated;
			}

			return ret;
		}

		protected void GetMainlineKeys(SpriterMainlineKey[] keys, float targetTime, out SpriterMainlineKey keyA, out SpriterMainlineKey keyB)
		{
			keyA = keys.GetLastKey(targetTime);
			keyA = keyA ?? keys[keys.Length - 1];
			var nextKey = keyA.Id + 1;
			if (nextKey >= keys.Length)
			{
				nextKey = 0;
			}
			keyB = keys[nextKey];

			if (keyA.Time == keyB.Time)
			{
				nextKey += 1;
				if (nextKey >= keys.Length)
				{
					nextKey = 0;
				}
				keyB = keys[nextKey];
			}
		}

		protected SpriterSpatial GetBoneInfo(SpriterRef spriterRef, SpriterAnimation animation, float targetTime)
		{
			var keys = animation.Timelines[spriterRef.TimelineId].Keys;
			var keyA = keys[spriterRef.KeyId];
			var keyB = keys.GetNextKey(keyA, animation.Looping);

			if (keyB == null)
			{
				return Copy(keyA.BoneInfo);
			}

			var factor = SpriterHelper.GetFactor(keyA, keyB, animation.Length, targetTime);
			return Interpolate(keyA.BoneInfo, keyB.BoneInfo, factor, keyA.Spin);
		}

		protected SpriterObject GetObjectInfo(SpriterRef spriterRef, SpriterAnimation animation, float targetTime)
		{
			var keys = animation.Timelines[spriterRef.TimelineId].Keys;
			var keyA = keys[spriterRef.KeyId];
			var keyB = keys.GetNextKey(keyA, animation.Looping);

			if (keyB == null)
			{
				return Copy(keyA.ObjectInfo);
			}

			var factor = SpriterHelper.GetFactor(keyA, keyB, animation.Length, targetTime);
			return Interpolate(keyA.ObjectInfo, keyB.ObjectInfo, factor, keyA.Spin);
		}

		protected SpriterSpatial Interpolate(SpriterSpatial a, SpriterSpatial b, float f, int spin)
		{
			var ss = _pool.GetObject<SpriterSpatial>();
			ss.Interpolate(a, b, f, spin);
			return ss;
		}

		protected SpriterObject Interpolate(SpriterObject a, SpriterObject b, float f, int spin)
		{
			var so = _pool.GetObject<SpriterObject>();
			so.Interpolate(a, b, f, spin);
			return so;
		}

		protected SpriterSpatial Copy(SpriterSpatial info)
		{
			var copy = _pool.GetObject<SpriterSpatial>();
			copy.FillFrom(info);
			return copy;
		}

		protected SpriterObject Copy(SpriterObject obj)
		{
			var copy = _pool.GetObject<SpriterObject>();
			copy.FillFrom(obj);
			return copy;
		}
	}
}
