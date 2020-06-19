using BehaviorDesigner.Runtime.Tasks;
namespace BehaviorDesigner.Runtime.ObjectDrawers
{
    public class HotfixBtActionAttribute : ObjectDrawerAttribute
    {
        public string hotfixBtActionPath;
        public HotfixBtActionAttribute() : base()
        {
            this.hotfixBtActionPath = "Hotfix/Module/BehaviorTree";
        }
        // Start is called before the first frame update
        public HotfixBtActionAttribute(string hotfixBtActionPath) : base()
        {
            this.hotfixBtActionPath = hotfixBtActionPath;
        }
    }
}
