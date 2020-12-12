using Monofoxe.Spriter;
using Monofoxe.Spriter.Monofoxe;
using Monofoxe.Spriter.Monofoxe.Content;
using Microsoft.Xna.Framework.Content;
using Monofoxe.Engine;
using Monofoxe.Engine.Resources;

namespace Monofoxe.Spriter.Resources
{
	public class SpriterAnimations : ResourceBox<AnimatorTemplate>
	{
		private ContentManager _content;

		public static readonly Config Config = new Config
		{
			MetadataEnabled = true,
			EventsEnabled = true,
			PoolingEnabled = true,
			TagsEnabled = true,
			VarsEnabled = true,
			SoundsEnabled = false
		};

		public SpriterAnimations() : base("Animations")
		{
			_content = new ContentManager(GameMgr.Game.Services);
			_content.RootDirectory = ResourceInfoMgr.ContentDir + "/Graphics/Animations";
		}

		public override void Load()
		{
			if (Loaded)
			{
				return;
			}
			Loaded = true;

			AnimatorTemplate.Init(Config);
			
			AddResource("GreyGuy", new AnimatorTemplate(_content, "GreyGuy/player")); // TODO: Remove.
		}

		public override void Unload()
		{
			if (!Loaded)
			{
				return;
			}
			Loaded = false;
			_content.Unload();
		}

		private void AddResource(string name, AnimationDriver driver = null) =>
			AddResource(name, new AnimatorTemplate(_content, name + "/" + name, driver));
	}
}
