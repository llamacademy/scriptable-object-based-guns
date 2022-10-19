using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Shoot Config", menuName = "Guns/Shoot Config", order = 2)]
public class ShootConfigScriptableObject : ScriptableObject
{
    public LayerMask HitMask;
    public float FireRate = 0.25f;
    public BulletSpreadType SpreadType = BulletSpreadType.Simple;
    public float RecoilRecoverySpeed = 1f;
    public float MaxSpreadTime = 1f;
    [Header("Simple Spread")]
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    [Header("Texture-Based Spread")]
    [Range(0.001f, 5f)]
    public float SpreadMultiplier = 0.1f;
    public Texture2D AdvancedSpreadTexture;

    public SpriteRenderer SpriteRenderer;

    private Color[] OriginalColors = null;
    private Texture2D RenderTexture = null;

    public Vector3 GetSpread(float ShootTime = 0)
    {
        Vector3 spread = Vector3.zero;

        if (SpreadType == BulletSpreadType.Simple)
        {
            spread = Vector3.Lerp(
                Vector3.zero,
                new Vector3(
                    Random.Range(-Spread.x, Spread.x),
                    Random.Range(-Spread.y, Spread.y),
                    Random.Range(-Spread.z, Spread.z)
                ),
                Mathf.Clamp01(ShootTime / MaxSpreadTime)
            );
        }
        else if (SpreadType == BulletSpreadType.TextureBased)
        {
            Vector3 direction = GetTextureDirection(ShootTime);

            spread = direction * SpreadMultiplier;
        }

        return spread;
    }

    private Vector3 GetTextureDirection(float ShootTime)
    {
        Vector2 halfSize = new Vector2(AdvancedSpreadTexture.width / 2f, AdvancedSpreadTexture.height / 2f);

        // how to handle shotgun where you might always want some spread?
        int halfSquareRadius = Mathf.CeilToInt(Mathf.Lerp(1, halfSize.x, Mathf.Clamp01(ShootTime / MaxSpreadTime)));

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareRadius;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareRadius;

        if (OriginalColors == null || RenderTexture == null)
        {
            OriginalColors = AdvancedSpreadTexture.GetPixels();
            RenderTexture = new Texture2D(AdvancedSpreadTexture.width, AdvancedSpreadTexture.height, AdvancedSpreadTexture.format, false, true);
            RenderTexture.SetPixels(OriginalColors);
            RenderTexture.Apply();

            Sprite sprite = Sprite.Create(
                RenderTexture,
                new Rect(0, 0, RenderTexture.width, RenderTexture.height),
                Vector2.one / 2f
            );
            SpriteRenderer.sprite = sprite;
        }

        Color[] sampleColors = AdvancedSpreadTexture.GetPixels(
            minX,
            minY,
            halfSquareRadius * 2,
            halfSquareRadius * 2
        );

        float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGreyValue = colorsAsGrey.Sum();

        float grey = Random.Range(0, totalGreyValue);
        int i = 0;
        for (; i < colorsAsGrey.Length; i++)
        {
            grey -= colorsAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareRadius * 2);
        int y = minY + i / (halfSquareRadius * 2);

        Vector3 targetPosition = new Vector3(x, y, 0);

        Vector3 direction = (targetPosition - new Vector3(halfSize.x, halfSize.y, 0)) / halfSize.x;

        RenderTexture.SetPixels(OriginalColors);
        SetRectPixels(RenderTexture, minX, minY, halfSquareRadius * 2);
        RenderTexture.SetPixel(x, y, Color.green);
        RenderTexture.Apply();

        return direction;
    }

    //For debug UI only
    private void SetRectPixels(Texture2D RenderTexture, int MinX, int MinY, int RectSize)
    {
        for (int x = 0; x < RectSize; x++)
        {
            for (int y = 0; y < RectSize; y++)
            {
                if (x == 0 || x == RectSize - 1 || y == 0 || y == RectSize - 1)
                {
                    RenderTexture.SetPixel(MinX + x, MinY + y, Color.red);
                }
            }
        }
    }

    public enum BulletSpreadType
    {
        None,
        Simple,
        TextureBased
    }
}
