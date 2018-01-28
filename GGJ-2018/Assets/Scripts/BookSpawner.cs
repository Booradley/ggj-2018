﻿using UnityEngine;

public class BookSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _bookGizmo;

    [SerializeField]
    private GameObject[] _bookPrefabs;

    [SerializeField]
    private float _spawnDistance;

    [SerializeField]
    private int _minBooks;

    [SerializeField]
    private int _maxBooks;

    private void Awake()
    {
        _bookGizmo.SetActive(false);

        int count = Random.Range(_minBooks, _maxBooks + 1);
        float space = _spawnDistance / count;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = transform.position + transform.right * (i * space);
            pos.x += Random.Range(0f, space - 0.03f);
            Instantiate(_bookPrefabs[Random.Range(0, _bookPrefabs.Length)], pos, transform.rotation * Quaternion.Euler(new Vector3(90f, -90f, 0f)));
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 right = transform.position + (transform.right * _spawnDistance);

        Gizmos.DrawLine(transform.position, right);
    }
}