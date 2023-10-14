namespace LlamAcademy.Guns.Persistence.Model
{
    /// <summary>
    /// Simple Player Loadout model for persisting state on disk.
    /// Using <see cref="UnityEngine.JsonUtility"/> allows us to easily and concisely refer to an existing object
    /// by instance id which keeps file size very low.
    /// </summary>
    public class Loadout
    {
        /// <summary>
        /// Reference to the GunScriptableObject. When persisted, this will just have the InstanceId instead of the full structure
        /// </summary>
        public GunScriptableObject Gun;
        /// <summary>
        /// Ordered attachments by <see cref="Slot"/>. Index is important!
        /// 0 - Barrel
        /// 1 - Handle
        /// 2 - Ammo
        /// </summary>
        public int[] Attachments;
    }
}