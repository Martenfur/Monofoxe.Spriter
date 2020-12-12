using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterTimelineKey : SpriterKey
	{
		[XmlAttribute("spin")]
		public int Spin;

		[XmlElement("bone", typeof(SpriterSpatial))]
		public SpriterSpatial BoneInfo;

		[XmlElement("object", typeof(SpriterObject))]
		public SpriterObject ObjectInfo;

		public SpriterTimelineKey()
		{
			Spin = 1;
		}
	}
}
