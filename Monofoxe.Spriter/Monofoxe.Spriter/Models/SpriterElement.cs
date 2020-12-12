using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterElement
	{
		[XmlAttribute("id")]
		public int Id;

		[XmlAttribute("name")]
		public string Name;
	}
}
