using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterHandle : MonoBehaviour
{
	[SerializeField]
	private float mScale = 1.0f;
	
	private void LateUpdate()
	{
		float distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
		transform.localScale = Vector3.one * distanceToCamera * mScale;
	}
}
