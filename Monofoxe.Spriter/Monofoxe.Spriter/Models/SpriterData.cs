using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	[XmlRoot("spriter_data")]
	public class SpriterData
	{
		[XmlElement("folder")]
		public SpriterFolder[] Folders;

		[XmlElement("entity")]
		public SpriterEntity[] Entities;

		[XmlArray("tag_list"), XmlArrayItem("i")]
		public SpriterElement[] Tags;

		[XmlArray("atlas"), XmlArrayItem("i")]
		public SpriterElement[] Atlases;
	}
}
