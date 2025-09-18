using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; }
    public float DefaultGravityScale { get; private set; }
    public bool ToLaunchObject { get; private set; }

    [SerializeField] private Rigidbody2D _rb;
    public Rigidbody2D Rb => _rb;
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color launchObjectColor;
    [SerializeField] private float transitionTime = 0.1f;

    private Color currentTargetColor;
    private Color startColor;
    private float transitionTimer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;

        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_sr == null) _sr = GetComponentInChildren<SpriteRenderer>();

        DefaultGravityScale = _rb.gravityScale;

        _sr.color = defaultColor;
        currentTargetColor = defaultColor;
        startColor = defaultColor;
        transitionTimer = transitionTime;
    }

    public void ToggleLaunchMode()
    {
        ToLaunchObject = !ToLaunchObject;
    }

    public void ResetGravity()
    {
        _rb.gravityScale = DefaultGravityScale;
    }

    public void DisableGravity()
    {
        _rb.gravityScale = 0f;
    }

    public void StopMovement()
    {
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }

    public void SetPlayerScale(Vector3 newScale)
    {
        transform.localScale = new Vector3(newScale.x, newScale.y, 1f);
    }

    public void ResetPlayer()
    {
        transform.localScale = Vector3.one;
        _rb.mass = 1f;
        _rb.gravityScale = 10f;
    }

    public void UpdateColor(bool launchMode)
    {
        Color desiredColor = launchMode ? launchObjectColor : defaultColor;

        if (desiredColor != currentTargetColor)
        {
            startColor = _sr.color;
            currentTargetColor = desiredColor;
            transitionTimer = 0f;
        }

        transitionTimer += Time.deltaTime / transitionTime;
        _sr.color = Color.Lerp(startColor, currentTargetColor, transitionTimer);
    }
}
