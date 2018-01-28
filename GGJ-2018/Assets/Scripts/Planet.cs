using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Planet : RigidbodyWatcher
{	
    [SerializeField]
    private float _antiGravityStrength = 30f;

    [System.NonSerialized]
    protected bool _active = false;

    protected override void Reset()
    {
        base.Reset();

        _active = false;
    }

    protected override void Update()
	{
		base.Update();

		if (IsGrabbed())
		{
			if (!_active)
            {
                for (int i = 0; i < _rigidbodyData.Length; ++i)
                {
                    Rigidbody rigidbody = _rigidbodyData[i].rigidbody;
                    rigidbody.useGravity = false;
                    rigidbody.AddForce(UnityEngine.Random.insideUnitSphere * _antiGravityStrength * rigidbody.mass);
                }
                _active = true;
            }
		}
	}
}