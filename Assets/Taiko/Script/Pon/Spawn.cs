using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {

	[SerializeField] RectTransform panel;

	[SerializeField] TweenPos obj;

	[SerializeField] List<float> times;
	private IEnumerator<float> cursor;

	void Start () 
	{
		cursor = times.GetEnumerator();
		cursor.MoveNext();
	}
	
	void Update () 
	{
		int count = 0;
		while( true ){
			if( cursor.Current - obj.time > Time.timeSinceLevelLoad ){
				break;
			}
			if( cursor.MoveNext()== false){
				enabled = false;
				break;
			}

			var newinstance = Instantiate<GameObject>(obj.gameObject);
			newinstance.transform.SetParent( panel.transform, false );
			newinstance.transform.position = obj.transform.parent.position + Vector3.down * count * 30;
			count ++;
		}
	}
}
