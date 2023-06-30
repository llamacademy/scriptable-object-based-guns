using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LlamAcademy.Guns.Demo
{
    [DisallowMultipleComponent]
    public class PlayerAction : MonoBehaviour
    {
        // public for editor
        public PlayerGunSelector GunSelector;
        [SerializeField]
        private bool AutoReload = false;
        [SerializeField]
        private PlayerIK InverseKinematics;
        [SerializeField]
        private Animator PlayerAnimator;
        [SerializeField]
        private Image Crosshair;
        [SerializeField]
        private Transform AimTarget;
        private bool IsReloading;

        private void Update()
        {
            GunSelector.ActiveGun.Tick(
                !IsReloading
                && Application.isFocused && Mouse.current.leftButton.isPressed
                && GunSelector.ActiveGun != null
            );

            if (ShouldManualReload() || ShouldAutoReload())
            {
                GunSelector.ActiveGun.StartReloading();
                IsReloading = true;
                PlayerAnimator.SetTrigger("Reload");
                InverseKinematics.HandIKAmount = 0.25f;
                InverseKinematics.ElbowIKAmount = 0.25f;
            }

            UpdateCrosshair();
        }

        private void UpdateCrosshair()
        {
            Vector3 gunTipPoint = GunSelector.ActiveGun.GetRaycastOrigin();
            Vector3 forward;
            if (GunSelector.ActiveGun.ShootConfig.ShootType == ShootType.FromGun)
            {
                forward = GunSelector.ActiveGun.GetGunForward();
            }
            else
            {
                forward = GunSelector.Camera.transform.forward;
            }

            Vector3 hitPoint = gunTipPoint + forward * 10;
            if (Physics.Raycast(gunTipPoint, forward, out RaycastHit hit, float.MaxValue, GunSelector.ActiveGun.ShootConfig.HitMask))
            {
                hitPoint = hit.point;
            }

            AimTarget.transform.position = hitPoint;

            if (GunSelector.ActiveGun.ShootConfig.ShootType == ShootType.FromGun)
            {
                Vector3 screenSpaceLocation = GunSelector.Camera.WorldToScreenPoint(hitPoint);

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform)Crosshair.transform.parent,
                    screenSpaceLocation,
                    null,
                    out Vector2 localPosition))
                {
                    Crosshair.rectTransform.anchoredPosition = localPosition;
                }
                else
                {
                    Crosshair.rectTransform.anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                Crosshair.rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        private bool ShouldManualReload()
        {
            return !IsReloading
                && Keyboard.current.rKey.wasReleasedThisFrame
                && GunSelector.ActiveGun.CanReload();
        }

        private bool ShouldAutoReload()
        {
            return !IsReloading
                && AutoReload
                && GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0
                && GunSelector.ActiveGun.CanReload();
        }

        private void EndReload()
        {
            GunSelector.ActiveGun.EndReload();
            InverseKinematics.HandIKAmount = 1f;
            InverseKinematics.ElbowIKAmount = 1f;
            IsReloading = false;
        }
    }
}