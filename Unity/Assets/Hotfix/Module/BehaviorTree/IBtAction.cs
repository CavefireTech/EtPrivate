using ETModel;
using BehaviorDesigner.Runtime.Tasks;

namespace ETHotfix
{
    public interface IBtAction
    {
        HotfixedAction actionObj { get; set; }
        void OnAwake(HotfixedAction bdAction);
        void OnStart();
        TaskStatus OnUpdate();
        void OnLateUpdate();
        void OnFixedUpdate();
        void OnEnd();
        void OnPause(bool paused);
        void OnReset();
    }
}