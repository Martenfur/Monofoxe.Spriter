using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterSpatial
	{
		[XmlAttribute("x")]
		public float X;

		[XmlAttribute("y")]
		public float Y;

		[XmlAttribute("angle")]
		public float Angle;

		[XmlAttribute("scale_x")]
		public float ScaleX;

		[XmlAttribute("scale_y")]
		public float ScaleY;

		[XmlAttribute("a")]
		public float Alpha;

		public SpriterSpatial()
		{
			ScaleX = 1;
			ScaleY = 1;
			Alpha = 1;
		}
	}

}
