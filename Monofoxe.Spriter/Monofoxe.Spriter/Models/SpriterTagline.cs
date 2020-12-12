using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterTagline
	{
		[XmlElement("key")]
		public SpriterTaglineKey[] Keys;
	}
}
