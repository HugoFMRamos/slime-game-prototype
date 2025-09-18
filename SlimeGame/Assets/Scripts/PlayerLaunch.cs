using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerLaunch : MonoBehaviour
{
    public bool isLaunched = true;
    public float launchForceMultiplier = 5f;
    public LayerMask whatIsSurface;
    [SerializeField] private PlayerInput _pi;
    [SerializeField] private TrajectoryLine _tl;

    void Awake()
    {
        if (_pi == null) _pi = GetComponent<PlayerInput>();
        if (_tl == null) _tl = GameObject.FindWithTag("TrajectoryLine").GetComponent<TrajectoryLine>();
    }

    private void OnEnable()
    {
        _pi.OnLaunchStart += SetLaunchVariables;
        _pi.OnLaunchRequested += Launch;
        _pi.OnLaunchCancel += CancelLaunch;
        _pi.OnTrajectoryUpdate += ShowTrajectory;
        _pi.OnTrajectoryEnd += HideTrajectory;
    }

    private void OnDisable()
    {
        _pi.OnLaunchStart -= SetLaunchVariables;
        _pi.OnLaunchRequested -= Launch;
        _pi.OnLaunchCancel -= CancelLaunch;
        _pi.OnTrajectoryUpdate -= ShowTrajectory;
        _pi.OnTrajectoryEnd -= HideTrajectory;
    }

    private void SetLaunchVariables()
    {
        launchForceMultiplier = ForceCalculator.Map(0f, 1f, 3f, 5f, 1f / Player.instance.Rb.mass);
    }

    private void ShowTrajectory(Vector2 start, Vector2 end, float mass)
    {
        Vector2 forceApplied = ForceCalculator.CalculateForce(start, end, 10f) * launchForceMultiplier;

        _tl.Show();
        _tl.UpdateDots(transform.position, forceApplied, Player.instance.DefaultGravityScale);
    }

    private void HideTrajectory()
    {
        _tl.Hide();
    }

    public void AttachToSurface(bool isDynamicObject)
    {
        isLaunched = false;

        if (!isDynamicObject)
        {
            Player.instance.DisableGravity();
            return;
        }

        Player.instance.StopMovement();
    }

    public void Launch(Vector3 force)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, force.normalized, .2f, whatIsSurface);
        if (isLaunched && hit.collider != null) return;

        isLaunched = true;
        Player.instance.ResetGravity();

        Vector2 appliedForce = force * launchForceMultiplier * Player.instance.Rb.mass;
        Player.instance.Rb.AddForce(appliedForce, ForceMode2D.Impulse);
    }

    public void CancelLaunch()
    {
        if (isLaunched) return;

        HideTrajectory();
        isLaunched = false;

        Player.instance.DisableGravity();
    }
}
