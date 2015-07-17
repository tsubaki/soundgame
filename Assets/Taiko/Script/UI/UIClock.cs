using UnityEngine;
using System.Collections;

public class UIClock : MonoBehaviour {

	[SerializeField, HideInInspector]
	UnityEngine.UI.Text label;

	void Reset()
	{
		label = GetComponent<UnityEngine.UI.Text>();
	}

	void Update () 
	{
		label.text = Time.timeSinceLevelLoad.ToString("0.0");
	}
}
