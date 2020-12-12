using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterSoundline : SpriterElement
	{
		[XmlElement("key")]
		public SpriterSoundlineKey[] Keys;
	}
}
