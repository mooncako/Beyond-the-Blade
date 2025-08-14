using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputProcessor))]
public class PlayerController : Controller
{
    [SerializeField, FoldoutGroup("Base Reference")] private InputProcessor _inputProcessor;

    private Dictionary<PlayerActionType, bool> _availableActions = new Dictionary<PlayerActionType, bool>();


    protected override void OnValidate()
    {
        base.OnValidate();
        if (_inputProcessor == null) _inputProcessor = GetComponent<InputProcessor>();
    }

    protected override void Awake()
    {
        base.Awake();

        foreach (PlayerActionType actionType in System.Enum.GetValues(typeof(PlayerActionType)))
        {
            _availableActions[actionType] = true;
        }
    }

    public void InputMovement(InputAction.CallbackContext context)
    {
        // Always process the input vector, regardless of action availability
        // This ensures we're always capturing the latest input
        Vector2 inputValue = context.ReadValue<Vector2>();

        // Store the input in the InputProcessor
        _inputProcessor.ProcessInputVector(inputValue);

        // Only apply movement if the action is available
        _inputProcessor.SetInputActive(IsActionAvailable(PlayerActionType.Move));
    }


    public void SetActionAvailable(PlayerActionType actionType, bool available)
    {
        _availableActions[actionType] = available;
    }

    private bool IsActionAvailable(PlayerActionType actionType)
    {
        return _availableActions.TryGetValue(actionType, out bool available) && available;
    }
}
