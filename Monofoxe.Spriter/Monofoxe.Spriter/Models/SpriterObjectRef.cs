using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterObjectRef : SpriterRef
	{
		[XmlAttribute("z_index")]
		public int ZIndex;
	}
}
