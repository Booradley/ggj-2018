using UnityEngine;
using System;
using System.Linq;

[Serializable]
public struct WeightedBook
{
    public GameObject bookPrefab;
    public int weight;
}

public class BookSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _bookGizmo;

    [SerializeField]
    private WeightedBook[] _bookPrefabs;

    [SerializeField]
    private float _spawnDistance;

    [SerializeField]
    private int _minBooks;

    [SerializeField]
    private int _maxBooks;

    private void Awake()
    {
        _bookGizmo.SetActive(false);

        int count = UnityEngine.Random.Range(_minBooks, _maxBooks + 1);
        float space = _spawnDistance / count;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + transform.right * (i * space);
            pos.x += UnityEngine.Random.Range(0f, space - 0.03f);
            pos.z += UnityEngine.Random.Range(-0.005f, 0.005f);
            Instantiate(GetRandomBook(), pos, transform.rotation * Quaternion.Euler(new Vector3(90f, -90f, 0f)));
        }
    }

    private GameObject GetRandomBook()
    {
        int totalWeight = _bookPrefabs.Sum(weightedBook => weightedBook.weight);
        int roll = UnityEngine.Random.Range(0, totalWeight);
        int currentWeight = 0;
        for (int i = 0; i < _bookPrefabs.Length; i++)
        {
            currentWeight += _bookPrefabs[i].weight;
            if (currentWeight >= roll)
            {
                return _bookPrefabs[i].bookPrefab;
            }
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Vector3 right = transform.position + (transform.right * _spawnDistance);

        Gizmos.DrawLine(transform.position, right);
    }
}