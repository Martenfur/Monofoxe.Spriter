// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using Monofoxe.Spriter.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Monofoxe.Engine;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Utils;
using System;
using System.Collections.Generic;

namespace Monofoxe.Spriter.Monofoxe
{
	/// <summary>
	/// Fuck you, lookadrawa.
	/// </summary>
	public class FoxeAnimator : Animator<Sprite, SoundEffect>
	{
		/// <summary>
		/// Scale factor of the animator. Negative values flip the image.
		/// </summary>
		public virtual Vector2 Scale
		{
			get => _scale;
			set
			{
				_scale = value;
				_scaleAbs = new Vector2(Math.Abs(value.X), Math.Abs(value.Y));
			}
		}
		private Vector2 _scale;
		private Vector2 _scaleAbs;

		/// <summary>
		/// Rotation in radians.
		/// </summary>
		public virtual float Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				_rotationSin = (float)Math.Sin(Rotation);
				_rotationCos = (float)Math.Cos(Rotation);
			}
		}
		private float _rotation;
		private float _rotationSin;
		private float _rotationCos;

		/// <summary>
		/// Position in pixels.
		/// </summary>
		public Vector2 Position;

		/// <summary>
		/// The drawing depth. Should be in the [0,1] interval.
		/// </summary>
		public float Depth = _defaultDepth;

		/// <summary>
		/// The depth distance between different sprites of the same animation.
		/// </summary>
		public float DeltaDepth = _defaultDeltaDepth;

		/// <summary>
		/// The color used to render all the sprites.
		/// </summary>
		public Color Color = Color.White;

		public bool ZTilted = false;

		/// <summary>
		/// Lowest bound is needed to calculate z tilt.
		/// </summary>
		private float _lowestBound;

		public TimeKeeper TimeKeeper = TimeKeeper.Global;

		protected static readonly Stack<SpriteDrawInfo> _drawInfoPool = new Stack<SpriteDrawInfo>();
		protected List<SpriteDrawInfo> _drawInfos = new List<SpriteDrawInfo>();

		private static readonly float _defaultDepth = 0;
		private static readonly float _defaultDeltaDepth = 0.0f;

		public readonly AnimationDriver Driver;

		internal FoxeAnimator(
			SpriterEntity entity,
			ResourceProviderFactory providerFactory = null,
			AnimationDriver driver = null
		) : base(entity, providerFactory)
		{
			Scale = Vector2.One;
			Rotation = 0;
			Driver = driver;
			Driver?.Bind(this);
		}

		public void Update()
		{
			Driver?.Update(this);

			for (var i = 0; i < _drawInfos.Count; i += 1)
			{
				_drawInfoPool.Push(_drawInfos[i]);
			}
			_drawInfos.Clear();

			_lowestBound = Position.Y;

			Update((float)TimeKeeper.Time() * 1000);

			if (!ZTilted)
			{
				return;
			}
			for (var i = 0; i < _drawInfos.Count; i += 1)
			{
				_drawInfos[i].TiltDepth -= _lowestBound * Vector4.One;
			}
		}

		/// <summary>
		/// Draws the animation with the given SpriteBatch.
		/// </summary>
		public virtual void Draw()
		{
			// TODO: Implement proper sprite rounding.
			// with z-tilt support.
			for (int i = 0; i < _drawInfos.Count; i += 1)
			{
				SpriteDrawInfo di = _drawInfos[i];
				Sprite sprite = di.Drawable;

				sprite.Draw(di.Position, 0, di.Pivot, di.Scale, Angle.FromRadians(di.Rotation), di.Color, Vector4.One * di.Depth + di.TiltDepth);
			}
		}

		protected override void ApplySpriteTransform(Sprite drawable, SpriterObject info)
		{
			float posX, posY, rotation;
			GetPositionAndRotation(info, out posX, out posY, out rotation);

			SpriteDrawInfo di = _drawInfoPool.Count > 0 ? _drawInfoPool.Pop() : new SpriteDrawInfo();

			di.Drawable = drawable;
			di.Pivot = new Vector2(info.PivotX * drawable.Width, (1 - info.PivotY) * drawable.Height);//.RoundV();
			di.Position = new Vector2(posX, posY);//.RoundV();
			di.Scale = new Vector2(info.ScaleX, info.ScaleY) * Scale;
			di.Rotation = rotation;
			di.Color = Color * info.Alpha;
			di.Depth = Depth + DeltaDepth * _drawInfos.Count;

			_drawInfos.Add(di);

			if (!ZTilted)
			{
				di.TiltDepth = Vector4.Zero;
				return;
			}

			// Z tilting.
			var dx = -di.Pivot.X * di.Scale.X;
			var dy = -di.Pivot.Y * di.Scale.Y;

			var y = di.Position.Y;

			var sin = (float)Math.Sin(di.Rotation);
			var cos = (float)Math.Cos(di.Rotation);

			var w = di.Drawable.Width * di.Scale.X;
			var h = di.Drawable.Height * di.Scale.Y;

			// Getting each y for the quad.

			di.TiltDepth.X = y + dx * sin + dy * cos;
			di.TiltDepth.Y = y + (dx + w) * sin + dy * cos;
			di.TiltDepth.Z = y + dx * sin + (dy + h) * cos;
			di.TiltDepth.W = y + (dx + w) * sin + (dy + h) * cos;

			di.TiltDepth = CorrectForNegativeScale(di.TiltDepth, di.Scale);

			// Later an offset by _lowestBound will be performed, since we don't know it yet.
			// TODO: optimize

			// Z tilting.


			var box = GetBoundingBox(info, drawable.Width, drawable.Height);
			if (box.Point1.Y > _lowestBound)
			{
				_lowestBound = box.Point1.Y;
			}
			if (box.Point2.Y > _lowestBound)
			{
				_lowestBound = box.Point2.Y;
			}
			if (box.Point3.Y > _lowestBound)
			{
				_lowestBound = box.Point3.Y;
			}
			if (box.Point4.Y > _lowestBound)
			{
				_lowestBound = box.Point4.Y;
			}
		}

		protected override void PlaySound(SoundEffect sound, SpriterSound info)
		{
			sound.Play(info.Volume, 0.0f, info.Panning);
		}

		public Box GetBoundingBox(SpriterObject info, float width, float height)
		{
			float posX, posY, rotation;
			GetPositionAndRotation(info, out posX, out posY, out rotation);

			float w = width * info.ScaleX * Scale.X;
			float h = height * info.ScaleY * Scale.Y;

			float rs = (float)Math.Sin(rotation);
			float rc = (float)Math.Cos(rotation);

			Vector2 originDelta = Rotate(new Vector2(-info.PivotX * w, -(1 - info.PivotY) * h), rs, rc);

			Box cb = new Box();
			Vector2 horizontal = Rotate(new Vector2(w, 0), rs, rc);
			cb.Point1 = new Vector2(posX, posY) + originDelta;
			cb.Point2 = cb.Point1 + horizontal;
			cb.Point4 = cb.Point1 + Rotate(new Vector2(0, h), rs, rc);
			cb.Point3 = cb.Point4 + horizontal;

			return cb;
		}

		public Vector2 GetPosition(SpriterObject info)
		{
			float posX, posY, rotation;
			GetPositionAndRotation(info, out posX, out posY, out rotation);
			return new Vector2(posX, posY);
		}

		private void GetPositionAndRotation(SpriterObject info, out float posX, out float posY, out float rotation)
		{
			float px = info.X;
			float py = -info.Y;
			rotation = MathHelper.ToRadians(-info.Angle);

			if (Scale.X < 0)
			{
				px = -px;
				rotation = -rotation;
			}

			if (Scale.Y < 0)
			{
				py = -py;
				rotation = -rotation;
			}

			px *= _scaleAbs.X;
			py *= _scaleAbs.Y;

			rotation += Rotation;

			posX = px * _rotationCos - py * _rotationSin + Position.X;
			posY = px * _rotationSin + py * _rotationCos + Position.Y;
		}

		private static Vector2 Rotate(Vector2 v, float s, float c)
		{
			return new Vector2(v.X * c - v.Y * s, v.X * s + v.Y * c);
		}


		public static Vector4 CorrectForNegativeScale(Vector4 tilt, Vector2 scale)
		{
			// Due to how VertexBatch/SpriteBatch works, negative scaling doesn't actually flip the quads.
			// Instead, it offsets them and flips the texture coordinates. 
			// This was done to preserve the culling, but introduces problems when tilting.
			// This piece of shit corrects for the scaling errors.
			// In this part of the comment, I would call out VertexBatch developer,
			// Except VertexBatch is written by me. 
			// Good job, retard.
			var buffer = new Vector4();
			if (scale.X < 0)
			{
				buffer.X = tilt.Y;
				buffer.Y = tilt.X;
				buffer.Z = tilt.W;
				buffer.W = tilt.Z;

				tilt = buffer;
			}
			if (scale.Y < 0)
			{
				buffer.X = tilt.Z;
				buffer.Y = tilt.W;
				buffer.Z = tilt.X;
				buffer.W = tilt.Y;

				tilt = buffer;
			}

			return tilt;
		}
	}
}
