using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterSound : SpriterElement
	{
		[XmlAttribute("folder")]
		public int FolderId;

		[XmlAttribute("file")]
		public int FileId;

		[XmlAttribute("trigger")]
		public bool Trigger;

		[XmlAttribute("panning")]
		public float Panning;

		[XmlAttribute("volume")]
		public float Volume;

		public SpriterSound()
		{
			Trigger = true;
			Volume = 1.0f;
		}
	}
}
