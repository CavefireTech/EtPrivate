using System;
using System.Net;
using System.Reflection.Emit;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiLoginComponentSystem : AwakeSystem<UILoginComponent>
	{
		public override void Awake(UILoginComponent self)
		{
			self.Awake();
		}
	}
	
	public class UILoginComponent: Component
	{
		private GameObject account;
		private GameObject loginBtn;
		private GameObject bgImage;
		
		public void Awake()
		{
			ReferenceCollector rc = this.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			loginBtn = rc.Get<GameObject>(nameof(this.loginBtn));
			loginBtn.GetComponent<Button>().onClick.Add(OnLogin);
			this.account = rc.Get<GameObject>(nameof(this.account));
			this.bgImage = rc.Get<GameObject>(nameof(this.bgImage));
			//Log.Debug("account variable name: " +  nameof(this.account));
		}

		public void OnLogin()
		{
			bgImage.GetComponent<Image>().color = Color.blue;
			LoginHelper.OnLoginAsync(this.account.GetComponent<InputField>().text).Coroutine();
		}
	}
}
