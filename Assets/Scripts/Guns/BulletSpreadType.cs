namespace LlamAcademy.Guns
{
    /// <summary>
    /// Defines how spread is calculated.
    /// </summary>
    public enum BulletSpreadType
    {
        /// <summary>
        /// No Spread is ever calculated. Always will shoot directly forward based on the <see cref="ShootType"/>
        /// </summary>
        None,
        /// <summary>
        /// Picks random values in each of X,Y,Z as defined in <see cref="ShootConfigScriptableObject.Spread"/>.
        /// </summary>
        Simple,
        /// <summary>
        /// Spread calculated based on a provided texture's greyscale value. More white values are more likely to be chosen.
        /// Larger textures are significantly slower to read.
        /// </summary>
        TextureBased
    }
}