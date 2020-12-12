using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterSoundlineKey : SpriterKey
	{
		[XmlElement("object")]
		public SpriterSound SoundObject;
	}
}
