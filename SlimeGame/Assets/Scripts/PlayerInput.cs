using System;
using UnityEngine;

[RequireComponent(typeof(PlayerPickUp))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerInput : MonoBehaviour
{
    public KeyCode absorbKey = KeyCode.E;
    public KeyCode launchObjectKey = KeyCode.Alpha1;

    [SerializeField] private PlayerPickUp _ppu;   
    [SerializeField] private PlayerLaunch _pl;    
    [SerializeField] private Camera _cam; 

    private float _minDragDistance;
    private bool _isDragging = false;
    private Vector2 _mouseDownPos;
    private Vector2 _mouseUpPos;

    public event Action OnAbsorbPressed;
    public event Action OnEjectPressed;
    public event Action OnLaunchStart;
    public event Action<Vector3> OnLaunchRequested;
    public event Action<Vector3, float> OnObjectLaunchRequested;
    public event Action OnLaunchCancel;
    public event Action<Vector2, Vector2, float> OnTrajectoryUpdate;
    public event Action OnTrajectoryEnd;

    private void Awake()
    {
        if (_ppu == null) _ppu = GetComponent<PlayerPickUp>();
        if (_pl == null) _pl = GetComponent<PlayerLaunch>();
        if (_cam == null) _cam = Camera.main;

        _minDragDistance = GetComponent<BoxCollider2D>().size.x;
    }

    private void Update()
    {
        if (Input.GetKeyDown(absorbKey))
        {
            if (!_ppu.HasObject && _ppu.InRangeToAbsorb)
                OnAbsorbPressed?.Invoke();
            else
                OnEjectPressed?.Invoke();
        }

        if (Input.GetKeyDown(launchObjectKey) && _ppu.HasObject)
        {
            Player.instance.ToggleLaunchMode();
        }
    }

    void OnMouseDown()
    {
        _mouseDownPos = _cam.ScreenToWorldPoint(Input.mousePosition);
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;

        Vector2 currentPos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(_mouseDownPos, currentPos) < _minDragDistance)
        {
            OnTrajectoryEnd?.Invoke();  
            return;
        }

        OnLaunchStart?.Invoke();
        OnTrajectoryUpdate?.Invoke(_mouseDownPos, currentPos, Player.instance.Rb.mass);
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;

        _mouseUpPos = _cam.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(_mouseDownPos, _mouseUpPos) < _minDragDistance)
        {
            OnLaunchCancel?.Invoke();
            _isDragging = false;
            return;
        }

        Vector3 force = ForceCalculator.CalculateForce(_mouseDownPos, _mouseUpPos, 10f);

        if (!Player.instance.ToLaunchObject)
            OnLaunchRequested?.Invoke(force);
        else
            OnObjectLaunchRequested?.Invoke(force, _pl.launchForceMultiplier);

        OnTrajectoryEnd?.Invoke();
        _isDragging = false;
    }
}
