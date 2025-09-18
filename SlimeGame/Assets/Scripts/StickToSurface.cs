using UnityEngine;

public class StickToSurface : MonoBehaviour
{
    public bool isDynamicObject;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        var player = collision.gameObject.GetComponent<PlayerLaunch>();

        if (player == null)
        {
            Debug.LogError("PlayerLaunch script is null. (StickToSurface)");
            return;
        }

        player.AttachToSurface(isDynamicObject);
    }
}
