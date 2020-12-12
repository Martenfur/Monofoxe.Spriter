using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterMeta
	{
		[XmlElement("varline")]
		public SpriterVarline[] Varlines;

		[XmlElement("tagline")]
		public SpriterTagline Tagline;
	}
}
