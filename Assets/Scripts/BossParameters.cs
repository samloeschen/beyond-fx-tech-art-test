using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct BossParameters {
	public string name;
	public string subtitle;
	public Element strongType;
	public Element weakType;
	public float healthPoints;
	public float moveSpeed;
	public LimitedRange attackDelayTime;
	public LimitedRange moveDelayTime;

	[TextArea]
	public string description;
}

