using System.Collections.Generic;
using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterAnimation : SpriterElement
	{
		public SpriterEntity Entity;

		[XmlAttribute("length")]
		public float Length;

		[XmlAttribute("looping")]
		public bool Looping;

		[XmlArray("mainline"), XmlArrayItem("key")]
		public SpriterMainlineKey[] MainlineKeys;

		[XmlElement("timeline")]
		public SpriterTimeline[] Timelines;

		[XmlElement("eventline")]
		public SpriterEventline[] Eventlines;

		[XmlElement("soundline")]
		public SpriterSoundline[] Soundlines;

		[XmlElement("meta")]
		public SpriterMeta Meta;

		private Dictionary<string, SpriterTimeline> _sprites;
		private Dictionary<string, SpriterTimeline> _bones;
		private bool _initialized = false;

		public SpriterAnimation()
		{
			Looping = true;
		}


		public SpriterTimeline FindBoneTimeline(string name)
		{
			InitDictionaries();
			return _bones[name];
		}


		public bool TryFindBoneTimeline(string name, out SpriterTimeline timeline)
		{
			InitDictionaries();
			return _bones.TryGetValue(name, out timeline);
		}


		public SpriterTimeline FindSpriteTimeline(string name)
		{
			InitDictionaries();
			return _sprites[name];
		}


		public bool TryFindSpriteTimeline(string name, out SpriterTimeline timeline)
		{
			InitDictionaries();
			return _sprites.TryGetValue(name, out timeline);
		}


		private void InitDictionaries()
		{
			if (_initialized)
			{
				return;
			}
			_initialized = true;

			_sprites = new Dictionary<string, SpriterTimeline>();
			_bones = new Dictionary<string, SpriterTimeline>();

			for (var i = 0; i < Timelines.Length; i += 1)
			{
				if (Timelines[i].ObjectType == SpriterObjectType.Bone)
				{
					_bones.Add(Timelines[i].Name, Timelines[i]);
					continue;
				}
				if (Timelines[i].ObjectType == SpriterObjectType.Sprite)
				{
					_sprites.Add(Timelines[i].Name, Timelines[i]);
					continue;
				}
			}
		}
	}

}
