using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LimitedRange {
	public float minimumBound;
	public float maximumBound;

	public float lowerBound {
		get { return _lowerBound; }
		set { _lowerBound = Mathf.Clamp(value, minimumBound, maximumBound); }
	}

	[SerializeField]
	private float _lowerBound;

	public float upperBound {
		get { return _upperBound; }
		set { _upperBound = Mathf.Clamp(value, minimumBound, maximumBound); }
	}
	[SerializeField]
	private float _upperBound;

	public LimitedRange(float min, float lower, float upper, float max) {
		minimumBound = min;
		maximumBound = max;
		_lowerBound = lower;
		_upperBound = upper;
	}
}