using UnityEngine;

public class InputProcessor
{
    private Vector2 inputVector;
    private bool isInputActive = true;
    
    public Vector2 InputVector => isInputActive ? inputVector : Vector2.zero;
    public Vector2 RawInputVector => inputVector; // Always returns the actual input
    public Vector2 InputVectorNormalized => InputVector.normalized;

    public InputProcessor()
    {
    }

    public void ProcessInputVector(Vector2 value)
    {
        // Always store the input value, regardless of whether it's active
        inputVector = value;
    }
    
    public void SetInputActive(bool active)
    {
        isInputActive = active;
    }
}
