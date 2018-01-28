using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullUnderUpdater : MonoBehaviour 
{
	[SerializeField]
	private Mesh _gizmoMesh;

	[SerializeField]
	private Renderer[] _defaultRenderers;

	[SerializeField]
	private GameObject _getAllRenderersInChildren;

	[System.NonSerialized]
	private List<Renderer> _renderers;

	[System.NonSerialized]
	private Shader _sharedCullUnderShader;

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		if (_sharedCullUnderShader == null)
		{
			_sharedCullUnderShader = Shader.Find("Custom/CullUnder");;
		}
		_renderers = new List<Renderer>();
		foreach (Renderer renderer in _defaultRenderers)
		{
			if (renderer != null)
			{
				AddRenderer(renderer);
			}
		}
		if (_getAllRenderersInChildren != null)
		{
			foreach(Renderer renderer in _getAllRenderersInChildren.GetComponentsInChildren<Renderer>())
			{
				if (renderer != null)
				{
					AddRenderer(renderer);
				}
			}
		}
	}

	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		if (_gizmoMesh != null)
		{
			Gizmos.DrawWireMesh(_gizmoMesh, transform.position, transform.rotation, transform.localScale);
		}
	}

	public void AddRenderer(Renderer renderer)
	{
		if (!_renderers.Contains(renderer))
		{
			if (HasCullUnderMaterial(renderer))
			{
				_renderers.Add(renderer);
			}
			else
			{
				Debug.LogWarningFormat("Renderer {0} does not contain a {1} shader!", renderer.gameObject.name, _sharedCullUnderShader.name); 
			}			
		}
	}

	private bool HasCullUnderMaterial(Renderer renderer)
	{
		foreach (Material material in renderer.materials)
		{
			if (IsCullUnderMaterial(material))
			{
				return true;
			}
		}

		return false;
	}

	public void RemoveRenderer(Renderer renderer)
	{
		if (_renderers.Contains(renderer))
		{
			_renderers.Remove(renderer);
		}
	}

	private bool IsCullUnderMaterial(Material material)
	{
		return material.shader == _sharedCullUnderShader;
	}

	private void Update() 
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
					if (IsCullUnderMaterial(materials[i]))
					{
						materials[i].SetVector("_objWorldPosition", objectPosition);
						materials[i].SetVector("_objNormal", objectNormal);
					}
				}
			}
		}
	}
}
