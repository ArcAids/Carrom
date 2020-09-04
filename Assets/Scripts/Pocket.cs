using UnityEngine;

public class Pocket : MonoBehaviour
{
    [SerializeField] float threshold = 20f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Rigidbody2D rigid))
        {
            if (rigid.velocity.sqrMagnitude <= threshold * threshold)
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}
