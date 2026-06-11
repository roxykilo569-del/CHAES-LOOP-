using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SciFiEnvironments.FX
{

	public class ExampleCtrl : MonoBehaviour
	{

		public float displayTime = 8f;
		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		void OnEnable()
		{
			StartCoroutine(IEDisplay());
		}

		IEnumerator IEDisplay()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				for (int j = 0; j < transform.childCount; j++)
				{
					transform.GetChild(j).gameObject.SetActive(false);
				}
				transform.GetChild(i).gameObject.SetActive(true);
				yield return new WaitForSeconds(displayTime);
			}
		}
	}
}