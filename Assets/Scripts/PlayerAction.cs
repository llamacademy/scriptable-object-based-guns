using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    private PlayerGunSelector GunSelector;
    public SpriteRenderer Sprite;

    private void Start()
    {
        GunSelector.ActiveGun.ShootConfig.SpriteRenderer = Sprite;    
    }

    private void Update()
    {
        GunSelector.ActiveGun.Tick(
            Mouse.current.leftButton.isPressed
            && GunSelector.ActiveGun != null
        );
    }
}
