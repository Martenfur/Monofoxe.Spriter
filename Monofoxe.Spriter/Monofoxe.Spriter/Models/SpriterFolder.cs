using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterFolder : SpriterElement
	{
		[XmlElement("file")]
		public SpriterFile[] Files;

		[XmlAttribute("atlas")]
		public int AtlasId;

		public SpriterFolder()
		{
			AtlasId = -1;
		}
	}
}
