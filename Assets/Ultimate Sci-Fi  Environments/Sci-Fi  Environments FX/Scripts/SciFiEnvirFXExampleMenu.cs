using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SciFiEnvironments.FX
{

	[System.Serializable]
	public class BtnActive
	{
		public GameObject btn;
		public GameObject activeObj;
	}
	public class SciFiEnvirFXExampleMenu : MonoBehaviour
	{
		public List<BtnActive> btnActiveGrp = new List<BtnActive>();
		// Use this for initialization
		void Start()
		{
			int c = btnActiveGrp.Count;
			for (int i = 0; i < c; i++)
			{
				PointerClickListener.Get(btnActiveGrp[i].btn).onClick = OnClickBtn;
			}
		}

		private void OnClickBtn(GameObject go)
		{
			RestActive();

			int c = btnActiveGrp.Count;
			for (int i = 0; i < c; i++)
			{
				if (go.name == btnActiveGrp[i].btn.name)
					btnActiveGrp[i].activeObj.SetActive(true);
			}
		}

		private void RestActive()
		{
			int c = btnActiveGrp.Count;
			for (int i = 0; i < c; i++)
			{
				btnActiveGrp[i].activeObj.SetActive(false);
			}
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}