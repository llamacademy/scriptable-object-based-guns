using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;

    private void Update()
    {
        GunSelector.ActiveGun.Tick(
            Application.isFocused && Mouse.current.leftButton.isPressed
            && GunSelector.ActiveGun != null
        );
    }
}
