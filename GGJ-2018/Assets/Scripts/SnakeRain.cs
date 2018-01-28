using System.Collections;
using UnityEngine;
using VRTK;

public class SnakeRain : VRTK_InteractableObject
{
    [SerializeField]
    private GameObject[] _snakePrefabs;

    private Coroutine _rainCoroutine;

    public override void Grabbed(VRTK_InteractGrab currentGrabbingObject = null)
    {
        base.Grabbed(currentGrabbingObject);

        _rainCoroutine = StartCoroutine(Spawn());
    }

    public override void Ungrabbed(VRTK_InteractGrab previousGrabbingObject = null)
    {
        base.Ungrabbed(previousGrabbingObject);

        if (_rainCoroutine != null)
        {
            StopCoroutine(_rainCoroutine);
            _rainCoroutine = null;
        }
    }

    private IEnumerator Spawn()
    {
        Transform spawnTransform = GameObject.FindGameObjectWithTag("Snake Rain Spawner").transform;
        WaitForSeconds interval = new WaitForSeconds(.5f);
        while (true)
        {
            yield return interval;

            GameObject go = Instantiate(_snakePrefabs[UnityEngine.Random.Range(0, _snakePrefabs.Length)], spawnTransform.position, Random.rotation);
            Rigidbody[] rbs = go.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rbs.Length; i++)
            {
                rbs[i].AddForce(Random.insideUnitSphere * 0.5f);
            }
        }
    }
}