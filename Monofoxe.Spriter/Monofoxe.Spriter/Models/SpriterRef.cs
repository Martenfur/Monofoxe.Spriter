using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterRef : SpriterElement
	{
		[XmlAttribute("parent")]
		public int ParentId;

		[XmlAttribute("timeline")]
		public int TimelineId;

		[XmlAttribute("key")]
		public int KeyId;

		public SpriterRef()
		{
			ParentId = -1;
		}
	}
}
