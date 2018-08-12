using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoss : MonoBehaviour {

	public BossParametersAsset parametersAsset;
	void Update () {

		float speed = parametersAsset.bossParameters.moveSpeed;
		float t = Mathf.Repeat(Time.time * speed, 1f);
		float y = Mathf.Abs(Mathf.Sin(t * Mathf.PI * 5f) * (1f - Mathf.Sqrt(t))) * 2f;

		transform.position = Vector3.zero + (Vector3.up * y);
	}
}
