using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterTaglineKey : SpriterKey
	{
		[XmlElement("tag")]
		public SpriterTag[] Tags;
	}
}
