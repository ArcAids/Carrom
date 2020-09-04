using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float bounds = 3;
    [SerializeField] Striker striker = null;
    [SerializeField] Camera cam = null;
    [SerializeField] GameObject overlappingText = null;

    float currentX = 0;
    const float stepToFixPosition = 0.02f;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    private void OnMouseDown()
    {
        StopAllCoroutines();
    }

    private void OnMouseDrag()
    {
        if (GameManager.gameState == GameManager.State.PLACING)
        {
            float x = Mathf.Clamp(cam.ScreenToWorldPoint(Input.mousePosition).x, -bounds, bounds);
            transform.localPosition = new Vector3(x, transform.localPosition.y);
            List<Collider2D> list = striker.PreviewXPosition(x);
            if (list != null && list.Count > 0)
            {
                overlappingText.SetActive(true);
            }
            else
            {
                overlappingText.SetActive(false);
            }
            currentX = x;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.gameState == GameManager.State.PLACING)
        {
            AttemptPlacingStriker(currentX);
        }
    }

    public void AttemptPlacingStriker(float xPosition)
    {
        StartCoroutine(PositionCorrection(xPosition));
    }

    IEnumerator PositionCorrection(float xPosition)
    {
        transform.localPosition = new Vector3(xPosition, transform.localPosition.y);
        List<Collider2D> list = striker.PreviewXPosition(xPosition);
        if (list != null)
        {
            xPosition = Mathf.Clamp(xPosition, -bounds, bounds);
            yield return null;
            if (list.Count > 0)
            {
                overlappingText.SetActive(true);
                float direction = FindDirection(xPosition, list);
                //Debug.Log(xPosition + direction);
                yield return null;
                StartCoroutine(PositionCorrection(xPosition + direction));
            }
            else
                PlaceStriker(xPosition);
        }
    }

    /// <summary>
    /// Finds direction to move striker in to avoid collision.
    /// </summary>
    /// <param name="xPosition">x position of striker</param>
    /// <param name="list">list of collisions</param>
    /// <returns>small step in direction</returns>
    private float FindDirection(float xPosition, List<Collider2D> list)
    {
        float direction;
        float averagePosition = 0;
        foreach (var item in list)
        {
            averagePosition += item.transform.position.x;
        }
        averagePosition /= list.Count;
        if (Mathf.Abs(averagePosition) > (bounds - 0.4f))
        {
            direction = averagePosition >= 0 ? -stepToFixPosition : stepToFixPosition;
        }
        else
        {
            direction = averagePosition > xPosition ? -stepToFixPosition : stepToFixPosition;
        }

        return direction;
    }

    void PlaceStriker(float x)
    {
        overlappingText.SetActive(false);
        striker.SetXPosition(x);
        transform.localPosition = new Vector3(x, transform.localPosition.y);
    }
}
