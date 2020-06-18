using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public class Movement: IBtAction
    {
        public SharedAnimationCurve curve { get; set; }
        public SharedGameObject selfObj { get; set; }
        public SharedVector3 direction { get; set; }
        public SharedFloat speed { get; set; }
        public HotfixedAction actionObj { get; set; }
        
        public void OnAwake(HotfixedAction bdAction)
        {
            actionObj = bdAction;
            selfObj.Value = actionObj.Owner.gameObject;
            Log.Info("Movement action OnAwake : obj is " + actionObj);
        }

        public void OnStart()
        {
            Log.Info("Movement action OnStart");
        }

        public TaskStatus OnUpdate()
        {
            Log.Info("Movement action OnUpdate");
            actionObj.Owner.transform.Translate(new Vector3(1,1,1),Space.World);
            return TaskStatus.Success;
        }

        public void OnLateUpdate()
        {
            Log.Info("Movement action OnLateUpdate");
        }

        public void OnFixedUpdate()
        {
            Log.Info("Movement action OnFixedUpdate");
        }

        public void OnEnd()
        {
            Log.Info("Movement action OnEnd");
        }

        public void OnPause(bool paused)
        {
            Log.Info("Movement action OnPause");
        }

        public void OnReset()
        {
            Log.Info("Movement action OnReset");
        }
    }
}