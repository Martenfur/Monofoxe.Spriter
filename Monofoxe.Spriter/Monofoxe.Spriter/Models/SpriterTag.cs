using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterTag : SpriterElement
	{
		[XmlAttribute("t")]
		public int TagId;
	}
}
