using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterVarline : SpriterElement
	{
		[XmlAttribute("def")]
		public int Def;

		[XmlElement("key")]
		public SpriterVarlineKey[] Keys;
	}
}
