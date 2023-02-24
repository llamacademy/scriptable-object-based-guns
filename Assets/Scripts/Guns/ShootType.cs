namespace LlamAcademy.Guns
{
    /// <summary>
    /// Defines the shooting mechanism for a particular gun.
    /// </summary>
    public enum ShootType
    {
        /// <summary>
        /// Indicates that a raycast will happen from the center of screen and a trail/bullet will be shot towards the hit point from the camera.
        /// </summary>
        FromCamera,
        /// <summary>
        /// Indicates that shooting happens based on the barrel of the gun instead of from the center of the screen.
        /// </summary>
        FromGun
    }

}
