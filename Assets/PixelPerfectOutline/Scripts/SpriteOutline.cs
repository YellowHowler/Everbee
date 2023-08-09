using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Pixel Perfect Outline/Sprite Outline")]
public class SpriteOutline : MonoBehaviour
{
    public SpriteRenderer sr;

    [Serializable]
    struct Directions
    {
        public bool top;
        public bool bottom;
        public bool left;
        public bool right;

        public Directions(bool top, bool bottom, bool left, bool right)
        {
            this.top = top;
            this.bottom = bottom;
            this.left = left;
            this.right = right;
        }
    }

    [SerializeField]
    [HideInInspector]
    Material material;

    [SerializeField]
    Color outlineColor = Color.white;

    [SerializeField]
    Directions directions = new Directions(true, true, true, true);

    Color currentOutlineColor;
    Rect currentRect;
    Vector2 currentPivot;
    float currentPixelsPerUnit;
    Directions currentDirections;

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            outlineColor = value;
            UpdateProperties();
        }
    }

    void Reset()
    {
        sr.material = material;
        UpdateProperties();
    }

    void Awake()
    {
        UpdateProperties();
    }

    void LateUpdate()
    {
        UpdateProperties();
    }

    public void EnableOutline()
    {
        directions.top = true;
        directions.bottom = true;
        directions.left = true;
        directions.right = true;

        UpdateProperties();
    }

    public void DisableOutline()
    {
        directions.top = false;
        directions.bottom = false;
        directions.left = false;
        directions.right = false;

        UpdateProperties();
    }

    void UpdateProperties()
    {
        Rect spriteRect = sr.sprite.rect;
        Vector2 pivot = sr.sprite.pivot;
        float pixelsPerUnit = sr.sprite.pixelsPerUnit;

        if (outlineColor == currentOutlineColor && spriteRect == currentRect && pivot == currentPivot &&
            Mathf.Approximately(pixelsPerUnit, currentPixelsPerUnit) && directions.Equals(currentDirections))
            return;

        MaterialPropertyBlock properties = new MaterialPropertyBlock();
        sr.GetPropertyBlock(properties);

        Vector4 vector = new Vector4(spriteRect.x, spriteRect.y, spriteRect.width, spriteRect.height);
        properties.SetVector("_RectPosSize", vector);
        properties.SetVector("_Pivot", pivot);
        properties.SetFloat("_PixelsPerUnit", pixelsPerUnit);
        properties.SetColor("_OutlineColor", enabled ? OutlineColor : Color.clear);

        properties.SetFloat("_Top", directions.top ? 1 : 0);
        properties.SetFloat("_Bottom", directions.bottom ? 1 : 0);
        properties.SetFloat("_Left", directions.left ? 1 : 0);
        properties.SetFloat("_Right", directions.right ? 1 : 0);

        sr.SetPropertyBlock(properties);

        currentRect = spriteRect;
        currentPivot = pivot;
        currentPixelsPerUnit = pixelsPerUnit;
        currentOutlineColor = outlineColor;
        currentDirections = directions;
    }

    void OnDrawGizmosSelected()
    {
        UpdateProperties();
    }
}