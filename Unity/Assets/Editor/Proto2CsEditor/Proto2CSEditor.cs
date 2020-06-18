using System.Diagnostics;
using System.IO;
using ETModel;
using UnityEditor;

namespace ETEditor
{
	internal class OpcodeInfo
	{
		public string Name;
		public int Opcode;
	}

	public class Proto2CSEditor: EditorWindow
	{
		[MenuItem("Tools/Proto2CS")]
		public static void AllProto2CS()
		{
			//Process process = ProcessHelper.Run("dotnet", "Proto2CS.dll", "../Proto/", true);
			Process process = ProcessHelper.Run("/usr/local/share/dotnet/dotnet", "Proto2CS.dll", Path.GetFullPath("../Proto/"), true);
			Log.Info(process.StandardOutput.ReadToEnd());
			AssetDatabase.Refresh();
		}
	}
}
