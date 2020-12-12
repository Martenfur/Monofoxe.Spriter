using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public enum SpriterFileType
	{
		Image,

		[XmlEnum("sound")]
		Sound
	}
}
