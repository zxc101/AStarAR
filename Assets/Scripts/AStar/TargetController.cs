using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private Queue<GameObject> saveTargets;

    public Queue<GameObject> SaveTargets { get => saveTargets; }

    private void Start()
    {
        saveTargets = new Queue<GameObject>();
    }

    public void SpawnTarget(Vector3 position)
    {
        saveTargets.Enqueue(Instantiate(target, position, Quaternion.identity));
    }

    public void DestroyTarget()
    {
        if(saveTargets.Count != 0)
        {
            Destroy(saveTargets.Peek());
            saveTargets.Dequeue();
        }
    }
}
