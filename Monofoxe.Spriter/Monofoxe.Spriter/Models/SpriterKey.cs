using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterKey : SpriterElement
	{
		[XmlAttribute("time")]
		public float Time;

		[XmlAttribute("curve_type")]
		public SpriterCurveType CurveType;

		[XmlAttribute("c1")]
		public float C1;

		[XmlAttribute("c2")]
		public float C2;

		[XmlAttribute("c3")]
		public float C3;

		[XmlAttribute("c4")]
		public float C4;

		public SpriterKey()
		{
			Time = 0;
		}
	}
}
