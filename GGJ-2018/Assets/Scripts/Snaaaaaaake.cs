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
        int count = 200;
        while (count > 0)
        {
            yield return interval;

            GameObject go = Instantiate(_snake, transform.position, Random.rotation);
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(Random.insideUnitSphere * 0.5f);
            }

            count--;
        }
    }
}