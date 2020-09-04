using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionArrow : MonoBehaviour
{
    public void Enable(bool state)
    {
        gameObject.SetActive(state);
    }

    public void MoveTo(Vector2 position)
    {
        transform.position = position;
    }

    public void RotateTo(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(
            0,
            0,
            angle)
            );
    }
}
