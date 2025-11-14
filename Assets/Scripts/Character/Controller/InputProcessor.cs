using UnityEngine;

public class InputProcessor
{
    private Vector2 _inputVector;
    private bool _isInputActive = true;
    
    public Vector2 InputVector => _isInputActive ? _inputVector : Vector2.zero;
    public Vector2 RawInputVector => _inputVector; // Always returns the actual input
    public Vector2 InputVectorNormalized => InputVector.normalized;

    public InputProcessor()
    {
    }

    public void ProcessInputVector(Vector2 value)
    {
        // Always store the input value, regardless of whether it's active
        _inputVector = value;
        if(_isInputActive)
        {
            CameraRotateEvent.Trigger(value);
        }
    }
    
    public void SetInputActive(bool active)
    {
        _isInputActive = active;
    }
}
