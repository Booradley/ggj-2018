using UnityEngine;

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
        for (int i = 0; i < count; i++)
        {

        }
    }

    private void OnDrawGizmos()
    {
        Vector3 left = transform.position + (transform.right * (_spawnDistance * -0.5f));
        Vector3 right = transform.position + (transform.right * (_spawnDistance * 0.5f));

        Gizmos.DrawLine(left, right);
    }
}