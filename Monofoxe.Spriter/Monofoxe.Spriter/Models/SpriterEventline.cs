using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterEventline : SpriterElement
	{
		[XmlElement("key")]
		public SpriterKey[] Keys;
	}
}
