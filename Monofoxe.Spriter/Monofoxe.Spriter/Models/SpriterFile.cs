using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterFile : SpriterElement
	{
		[XmlAttribute("type")]
		public SpriterFileType Type;

		[XmlAttribute("pivot_x")]
		public float PivotX;

		[XmlAttribute("pivot_y")]
		public float PivotY;

		[XmlAttribute("width")]
		public int Width;

		[XmlAttribute("height")]
		public int Height;

		public SpriterFile()
		{
			PivotX = 0f;
			PivotY = 1f;
		}
	}
}
