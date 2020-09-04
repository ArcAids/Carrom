using System.Collections.Generic;
using UnityEngine;

public class Striker : MonoBehaviour
{
    [SerializeField] float basePower = 10;
    [SerializeField] float minThreshold = 0.5f;
    [SerializeField] float dragRange = 2;
    [SerializeField] SpriteRenderer glow = null;
    [SerializeField] Camera cam = null;

    CircularPowerMeter powerCircle = null;
    StrikerPowerArrow arrow = null;
    Rigidbody2D rigidBody = null;
    StrikePredictinator predictInator3000 = null;
    new Collider2D collider = null;

    Vector3 originalPosition = Vector3.zero;
    Vector2 aimVector = Vector2.zero;

    bool dragged = false;
    bool isTurnOver = false;
    bool canShoot = false;
    public bool Dragged
    {
        get => dragged;
        set
        {
            arrow?.Enable(value);
            powerCircle?.Enable(value);
            dragged = value;
        }
    }

    private void Awake()
    {
        TryGetComponent(out rigidBody);
        TryGetComponent(out predictInator3000);
        collider = GetComponentInChildren<Collider2D>();
        arrow = GetComponentInChildren<StrikerPowerArrow>(true);
        powerCircle = GetComponentInChildren<CircularPowerMeter>(true);
        if (cam == null)
            cam = Camera.main;
        originalPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (canShoot && GameManager.gameState == GameManager.State.PLACING)
            GameManager.gameState = GameManager.State.AIMING;
        glow.enabled = false;
    }

    private void OnMouseDrag()
    {
        if (GameManager.gameState != GameManager.State.AIMING) return;

        aimVector = transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
        if (aimVector.sqrMagnitude <= minThreshold * minThreshold)
        {
            Dragged = false;
            glow.enabled = true;
        }
        else
        {
            float magnitudeNormalized = Mathf.Clamp(aimVector.magnitude- minThreshold, .1f, dragRange) / dragRange;
            aimVector = magnitudeNormalized * aimVector.normalized * basePower;

            powerCircle?.SetRadius(magnitudeNormalized);
            UpdateArrowDirection(magnitudeNormalized);

            predictInator3000?.Predict(aimVector.normalized);

            Dragged = true;
            glow.enabled = false;
        }
    }
    private void OnMouseUp()
    {
        predictInator3000?.Enable(false);
        if (GameManager.gameState != GameManager.State.AIMING) return;

        if (!Dragged)
        {
            GameManager.gameState = GameManager.State.PLACING;
            glow.enabled = true;
            return;
        }

        Shoot();

        Dragged = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.gameState != GameManager.State.PLAYING) return;

        if (!isTurnOver)
        {
            if (rigidBody.velocity.sqrMagnitude <= 0.02f)
            {
                isTurnOver = true;
                GameManager.Instance.FinishTurn();
            }
        }
    }

    public void Reset()
    {
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0;
        rigidBody.rotation = 0;

        collider.isTrigger = true;
        Dragged = false;
        glow.enabled = true;
    }

    private void UpdateArrowDirection(float magnitudeNormalized)
    {
        if (arrow == null) return;
        arrow.RotateTo(Mathf.Atan2(aimVector.normalized.y, aimVector.normalized.x) * Mathf.Rad2Deg);
        arrow.SetLength(magnitudeNormalized);
    }

    void Shoot()
    {
        rigidBody.AddForce(aimVector * 10);
        canShoot = false;
        GameManager.gameState = GameManager.State.PLAYING;
        Invoke("SetTurnStarted", 0.5f);
    }

    void SetTurnStarted()
    {
        isTurnOver = false;
    }

    public List<Collider2D> PreviewXPosition(float x)
    {
        if (GameManager.gameState != GameManager.State.PLACING)
        {
            return null;
        }

        canShoot = false;
        collider.isTrigger = true;
        rigidBody.position = new Vector2(x, originalPosition.y);

        List<Collider2D> list = new List<Collider2D>();
        if (rigidBody.OverlapCollider(new ContactFilter2D(), list) > 0)
        {
            glow.enabled = false;
            return list;
        }
        else
        {
            glow.enabled = true;
            return list;
        }
    }

    public void SetXPosition(float x)
    {
        if (GameManager.gameState != GameManager.State.PLACING) return;

        collider.isTrigger = false;
        canShoot = true;
        isTurnOver = true;
        rigidBody.position = new Vector2(x, originalPosition.y);
    }

}
