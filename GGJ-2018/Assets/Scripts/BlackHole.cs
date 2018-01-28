using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BlackHole : RigidbodyWatcher 
{
	[Space, Header("Black Hole Settings")]
	[SerializeField]
	private float _pulseDelay;

	[SerializeField]
	private float _pullForce;

	[SerializeField]
	private float _minGravityPercent;

	[SerializeField]
	private float _minTimeScalePercent;

	[SerializeField]
	private float _disableGravityPercent;

	[SerializeField]
	private float _pulseHapticStrength;

	[System.NonSerialized]
	private float _timer = 0;

	[System.NonSerialized]
	private Vector3 _originalGravity;

	[System.NonSerialized]
	private float _originalTimeScale;

	[System.NonSerialized]
	private bool _pulsing = false;

	[System.NonSerialized]
	private bool _gravityOverridden = false;

	private void Start()
	{
		_originalGravity = Physics.gravity;
		_originalTimeScale = Time.timeScale;
		_timer = 0;
		_pulsing = false;
		_gravityOverridden = false;
	}

	protected override void Reset()
	{
		if (_pulsing)
		{
			Physics.gravity = _originalGravity;
			Time.timeScale = _originalTimeScale;
		}

		base.Reset();
	}

	protected override void Update()
	{
		base.Update();

		if (IsGrabbed())
		{
			if (_timer >= _pulseDelay)
			{
				_timer = 0;
				Pulse();
			}
			else if (_pulsing)
			{
				UpdatePulse();
			}

			_timer += Time.deltaTime;
		}
	}

	private void Pulse()
	{
		_pulsing = true;
		Time.timeScale = _originalTimeScale * _minTimeScalePercent;
		Physics.gravity = _originalGravity * _minGravityPercent;
		_gravityOverridden = false;
		if (_disableGravityPercent > 0)
		{
			_gravityOverridden = true;
		}
		for (int i = 0; i < _rigidbodyData.Length; ++i)
		{
			Vector3 direction = transform.position - _rigidbodyData[i].rigidbody.transform.position;
			if (_gravityOverridden)
			{
				_rigidbodyData[i].rigidbody.useGravity = false;
			}
			_rigidbodyData[i].rigidbody.AddForce(direction * _pullForce * _rigidbodyData[i].rigidbody.mass);
		}
		
		if (_controllerReference != null)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(_controllerReference, _pulseHapticStrength, 0.2f, 0.01f);
		}
	}

	private void UpdatePulse()
	{
		float progress = Mathf.Clamp01(_timer / _pulseDelay);
		Time.timeScale = (_originalTimeScale * progress) + ((_originalTimeScale - (_originalTimeScale * progress)) * Mathf.Max(_minTimeScalePercent, progress));
		Physics.gravity = _originalGravity * Mathf.Max(_minGravityPercent, progress);
		if (progress > _disableGravityPercent && _gravityOverridden)
		{
			for (int i = 0; i < _rigidbodyData.Length; ++i)
			{
				_rigidbodyData[i].rigidbody.useGravity = _rigidbodyData[i].useGravity;
			}
			_gravityOverridden = false;
		}
	}
}
