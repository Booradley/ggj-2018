using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullUnderUpdater : MonoBehaviour 
{
	[SerializeField]
	private Material _sharedCullUnderMaterial;

	[SerializeField]
	private Renderer[] _defaultRenderers;

	[System.NonSerialized]
	private List<Renderer> _renderers;

	[System.NonSerialized]
	private Material _instanceCullUnderMaterial;

	private void Awake()
	{
		_renderers = new List<Renderer>();
		_instanceCullUnderMaterial = new Material(_sharedCullUnderMaterial);
		foreach (Renderer renderer in _defaultRenderers)
		{
			if (renderer != null)
			{
				AddRenderer(renderer);
			}
		}
	}

	public void AddRenderer(Renderer renderer)
	{
		if (!_renderers.Contains(renderer))
		{
			_renderers.Add(renderer);

			bool materialAlreadyExists = false;
			foreach (Material material in renderer.materials)
			{
				if (material.name == _instanceCullUnderMaterial.name)
				{
					materialAlreadyExists = true;
				}
			}
			if (!materialAlreadyExists)
			{
				Material[] materials = new Material[renderer.materials.Length + 1];
				for (int i = 0; i < renderer.materials.Length; ++i)
				{
					materials[i] = renderer.materials[i];
				}
				materials[renderer.materials.Length] = _instanceCullUnderMaterial;
				renderer.materials = materials;
			}
		}
	}

	public void RemoveRenderer(Renderer renderer)
	{
		if (_renderers.Contains(renderer))
		{
			_renderers.Remove(renderer);

			bool materialAlreadyExists = false;
			bool materialRemoved = true;
			foreach (Material material in renderer.materials)
			{
				if (material.name == _instanceCullUnderMaterial.name)
				{
					materialAlreadyExists = true;
					materialRemoved = false;
				}
			}
			if (materialAlreadyExists && renderer.materials.Length > 1)
			{
				Material[] materials = new Material[renderer.materials.Length - 1];
				int i = 0;
				int j = 0;
				for (; i < renderer.materials.Length; ++i, ++j)
				{
					if (!materialRemoved && renderer.materials[i].name == _instanceCullUnderMaterial.name)
					{
						--j;
						materialRemoved = true;
					}
					else
					{
						materials[j] = renderer.materials[i];
					}
				}
				renderer.materials = materials;
			}
		}
	}

	private void Update () 
	{
		foreach (Renderer renderer in _renderers)
		{
			if (renderer != null)
			{ 
				Material[] materials = renderer.materials;
				Vector4 objectPosition = new Vector4(transform.position.x, transform.position.y, transform.position.z, 0);
				Vector4 objectNormal = new Vector4(transform.up.x, transform.up.y, transform.up.z, 0);
				for (int i = 0; i < materials.Length; ++i) 
				{
					materials[i].SetVector("_objWorldPosition", objectPosition);
					materials[i].SetVector("_objNormal", objectNormal);
				}
			}
		}
	}
}
