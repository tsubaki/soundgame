using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TweenPos : MonoBehaviour {

	[SerializeField, Range(1, 5)]
	public float time = 3;

	[SerializeField]
	Transform targetPos;

	[SerializeField, HideInInspector]
	float speed;

	[SerializeField, HideInInspector]
	Text timeLabel;

	void Reset()
	{
		timeLabel = GetComponentInChildren<Text>();
	}

	void Start()
	{
		speed = (transform.position.x - targetPos.position.x) / time;

		timeLabel.text = (Time.timeSinceLevelLoad + time).ToString("0.0");
	}

	void Update () 
	{
		transform.position -= Vector3.right * speed * Time.deltaTime;
	}
}