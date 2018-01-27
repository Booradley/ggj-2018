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
			AddCullUnderMaterial(renderer);
		}
	}

	private void AddCullUnderMaterial(Renderer renderer)
	{
		foreach (Material material in renderer.materials)
		{
			if (material.name == _instanceCullUnderMaterial.name)
			{
				return;
			}
		}

		Material[] materials = new Material[renderer.materials.Length + 1];
		for (int i = 0; i < renderer.materials.Length; ++i)
		{
			materials[i] = renderer.materials[i];
		}
		materials[renderer.materials.Length] = _instanceCullUnderMaterial;
		renderer.materials = materials;
	}

	public void RemoveRenderer(Renderer renderer)
	{
		if (_renderers.Contains(renderer))
		{
			_renderers.Remove(renderer);
            RemoveCullUnderMaterial(renderer);	
		}
	}

	private void RemoveCullUnderMaterial(Renderer renderer)
	{
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
				if (!materialRemoved && IsCullUnderMaterial(renderer.materials[i]))
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

	private bool IsCullUnderMaterial(Material material)
	{
		return material.name == _instanceCullUnderMaterial.name || material.name == _sharedCullUnderMaterial.name;
	}

	private void Update () 
	{
		foreach (Renderer renderer in _renderers)
		{
			if (renderer != null)
			{ 
				Material[] materials = renderer.materials;
				Vector4 objectNormal = new Vector4(transform.up.x, transform.up.y, transform.up.z, 0);
				for (int i = 0; i < materials.Length; ++i) 
				{
					if (IsCullUnderMaterial(materials[i]))
					{
						materials[i].SetVector("_objNormal", objectNormal);
					}
				}
			}
		}
	}
}
