using UnityEngine;

namespace LlamAcademy.Guns.Demo
{
    public class DeathDestroyCallback : MonoBehaviour
    {
        public void DeathEnd()
        {
            Destroy(gameObject);
        }
    }
}