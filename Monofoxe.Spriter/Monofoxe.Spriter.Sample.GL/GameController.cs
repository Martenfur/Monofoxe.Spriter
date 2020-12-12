using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monofoxe.Engine;
using Monofoxe.Engine.Cameras;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.EC;
using Monofoxe.Engine.Resources;
using Monofoxe.Engine.SceneSystem;
using Monofoxe.Spriter.Models;
using Monofoxe.Spriter.Monofoxe;
using Monofoxe.Spriter.Monofoxe.Content;
using System.Diagnostics;
using System.Linq;

namespace Monofoxe.Spriter
{
	public class GameController : Entity
	{
		public Camera2D Camera = new Camera2D(800, 600);
		
		AnimatorTemplate _template;
		FoxeAnimator _customizedAnimator;
		FoxeAnimator _basicAnimator;

		string[] _animationNames;
		int _currentAnimationId = 0;
		float _headAngle = 180;

		public GameController() : base(SceneMgr.GetScene("default")["default"])
		{
			GameMgr.MaxGameSpeed = 60;
			GameMgr.MinGameSpeed = 60; // Fixing framerate on 60.

			Camera.BackgroundColor = Color.White;

			GameMgr.WindowManager.CanvasSize = Camera.Size;
			GameMgr.WindowManager.Window.AllowUserResizing = false;
			GameMgr.WindowManager.ApplyChanges();
			GameMgr.WindowManager.CenterWindow();
			GameMgr.WindowManager.CanvasMode = CanvasMode.Fill;

			GraphicsMgr.VertexBatch.SamplerState = SamplerState.PointClamp;

			Text.CurrentFont = ResourceHub.GetResource<IFont>("Fonts", "Arial");

			_template = ResourceHub.GetResource<AnimatorTemplate>("Animations", "GreyGuy");
			
			
			InitBasicAnimator();
			InitCustomizedAnimator();
		}


		private void InitBasicAnimator()
		{
			_basicAnimator = _template.MakeAnimator();

			_animationNames = _basicAnimator.GetAnimations().ToArray();

			_basicAnimator.Position = new Vector2(500, Camera.Size.Y / 2f);
			_basicAnimator.Play(_animationNames[0]);
		}
		
		
		private void InitCustomizedAnimator()
		{
			_customizedAnimator = _template.MakeAnimator();

			_customizedAnimator.Position = new Vector2(200, Camera.Size.Y / 2f);
			_customizedAnimator.EventTriggered += x => Debug.WriteLine("Event Happened: " + x);

			_customizedAnimator.Play(_animationNames[0]);

			_customizedAnimator.AnimationModifier = TestMod;
		}


		private void TestMod(SpriterAnimation first)
		{
			// Rotates the entire bone.
			if (first.TryFindBoneTimeline("front_arm", out var tl))
			{
				tl.Modify((k) => k.BoneInfo.Angle = _headAngle);
			}
			// Rotates sprite directly.
			if (first.TryFindSpriteTimeline("p_head_idle", out var tl1))
			{
				tl1.Modify((k) => k.ObjectInfo.Angle = -_headAngle);
			}
		}


		public override void Update()
		{
			base.Update();

			_customizedAnimator.Update();
			_basicAnimator.Update();


			// Rotates the head.
			// NOTE: In the sample, you'll see that both grey guys rotate at the same time, even though basic animator
			// is not connected to bne modifiers in any way. It is because ALL instances share the same skeleton,
			// and bones are being manipulated by modifying it. Be aware of this.
			_headAngle += (Input.CheckButton(Buttons.Up).ToInt() - Input.CheckButton(Buttons.Down).ToInt());

			// Cycles through animations.
			if (Input.CheckButtonPress(Buttons.Space))
			{
				_currentAnimationId += 1;
				if (_currentAnimationId >= _animationNames.Length)
				{
					_currentAnimationId = 0;
				}
				
				_customizedAnimator.Transition(_animationNames[_currentAnimationId], 0.25f);
			}
		}


		public override void Draw()
		{
			base.Draw();
			_customizedAnimator.Draw();
			_basicAnimator.Draw();

			Text.HorAlign = TextAlign.Center;
			Text.VerAlign = TextAlign.Center;
			GraphicsMgr.CurrentColor = Color.Black;
			Text.Draw("Space to change animation, Up/Down to rotate.", new Vector2(Camera.Size.X / 2, 500).FloorV());
		}

	}
}