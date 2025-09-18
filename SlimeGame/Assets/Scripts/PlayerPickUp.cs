using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPickUp : MonoBehaviour
{
    public GameObject objectToAbsorb;

    public bool HasObject { get; private set; }

    public bool InRangeToAbsorb { get; private set; }

    // Cached references
    [SerializeField] private PlayerInput _pi;
    private Rigidbody2D _objectRb;

    private void Awake()
    {
        if (_pi == null) _pi = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        Player.instance.UpdateColor(Player.instance.ToLaunchObject);
    }

    private void OnEnable()
    {
        _pi.OnAbsorbPressed += PickUpObject;
        _pi.OnEjectPressed += EjectObject;
        _pi.OnObjectLaunchRequested += LaunchObject;
    }

    private void OnDisable()
    {
        _pi.OnAbsorbPressed -= PickUpObject;
        _pi.OnEjectPressed -= EjectObject;
    }

    public void PickUpObject()
    {
        if (objectToAbsorb == null) return;

        _objectRb = objectToAbsorb.GetComponent<Rigidbody2D>();
        Player.instance.Rb.mass = _objectRb.mass;

        Player.instance.SetPlayerScale(objectToAbsorb.transform.localScale + transform.localScale * 0.5f);

        objectToAbsorb.transform.SetParent(Player.instance.gameObject.transform, false);
        objectToAbsorb.transform.position = Vector3.zero;
        objectToAbsorb.SetActive(false);

        HasObject = true;
    }

    public void EjectObject()
    {
        if (!HasObject) return;

        Player.instance.ResetPlayer();

        objectToAbsorb.SetActive(true);
        objectToAbsorb.transform.SetParent(null, false);

        objectToAbsorb.transform.position =
            Player.instance.gameObject.transform.position +
            Player.instance.gameObject.transform.right * 1.5f;

        Player.instance.ToggleLaunchMode();
        HasObject = false;
    }

    public void LaunchObject(Vector3 force, float forceMultiplier)
    {
        if (!HasObject || objectToAbsorb == null) return;

        Vector3 objDir = force.normalized;
        Player.instance.ResetPlayer();

        objectToAbsorb.SetActive(true);
        objectToAbsorb.transform.SetParent(null, true);
        objectToAbsorb.transform.position = gameObject.transform.position + objDir;

        Rigidbody2D rb = objectToAbsorb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(force * forceMultiplier * _objectRb.mass, ForceMode2D.Impulse);
            rb.angularVelocity = Random.Range(-200f, 200f);
        }

        Player.instance.ToggleLaunchMode();
        HasObject = false;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Absorbable") && !HasObject)
        {
            InRangeToAbsorb = true;
            objectToAbsorb = collision.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        InRangeToAbsorb = false;
    }
}
