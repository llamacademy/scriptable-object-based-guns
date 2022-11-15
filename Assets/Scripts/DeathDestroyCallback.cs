using UnityEngine;

public class DeathDestroyCallback : MonoBehaviour
{
    public void DeathEnd()
    {
        Destroy(gameObject);
    }
}
