using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterObjectInfo : SpriterElement
	{
		[XmlAttribute("type")]
		public SpriterObjectType ObjectType;

		[XmlAttribute("w")]
		public float Width;

		[XmlAttribute("h")]
		public float Height;

		[XmlAttribute("pivot_x")]
		public float PivotX;

		[XmlAttribute("pivot_y")]
		public float PivotY;

		[XmlArray("var_defs"), XmlArrayItem("i")]
		public SpriterVarDef[] Variables;
	}
}
