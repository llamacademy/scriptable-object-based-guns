using UnityEngine;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "Ammo Config", menuName = "Guns/Ammo Config", order = 3)]
    public class AmmoConfigScriptableObject : ScriptableObject, System.ICloneable
    {
        public int MaxAmmo = 120;
        public int ClipSize = 30;

        public int CurrentAmmo = 120;
        public int CurrentClipAmmo = 30;

        /// <summary>
        /// Reloads with the ammo conserving algorithm.
        /// Meaning it will only subtract the delta between the ClipSize and CurrentClipAmmo from the CurrentAmmo.
        /// </summary>
        public void Reload()
        {
            int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
            int availableBulletsInCurrentClip = ClipSize - CurrentClipAmmo;
            int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);
            CurrentClipAmmo += reloadAmount;
            CurrentAmmo -= reloadAmount;
        }

        /// <summary>
        /// Reloads not conserving ammo.
        /// Meaning it will always subtract the ClipSize from CurrentAmmo (if available).
        /// </summary>
        //public void Reload()
        //{
        //    int reloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
        //    CurrentClipAmmo = reloadAmount;
        //    CurrentAmmo -= reloadAmount;
        //}

        public bool CanReload()
        {
            return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
        }

        public void AddAmmo(int Amount)
        {
            if (CurrentAmmo + Amount > MaxAmmo)
            {
                CurrentAmmo = MaxAmmo;
            }
            else
            {
                CurrentAmmo += Amount;
            }
        }

        public object Clone()
        {
            AmmoConfigScriptableObject config = CreateInstance<AmmoConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}