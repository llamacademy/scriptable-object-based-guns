using UnityEngine;
using UnityEngine.Pool;

public class PoolableObject : MonoBehaviour
{
    public ObjectPool<GameObject> Parent;

    private void OnDisable()
    {
        if (Parent != null)
        {
            Parent.Release(gameObject);
        }
    }
}
