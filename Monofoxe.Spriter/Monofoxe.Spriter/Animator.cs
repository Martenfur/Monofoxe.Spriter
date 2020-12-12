// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using Monofoxe.Spriter.Providers;
using Microsoft.Xna.Framework.Audio;
using Monofoxe.Engine.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Monofoxe.Spriter
{
	public abstract class Animator<TSprite, TSound>
	{
		/// <summary>
		/// Occurs when the animation finishes playing or loops.
		/// </summary>
		public event Action<string> AnimationFinished = s => { };

		/// <summary>
		/// Occurs when an animation events gets triggered.
		/// </summary>
		public event Action<string> EventTriggered = s => { };

		/// <summary>
		/// The animated Entity.
		/// </summary>
		public SpriterEntity Entity { get; protected set; }

		/// <summary>
		/// The current animation.
		/// </summary>
		public SpriterAnimation CurrentAnimation { get; protected set; }

		/// <summary>
		/// The animation transitioned to or blended with the current animation.
		/// </summary>
		public SpriterAnimation NextAnimation { get; protected set; }

		/// <summary>
		/// The name of the current animation.
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Playback speed. Defaults to 1.0f. Negative values reverse the animation.<para />
		/// For example:<para />
		/// 0.5f corresponds to 50% of the default speed<para />
		/// 2.0f corresponds to 200% of the default speed<para />
		/// </summary>
		public float Speed = 1;

		/// <summary>
		/// The legth of the current animation in seconds.
		/// </summary>
		public float Length => _length / 1000f;
		private float _length;

		/// <summary>
		/// The current time in seconds.
		/// </summary>
		public float Time
		{ 
			get => _time / 1000f;
			set => _time = value * 1000;
		}
		private float _time;

		/// <summary>
		/// Allow external class to check if an animation exists for a given name.
		/// </summary>
		public bool HasAnimation(string name) => 
			_animations.ContainsKey(name);

		/// <summary>
		/// The current progress. Ranges from 0.0f - 1.0f.
		/// </summary>
		public float Progress
		{
			get => _time / _length; 
			set => _time = value * _length; 
		}

		public bool Running { get; private set; } = false;

		public bool Transitioning => NextAnimation != null;

		/// <summary>
		/// The provider of the animation data.
		/// </summary>
		private FrameDataProvider _dataProvider;

		/// <summary>
		/// The provider of sprite assets.
		/// </summary>
		public AssetProvider<Sprite> SpriteProvider;

		/// <summary>
		/// The provider of sound assets.
		/// </summary>
		public AssetProvider<SoundEffect> SoundProvider;

		/// <summary>
		/// The latest FrameData.
		/// </summary>
		public FrameData FrameData { get; protected set; }

		protected Dictionary<string, SpriterAnimation> _animations;

		private float _totalTransitionTime;
		private float _transitionTime;
		private float _transitionFactor;


		/// <summary>
		/// A place to modify bones and sprites. 
		/// NOTE: Your delegate will be caled multiple times per frame, 
		/// DO NOT put any actual logic inside. Prepare a model beforehand.
		/// </summary>
		public AnimationModifierDelegate AnimationModifier 
		{ 
			get => _dataProvider.AnimationModifier;
			set => _dataProvider.AnimationModifier = value;
		}

		protected Animator(SpriterEntity entity, ResourceProviderFactory providerFactory = null)
		{
			Entity = entity;
			_animations = entity.Animations.ToDictionary(a => a.Name, a => a);
			Speed = 1.0f;

			if (providerFactory != null)
			{
				_dataProvider = new FrameDataProvider();//providerFactory.GetDataProvider(entity);
				SpriteProvider = providerFactory.GetSpriteProvider(entity);
				SoundProvider = providerFactory.GetSoundProvider(entity);
			}
			else
			{
				_dataProvider = new FrameDataProvider();
				SpriteProvider = new AssetProvider<Sprite>();
				SoundProvider = new AssetProvider<SoundEffect>();
			}
		}

		/// <summary>
		/// Returns a list of all the animations for the entity
		/// </summary>
		public IEnumerable<string> GetAnimations() => 
			_animations.Keys;

		/// <summary>
		/// Plays the animation with the given name. Playback starts from the beginning.
		/// </summary>
		public virtual void Play(string name, float startTime = 0)
		{
			SpriterAnimation animation = _animations[name];
			Play(animation, startTime);
		}

		/// <summary>
		/// Plays the given animation. Playback starts from the beginning.
		/// </summary>
		public virtual void Play(SpriterAnimation animation, float startTime)
		{
			_time = startTime * 1000;

			CurrentAnimation = animation;
			Name = animation.Name;

			NextAnimation = null;
			_length = CurrentAnimation.Length;

			Running = true;
		}

		private float _transitionStartTime;

		/// <summary>
		/// Transitions to given animation doing a progressive blend in the given time.
		/// <remarks>Animation blending works only for animations with identical hierarchies.</remarks>
		/// </summary>
		public virtual void Transition(string name, float totalTransitionTime, float startTime = 0)
		{
			if (CurrentAnimation.Name == name)
			{
				return;
			}
			_totalTransitionTime = totalTransitionTime * 1000f; // TODO: Calcualte min duration between current, next and transition time.
			_transitionTime = 0.0f;
			_transitionFactor = 0.0f;

			_transitionStartTime = startTime * 1000;
			NextAnimation = _animations[name];

			Running = true;
		}

		/// <summary>
		/// Blends two animations with the given weight factor. Factor ranges from 0.0f - 1.0f.<para />
		/// Animation blending works only for animations with identical hierarchies.<para />
		/// For example:<para />
		/// factor == 0.0f corresponds to 100% of the first animation and 0% of the second<para />
		/// factor == 0.25f corresponds to 75% of the first animation and 25% of the second<para />
		/// factor == 0.5f corresponds to 50% of each animation<para />
		/// </summary>
		public virtual void Blend(string first, string second, float factor)
		{
			Play(first);
			NextAnimation = _animations[second];
			_totalTransitionTime = 0;
			_transitionFactor = factor;
		}

		/// <summary>
		/// Advances the animation for the deltaTime increment.
		/// </summary>
		public virtual void Update(float deltaTime)
		{
			if (CurrentAnimation == null) 
			{ 
				Play(_animations.Keys.First());
			}

			var initialTime = _time;
			var elapsed = deltaTime * Speed;

			if (NextAnimation != null && _totalTransitionTime != 0.0f)
			{
				elapsed += elapsed * _transitionFactor * CurrentAnimation.Length / NextAnimation.Length;

				_transitionTime += Math.Abs(elapsed);
				_transitionFactor = _transitionTime / _totalTransitionTime;
				if (_transitionTime >= _totalTransitionTime)
				{
					Play(NextAnimation.Name);
					_time = _transitionStartTime;

					NextAnimation = null;
				}
			}
			else
			{
				_time += elapsed;
			}

			if (_time < 0.0f)
			{
				if (CurrentAnimation.Looping)
				{
					_time += _length;
				}
				else
				{
					_time = 0.0f;
					Running = false;
				}
				if (_time != initialTime)
				{
					AnimationFinished(Name);
				}
			}
			else
			{
				if (_time >= _length)
				{
					if (CurrentAnimation.Looping)
					{
						_time -= _length;
					}
					else
					{
						_time = _length;
						Running = false;
					}
					if (_time != initialTime)
					{
						AnimationFinished(Name);
					}
				}
			}

			Animate(elapsed);
		}

		/// <summary>
		/// Gets the transform information for all object types and calls the relevant apply method for each one.
		/// </summary>
		protected virtual void Animate(float deltaTime)
		{
			FrameData = _dataProvider.GetFrameData(_time, deltaTime, _transitionFactor, CurrentAnimation, NextAnimation);

			for (var i = 0; i < FrameData.SpriteData.Count; i += 1)
			{
				var info = FrameData.SpriteData[i];
				
				var sprite = SpriteProvider.Get(info.FolderId, info.FileId);
				if (sprite != null)
				{
					ApplySpriteTransform(sprite, info);
				}
			}

			for (var i = 0; i < FrameData.Sounds.Count; i += 1)
			{
				var info = FrameData.Sounds[i];
				var sound = SoundProvider.Get(info.FolderId, info.FileId);
				if (sound != null)
				{
					PlaySound(sound, info);
				}
			}

			var pointE = FrameData.PointData.GetEnumerator();
			while (pointE.MoveNext())
			{
				var e = pointE.Current;
				ApplyPointTransform(e.Key, e.Value);
			}

			var boxE = FrameData.BoxData.GetEnumerator();
			while (boxE.MoveNext())
			{
				var e = boxE.Current;
				ApplyBoxTransform(Entity.ObjectInfos[e.Key], e.Value);
			}

			for (var i = 0; i < FrameData.Events.Count; i += 1)
			{
				DispatchEvent(FrameData.Events[i]);
			}
		}

		/// <summary>
		/// Applies the transform to the concrete sprite isntance.
		/// </summary>
		protected virtual void ApplySpriteTransform(Sprite sprite, SpriterObject info)
		{
		}

		/// <summary>
		/// Plays the concrete sound isntance.
		/// </summary>
		protected virtual void PlaySound(SoundEffect sound, SpriterSound info)
		{
		}

		/// <summary>
		/// Applies the transforms for the point with the given name.
		/// </summary>
		protected virtual void ApplyPointTransform(string name, SpriterObject info)
		{
		}

		/// <summary>
		/// Applies the transform for the given box.
		/// </summary>
		protected virtual void ApplyBoxTransform(SpriterObjectInfo objInfo, SpriterObject info)
		{
		}

		/// <summary>
		/// Dispatches event when triggered in animation.
		/// </summary>
		protected virtual void DispatchEvent(string eventName) =>
			EventTriggered(eventName);
	}
}
