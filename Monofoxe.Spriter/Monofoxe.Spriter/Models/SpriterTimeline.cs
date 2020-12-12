using System;
using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterTimeline : SpriterElement
	{
		[XmlAttribute("object_type")]
		public SpriterObjectType ObjectType;

		[XmlAttribute("obj")]
		public int ObjectId;

		[XmlElement("key")]
		public SpriterTimelineKey[] Keys;

		[XmlElement("meta")]
		public SpriterMeta Meta;

		public void Modify(Action<SpriterTimelineKey> modifyAction)
		{
			for (var i = 0; i < Keys.Length; i += 1)
			{
				modifyAction.Invoke(Keys[i]);
			}
		}
	}
}
