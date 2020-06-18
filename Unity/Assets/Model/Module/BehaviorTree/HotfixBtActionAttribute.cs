using BehaviorDesigner.Runtime.Tasks;
namespace BehaviorDesigner.Runtime.ObjectDrawers
{
    public class HotfixBtActionAttribute : ObjectDrawerAttribute
    {
        public string luaFolderPath;
        public HotfixBtActionAttribute() : base()
        {
            this.luaFolderPath = "HotfixResources/Lua/LuaBehavior";
        }
        // Start is called before the first frame update
        public HotfixBtActionAttribute(string luaFolderPath) : base()
        {
            this.luaFolderPath = luaFolderPath;
        }
    }
}
