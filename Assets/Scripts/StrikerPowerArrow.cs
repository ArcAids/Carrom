using UnityEngine;

public class StrikerPowerArrow : MonoBehaviour
{
    [SerializeField] float maxLength = 2;
    [SerializeField] Transform arrow = null;
    [SerializeField] Transform line = null;

    float lineHeight = 0;
    float arrowOffset = 0;

    private void Awake()
    {
        if (line.TryGetComponent(out SpriteRenderer renderer))
        {
            lineHeight = renderer.sprite.rect.height / 100;        //where 100 is pixels per unit of sprite
        }
        arrowOffset = line.localPosition.x;
    }


    public void Enable(bool state)
    {
        gameObject.SetActive(state);
    }

    public void RotateTo(float angle)
    {
        transform.rotation = Quaternion.Euler(
            new Vector3(
            0,
            0,
            angle)
            );
    }

    public void SetLength(float normalizedValue)
    {
        line.transform.localScale = new Vector2(line.transform.localScale.x, normalizedValue * maxLength);
        arrow.transform.localPosition = new Vector2(arrowOffset + normalizedValue * maxLength * lineHeight, 0);
    }
}
