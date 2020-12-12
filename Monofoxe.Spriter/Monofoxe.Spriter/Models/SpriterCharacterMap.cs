using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterCharacterMap : SpriterElement
	{
		[XmlElement("map")]
		public SpriterMapInstruction[] Maps;
	}
}
