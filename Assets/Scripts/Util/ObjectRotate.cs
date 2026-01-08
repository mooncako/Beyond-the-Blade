using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float _minSpeed = .5f;
    [SerializeField, BoxGroup("Settings")] private float _maxSpeed = 2f;
    [SerializeField, BoxGroup("Settings")] private float _minDuration = 2f;
    [SerializeField, BoxGroup("Settings")] private float _maxDuration = 4f;

    [SerializeField, BoxGroup("Debug"), ReadOnly] private Vector3 _currentDirection;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentSpeed;
    [SerializeField, BoxGroup("Debug"), ReadOnly] private float _currentDuration;

    void Update()
    {
        transform.Rotate(_currentDirection*_currentSpeed*Time.deltaTime);
    }

    private void Start()
    {
        StartCoroutine(ChangeRotationCO());
    }

    private IEnumerator ChangeRotationCO()
    {
        while (true)
        {
            _currentDuration = Random.Range(_minDuration, _maxDuration);
            _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
            _currentDirection = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)).normalized;
            yield return new WaitForSeconds(_currentDuration);
        }

    }

}
