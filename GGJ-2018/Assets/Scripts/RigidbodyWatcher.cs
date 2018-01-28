using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RigidbodyWatcher : VRTK_InteractableObject 
{
	[System.NonSerialized]
	private int _previousGameObjectCount = 0;

	[System.NonSerialized]
	protected RigidbodyData[] _rigidbodyData = new RigidbodyData[0];

	[System.NonSerialized]
	protected VRTK_ControllerReference _controllerReference;

	protected override void OnDisable()
	{
		base.OnDisable();

		Reset();
	}

	protected virtual void Reset()
	{	
        ResetRigidbodies();
	}

	private void ResetRigidbodies()
	{
		if (_rigidbodyData != null && _rigidbodyData.Length > 0)
		{
			for (int i = 0; i < _rigidbodyData.Length; ++i)
			{
				if (_rigidbodyData[i].rigidbody != null)
				{
					_rigidbodyData[i].rigidbody.useGravity = _rigidbodyData[i].useGravity;
				}
			}
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

		Reset();
    }

	protected override void Update()
	{
		base.Update();

		UpdateRigidbodyReferences();
	}

	private void UpdateRigidbodyReferences()
	{
		GameObject[] gameObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
		if (gameObjects.Length != _previousGameObjectCount)
		{
			ResetRigidbodies();

			List<Rigidbody> rigidbodies = new List<Rigidbody>();
			for (int i = 0; i < gameObjects.Length; ++i)
			{
				Rigidbody rigidbody = gameObjects[i].GetComponentInChildren<Rigidbody>(false);
				if (rigidbody != null)
				{
					rigidbodies.Add(rigidbody);
				}
			}

			_rigidbodyData = new RigidbodyData[rigidbodies.Count];
			for (int i = 0; i < rigidbodies.Count; ++i)
			{
				_rigidbodyData[i] = new RigidbodyData(rigidbodies[i]);
			}
			_previousGameObjectCount = gameObjects.Length;
		}
	}

	public class RigidbodyData
	{
		public Rigidbody rigidbody;
		public bool useGravity;

		public RigidbodyData(Rigidbody rigidbody)
		{
			this.rigidbody = rigidbody;
			this.useGravity = rigidbody.useGravity;
		}
	}
}
