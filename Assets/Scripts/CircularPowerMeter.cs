using UnityEngine;

public class CircularPowerMeter : MonoBehaviour
{
    [SerializeField] float minRadius = 2;
    [SerializeField] float maxRadius = 2;

    public void Enable(bool state)
    {
        gameObject.SetActive(state);
    }

    public void RotateTo(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(
            0,
            0,
            angle)
            );
    }

    public void SetRadius(float normalizedValue)
    {
        float radius = minRadius + normalizedValue * maxRadius;
        transform.localScale = new Vector3(radius, radius, radius);
    }
}
