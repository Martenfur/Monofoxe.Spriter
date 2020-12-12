using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public enum SpriterObjectType
	{
		[XmlEnum("sprite")]
		Sprite,

		[XmlEnum("bone")]
		Bone,

		[XmlEnum("box")]
		Box,

		[XmlEnum("point")]
		Point,

		[XmlEnum("sound")]
		Sound,

		[XmlEnum("entity")]
		Entity,

		[XmlEnum("variable")]
		Variable
	}
}
