using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class WindupToy : VRTK_InteractableObject 
{
	[Space, Header("Windup Toy")]
	[SerializeField]
	private VRTK_Wheel _wheel;

	[SerializeField]
	private Rigidbody _rigidbody;

	[SerializeField]
	private Transform _winderTransform;

	[SerializeField]
	private Vector3 _winderRotateAngleAmount;

	[SerializeField]
	private Transform _bounceSource;
	
	[SerializeField]
	private float _bounceDelay;

	[SerializeField]
	private Vector3 _bounceDirectionMin;

	[SerializeField]
	private Vector3 _bounceDirectionMax;

	[SerializeField]
	private float _checkForGroundDistance;

	[SerializeField]
	private float _windHapticStrength;

	[SerializeField]
	private float _bounceHapticStrength;

	[System.NonSerialized]
	private float _currentCharge = 0;

	[System.NonSerialized]
	private float _previousValue = 0;

	[System.NonSerialized]
	private float _timer;

	[System.NonSerialized]
	private VRTK_ControllerReference _controllerReference;

	private void Start() 
	{
		_wheel.ValueChanged += HandleWheelValueChanged;
	}

	private void OnDestroy()
	{
		_wheel.ValueChanged -= HandleWheelValueChanged;
	}

    private void HandleWheelValueChanged(object sender, Control3DEventArgs e)
    {
		float valueDelta = Mathf.Abs(e.value - _previousValue);
		_currentCharge += valueDelta;
		_previousValue = e.value;
		if (_controllerReference != null)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(_controllerReference, _windHapticStrength, 0.1f, 0.01f);
		}
    }

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);
        
        _controllerReference = VRTK_ControllerReference.GetControllerReference(currentGrabbingObject.controllerEvents.gameObject);
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);
        
        _controllerReference = null;
    }

    protected override void Update() 
	{
		base.Update();

		if (_currentCharge > 0)
		{
			if (_timer >= _bounceDelay)
			{
				if (Physics.Raycast(_bounceSource.position, -_bounceSource.up, _checkForGroundDistance))
				{
					Debug.DrawRay(_bounceSource.position, (-_bounceSource.up) * _checkForGroundDistance, Color.yellow);
					_currentCharge -= _wheel.stepSize;
					Vector3 bounceForce = new Vector3(
						UnityEngine.Random.Range(_bounceDirectionMin.x, _bounceDirectionMax.x),
						UnityEngine.Random.Range(_bounceDirectionMin.y, _bounceDirectionMax.y),
						UnityEngine.Random.Range(_bounceDirectionMin.z, _bounceDirectionMax.z)
					);
					Vector3 bounceDirection = new Vector3();
					bounceDirection = _bounceSource.forward * bounceForce.x;
					bounceDirection += _bounceSource.up * bounceForce.y;
					bounceDirection += _bounceSource.right * bounceForce.z;
					_rigidbody.AddForceAtPosition(bounceDirection, _bounceSource.position);
					_winderTransform.Rotate(_winderRotateAngleAmount.x, _winderRotateAngleAmount.y, _winderRotateAngleAmount.z);
					_timer = 0;
					if (_controllerReference != null)
					{
						VRTK_ControllerHaptics.TriggerHapticPulse(_controllerReference, _bounceHapticStrength, 0.1f, 0.01f);
					}
				}
			}
			else
			{
				_timer += Time.deltaTime;
			}
		}
		else
		{
			_currentCharge = 0;
		}
	}
}
