using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State { PLAYING, AIMING, PLACING, GAMEOVER }
    [SerializeField] Mover mover = null;
    [SerializeField] Striker striker = null;

    Camera cam;

    public static State gameState = State.PLACING;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        cam = Camera.main;
    }

    private void Start()
    {
        float orthoSize = 2.9f * Screen.height / Screen.width;     //5.8f/2 = 2.9f being size of board+some_padding.
        orthoSize = Mathf.Clamp(orthoSize, 3.5f, 10);
        cam.orthographicSize = orthoSize;
        ResetTurn();
    }


    public void FinishTurn()
    {
        StartCoroutine(FinishTurnAfterADelay());
    }

    IEnumerator FinishTurnAfterADelay()     //could wait for all coins to stop moving.
    {
        yield return new WaitForSeconds(2);
        ResetTurn();
    }

    private void ResetTurn()
    {
        gameState = State.PLACING;
        striker.Reset();
        mover.AttemptPlacingStriker(0);
    }
}
