using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;
    [SerializeField]
    private bool AutoReload = false;
    [SerializeField]
    private PlayerIK InverseKinematics;
    [SerializeField]
    private Animator PlayerAnimator;
    private bool IsReloading;

    private void Update()
    {
        GunSelector.ActiveGun.Tick(
            Application.isFocused && Mouse.current.leftButton.isPressed
            && GunSelector.ActiveGun != null
        );

        if (ShouldManualReload() || ShouldAutoReload()) 
        {
            IsReloading = true;
            PlayerAnimator.SetTrigger("Reload");
            InverseKinematics.HandIKAmount = 0.25f;
            InverseKinematics.ElbowIKAmount = 0.25f;
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

    public void EndReload()
    {
        GunSelector.ActiveGun.EndReload();
        InverseKinematics.HandIKAmount = 1f;
        InverseKinematics.ElbowIKAmount = 1f;
        IsReloading = false;
    }
}
