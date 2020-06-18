using UnityEngine;
using BehaviorDesigner.Runtime.ObjectDrawers;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BDAction = BehaviorDesigner.Runtime.Tasks.Action;
using System;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using System.Collections.Generic;

namespace ETModel
{
    [TaskCategory("Hotfix")]
    [TaskDescription("使用 iLRuntime hotfix 来实现逻辑的行为")]
    public class HotfixedAction: BDAction
    {
        [HotfixBtAction]
        public string hotfixClassName = "";

        public string curSelectedClass;
        public List<NamedShareVariable> variables;
        
        private IType hotfixActionType;
        private object hotfixActionObj;
        private AppDomain appdomain => Game.Hotfix.AppDomain;
        private IMethod onAwakeMethod;
        private IMethod onStartMethod;
        private IMethod onUpdateMethod;
        private IMethod onLateUpdateMethod;
        private IMethod onFixedUpdateMethod;
        private IMethod onEndMethod;
        private IMethod onPauseMethod;
        private IMethod onResetMethod; 
        
        public override void OnAwake()
        {
            base.OnAwake();

            Debug.Log("实例化热更里的类");

            hotfixActionType = appdomain.LoadedTypes[curSelectedClass];
            hotfixActionObj = ((ILType)hotfixActionType).Instantiate();
            
            onAwakeMethod = hotfixActionType.GetMethod("OnAwake", 1);
            onStartMethod = hotfixActionType.GetMethod("OnStart", 0);
            onUpdateMethod =  hotfixActionType.GetMethod("OnUpdate", 0);
            onLateUpdateMethod =  hotfixActionType.GetMethod("OnLateUpdate", 0);
            onFixedUpdateMethod =  hotfixActionType.GetMethod("OnFixedUpdate", 0);
            onEndMethod =  hotfixActionType.GetMethod("OnEnd", 0);
            onPauseMethod =  hotfixActionType.GetMethod("OnPause", 1);
            onResetMethod =  hotfixActionType.GetMethod("OnReset", 0);
            
            for (int i = 0; i < this.variables.Count; i++)
            {
                var setHotfixVarMethod = hotfixActionType.GetMethod("set_" + variables[i].name, 1);
                using (var ctx = appdomain.BeginInvoke(setHotfixVarMethod))
                {
                    ctx.PushObject(hotfixActionObj);
                    ctx.PushObject(variables[i].value);
                    ctx.Invoke();
                    Debug.Log("set value: set_" + variables[i].name);
                }
            }
            
            using (var ctx = appdomain.BeginInvoke(onAwakeMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.PushObject(this);
                ctx.Invoke();
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            
            using (var ctx = appdomain.BeginInvoke(onStartMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
            }
        }

        public override TaskStatus OnUpdate()
        {
            var status = base.OnUpdate();
            
            using (var ctx = appdomain.BeginInvoke(onUpdateMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
                status = ctx.ReadObject<TaskStatus>();
            }

            return status;
        }

        public override void OnLateUpdate()
        {
            base.OnLateUpdate();
            using (var ctx = appdomain.BeginInvoke(onLateUpdateMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
            }
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            using (var ctx = appdomain.BeginInvoke(onFixedUpdateMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            using (var ctx = appdomain.BeginInvoke(onEndMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
            }
        }

        public override void OnPause(bool paused)
        {
            base.OnPause(paused);
            using (var ctx = appdomain.BeginInvoke(onPauseMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.PushBool(paused);
                ctx.Invoke();
            }
        }

        public override void OnReset()
        {
            base.OnReset();
            using (var ctx = appdomain.BeginInvoke(onResetMethod))
            {
                ctx.PushObject(hotfixActionObj);
                ctx.Invoke();
            }
        }
    }
    
    [Serializable]
    public class NamedShareVariable
    {
        public string name;
        public SharedVariable value;
    }
}