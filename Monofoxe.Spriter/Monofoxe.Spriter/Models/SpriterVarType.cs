using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public enum SpriterVarType
	{
		[XmlEnum("string")]
		String,

		[XmlEnum("int")]
		Int,

		[XmlEnum("float")]
		Float
	}
}
