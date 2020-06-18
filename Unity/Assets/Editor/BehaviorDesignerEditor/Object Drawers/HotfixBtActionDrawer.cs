using System;
using System.CodeDom;
using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.ObjectDrawers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ETHotfix;
using ETModel;

namespace BehaviorDesigner.Editor.ObjectDrawers
{
    [CustomObjectDrawer(typeof(HotfixBtActionAttribute))]
    public class HotfixBtActionDrawer : ObjectDrawer
    {
        Rect buttonRect;
        private string[] fileNameArray;
        private Type[] hotfixBtTypes;
        public override void OnGUI(GUIContent label)
        {
            if (fileNameArray == null || hotfixBtTypes == null)
            {
                Type[] allHotFixTypes = typeof (IBtAction).Assembly.GetTypes();
                //这段的意思是如果这个类是基于可以从这个interface派生，但是不能派生这个interface的话（其实就是排除interface本身），选择这个类
                hotfixBtTypes = (from Type type in allHotFixTypes where typeof(IBtAction).IsAssignableFrom(type) && !type.IsAssignableFrom(typeof(IBtAction)) select type).ToArray();
                fileNameArray = new string[hotfixBtTypes.Length + 2];
                for (int i = 1; i < fileNameArray.Length -1; i++)
                {
                    fileNameArray[i] = hotfixBtTypes[i - 1].ToString();
                }
                fileNameArray[0] = "No Hotfix Bt Action Selected";
                fileNameArray[fileNameArray.Length - 1] = "+ Create New Hotfix Bt Action +";
            }
            int curIndex = 0;
            string scriptName = value as string;
            if (!string.IsNullOrEmpty(scriptName))
            {
                List<string> fileNameList = new List<string>(fileNameArray);
                curIndex = fileNameList.IndexOf(scriptName);
            }
            // var hotfixActionAttribute = (HotfixBtActionAttribute)attribute;
            var hotfixAction = task as HotfixedAction;

            // EditorGUILayout.BeginHorizontal();
            int selectedIndex = EditorGUILayout.Popup("HotfixAction: ", curIndex, fileNameArray);
            
            if (selectedIndex > 0 && selectedIndex < fileNameArray.Length - 1)
            {
                var hotfixScriptName = fileNameArray[selectedIndex];
                if (hotfixScriptName != value as string)
                {
                    value = hotfixScriptName;
                    hotfixAction.curSelectedClass = hotfixScriptName;
                    Type selectedHotfixActionType = hotfixBtTypes[selectedIndex - 1];
                    PropertyInfo[] hotfixActionProperties = selectedHotfixActionType.GetProperties();
                    PropertyInfo[] hotfixSharedVariableProperties = (from PropertyInfo property in hotfixActionProperties where property.PropertyType.IsSubclassOf(typeof(SharedVariable)) select property).ToArray();
                    Debug.Log("hotfixSharedVariableProperties: " + hotfixSharedVariableProperties.Length);
                    hotfixAction.variables = new List<NamedShareVariable>();
                    for (int i = 0; i < hotfixSharedVariableProperties.Length; i++)
                    {
                        var newNamedVariable = new NamedShareVariable();
                        newNamedVariable.name = hotfixSharedVariableProperties[i].Name;
                        newNamedVariable.value = Activator.CreateInstance(hotfixSharedVariableProperties[i].PropertyType) as SharedVariable;
                        hotfixAction.variables.Add(newNamedVariable);
                    }
                }
            }
            else if (selectedIndex == fileNameArray.Length - 1)
            {
                fileNameArray = null;
                var luaTaskStringAttribute = (HotfixBtActionAttribute)attribute;
                string luaTaskPath = Path.Combine(Application.dataPath, luaTaskStringAttribute.luaFolderPath);
                var luaScriptCreater = new LuaCodeGeneratePopup(luaTaskPath);
                PopupWindow.Show(buttonRect, luaScriptCreater);
            }
            else
            {
                value = "";
                hotfixAction.curSelectedClass = "";
            }
            // if (GUILayout.Button("新建Hotfix脚本"))
            // {
            //     fileNameArray = null;
            //     var luaTaskStringAttribute = (HotfixBtActionAttribute)attribute;
            //     string luaTaskPath = Path.Combine(Application.dataPath, luaTaskStringAttribute.luaFolderPath);
            //     var luaScriptCreater = new LuaCodeGeneratePopup(luaTaskPath);
            //     PopupWindow.Show(buttonRect, luaScriptCreater);
            // }
            // EditorGUILayout.EndHorizontal();
            if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
        }

        Dictionary<string, object> InstanceByType = new Dictionary<string, object>();
        public object GetInstance(string typeName)
        {
            if (!InstanceByType.ContainsKey(typeName))
            {
                var type = System.Type.GetType(typeName);
                var obj = Activator.CreateInstance(type);
                InstanceByType.Add(typeName, obj);
            }
            return InstanceByType[typeName];
        }
    }

    public class LuaCodeGeneratePopup : PopupWindowContent
    {
        public LuaCodeGeneratePopup(string saveScriptPath) : base()
        {
            saveScriptLocation = saveScriptPath;
        }
        public string saveScriptLocation;
        const string codeTemplate = @"-- This code template is generated by LuaTaskDrawer.cs

local $LuaTaskName = Class(" + "\"$LuaTaskName\", nil, false)" + @"

-- @param localVar 可以理解为公开的Task变量
-- 使用 variableName = variableType 的格式在$LuaTaskName.localVar中定义变量, 其中variableName为变量名称, variableType为下述的SharedVariable
-- 常用的SharedVariable: SharedBool, SharedString, SharedInt, SharedFloat, SharedVector2, SharedVector3, SharedTransform, SharedGameObject, SharedAnimationCurve, SharedLayerMask
-- 例如: MoveSpd = SharedFloat, canMove = SharedBool, targetObj = SharedGameObject, direction = SharedVector3
-- 使用 self.csObj:SetVariable(variableName) 来获取公开变量的值, 其中string variableName为下方定义的公开变量
-- 使用 self.csObj:SetVariable(variableName, variableValue) 来对公开的参数赋值, 其中其中string variableName为下方定义的公开变量, object variableValue为要赋予变量的值
$LuaTaskName.localVar = {}

--- 成员列表
--- csObj 对应的C#中的行为对象
---------------------------

---@param actionObj userdata 行为CS对象
function $LuaTaskName:OnAwake(actionObj)
    self.csObj = actionObj
end

function $LuaTaskName:OnStart()
end

function $LuaTaskName:OnUpdate()
    --return self.csObj.Success
    --return self.csObj.Running
    --return self.csObj.Failure
end

function $LuaTaskName:OnFixedUpdate()
end

function $LuaTaskName:OnLateUpdate()
end

function $LuaTaskName:OnEnd()
end

return $LuaTaskName
";
        public string createFileName = "";
        public override Vector2 GetWindowSize()
        {
            return new Vector2(150, 120);
        }
        public System.Action OnCreateLuaFileCallback;
        public override void OnGUI(Rect rect)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("请输入 Lua 脚本名称: ");
            GUILayout.Space(2);
            createFileName = EditorGUILayout.TextField(createFileName);
            GUILayout.Space(5);
            if (GUILayout.Button("新建 Lua 脚本", GUILayout.ExpandHeight(true)))
            {
                // selectedFileName = fileNameArray[i];
                // OnFileSelected?.Invoke(fileNameArray[i]);
                if (!string.IsNullOrEmpty(createFileName))
                {
                    GenerateCode();
                    editorWindow.Close();
                }
                GUILayout.Space(5);
                EditorGUILayout.EndVertical();
            }
        }
        public void GenerateCode()
        {
            string code = codeTemplate;
            code = code.Replace("$LuaTaskName", createFileName);
            string fileFullPath = saveScriptLocation + "/Lua_" + createFileName + ".lua.txt";
            StreamWriter writer = new StreamWriter(fileFullPath, false);
            writer.WriteLine(code);
            writer.Close();
            //Re-import the file to update the reference in the editor
            AssetDatabase.ImportAsset(fileFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // EditorTools.GenerateAssetManifest();
            OnCreateLuaFileCallback?.Invoke();
            Debug.Log("脚本生成完成: " + fileFullPath);
        }

        public override void OnOpen()
        {
            createFileName = "";
            // Debug.Log("Popup opened: " + this);
        }

        public override void OnClose()
        {
            // Debug.Log("Popup closed: " + this);
        }
    }
}