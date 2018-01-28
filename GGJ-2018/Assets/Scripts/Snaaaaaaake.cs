using System.Collections;
using UnityEngine;

public class Snaaaaaaake : MonoBehaviour
{
    [SerializeField]
    private GameObject _snake;

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        WaitForSeconds interval = new WaitForSeconds(.5f);
        while (true)
        {
            yield return interval;

            Instantiate(_snake, transform.position, Random.rotation);
        }
    }
}