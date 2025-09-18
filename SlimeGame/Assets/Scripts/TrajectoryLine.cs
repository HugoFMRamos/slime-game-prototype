using UnityEngine;

public class TrajectoryLine : MonoBehaviour
{
    [SerializeField] private int _dotsNumber = 30;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private float _dotSpacing = 0.1f;
    private Transform[] _dotsList;
    private bool _isVisible = false;

    void Awake()
    {
        PrepareDots();
        Hide();
    }

    private void PrepareDots()
    {
        _dotsList = new Transform[_dotsNumber];

        for (int i = 0; i < _dotsNumber; i++)
        {
            GameObject dot = Instantiate(_dotPrefab, transform);
            dot.SetActive(false);
            _dotsList[i] = dot.transform;
        }
    }

    public void UpdateDots(Vector3 startPos, Vector2 forceApplied, float gravityScale)
    {
        if (!_isVisible) Show();

        float timeStamp = _dotSpacing;

        for (int i = 0; i < _dotsNumber; i++)
        {
            Vector3 dotPos = startPos + new Vector3(
                forceApplied.x * timeStamp,
                forceApplied.y * timeStamp - Physics2D.gravity.magnitude * gravityScale * timeStamp * timeStamp / 2f,
                0f
            );

            _dotsList[i].position = dotPos;
            _dotsList[i].gameObject.SetActive(true);

            timeStamp += _dotSpacing;
        }
    }

    public void Show()
    {
        _isVisible = true;
        foreach (var dot in _dotsList)
            dot.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isVisible = false;
        foreach (var dot in _dotsList)
            dot.gameObject.SetActive(false);
    }
}

