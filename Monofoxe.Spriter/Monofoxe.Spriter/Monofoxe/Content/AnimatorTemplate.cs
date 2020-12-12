// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Monofoxe.Spriter.Models;
using Monofoxe.Spriter.Providers;
using Microsoft.Xna.Framework.Content;
using Monofoxe.Engine.Drawing;
using Monofoxe.Engine.Resources;
using System.IO;

namespace Monofoxe.Spriter.Monofoxe.Content
{
	public class AnimatorTemplate
	{
		private static ResourceProviderFactory _resourceProvider;

		public static void Init(Config config)
		{
			if (_resourceProvider != null)
			{ 
				return;
			}
			_resourceProvider = new ResourceProviderFactory(config);
		}

		public SpriterData Spriter { get; private set; }

		private readonly ContentManager _content;
		private readonly string _scmlPath;
		//private readonly string _rootPath;

		private readonly AnimationDriver _driver;

		public AnimatorTemplate(ContentManager content, string scmlPath, AnimationDriver driver = null)
		{
			_content = content;
			_scmlPath = scmlPath;
			//_rootPath = scmlPath.Substring(0, scmlPath.LastIndexOf("/"));
			_driver = driver;
			Fill();
		}

		public FoxeAnimator MakeAnimator() =>
			new FoxeAnimator(Spriter.Entities[0], _resourceProvider, _driver);

		// If animation has more than one entity, add another method here.

		private void Fill()
		{
			if (Spriter == null)
			{
				Load();
			}

			foreach (var folder in Spriter.Folders)
			{
				AddRegularFolder(folder, _resourceProvider);
			}
		}

		private void AddRegularFolder(SpriterFolder folder, ResourceProviderFactory factory)
		{
			foreach (var file in folder.Files)
			{
				//string path = FormatPath(file.Name);

				if (file.Type == SpriterFileType.Sound)
				{
					// TODO: Add sound support.
					//SoundEffect sound = LoadContent<SoundEffect>(path);
					//factory.SetSound(Spriter, folder, file, sound);
				}
				else
				{
					// TODO: This may result in problems with same names. Is it ok?
					var formattedName = Path.GetFileNameWithoutExtension(file.Name);
					var sprite = ResourceHub.GetResource<Sprite>("AnimationsSprites", formattedName);
					factory.SetSprite(Spriter, folder, file, sprite);
				}

			}
		}

		private void Load() =>
			Spriter = LoadContent<SpriterData>(_scmlPath);

		//private string FormatPath(string fileName) => 
		//	string.Format("{0}/{1}", _rootPath, fileName);

		private T LoadContent<T>(string path)
		{
			var index = path.LastIndexOf(".");
			if (index >= 0)
			{
				path = path.Substring(0, index);
			}

			T asset = default;
			try
			{
				asset = _content.Load<T>(path);
			}
			catch
			{
				System.Diagnostics.Debug.WriteLine("Missing Asset: " + path);
			}

			return asset;
		}
	}
}

