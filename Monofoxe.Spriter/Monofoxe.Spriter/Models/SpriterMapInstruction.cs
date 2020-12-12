using System.Xml.Serialization;

namespace Monofoxe.Spriter.Models
{
	public class SpriterMapInstruction
	{
		[XmlAttribute("folder")]
		public int FolderId;

		[XmlAttribute("file")]
		public int FileId;

		[XmlAttribute("target_folder")]
		public int TargetFolderId;

		[XmlAttribute("target_file")]
		public int TargetFileId;

		public SpriterMapInstruction()
		{
			TargetFolderId = -1;
			TargetFileId = -1;
		}
	}
}
