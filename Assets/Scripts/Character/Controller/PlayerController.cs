using System.Collections.Generic;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller, MMEventListener<PlayerAnimationStateChangeEvent>
{
    [field: SerializeField, FoldoutGroup("Base Reference")] private PlayerInput _input;

    [BoxGroup("Input")] public InputProcessor InputProcessor;
    [BoxGroup("Input"), ReadOnly] public Vector2 RotateInput { get; set; }
    [BoxGroup("Input")] public PlayerStateMachine StateMachine { get; private set; }
    [BoxGroup("Input")] public StateCollection States { get; private set; }
    [BoxGroup("Input"), ReadOnly] public PlayerStateType CurrentState { get; private set; }
    [BoxGroup("Input"), ReadOnly] public bool CanRotate = true;
    [BoxGroup("Input"), ReadOnly] private Vector3 _aimPoint;

    private Dictionary<PlayerActionType, bool> _availableActions = new Dictionary<PlayerActionType, bool>();

#if UNITY_EDITOR
    [Header("Current State")]
    [DisplayAsString, HideLabel, ShowInInspector] public string PlayerCurrentState => StateMachine?.CurrentState.ToString() ?? "None";
#endif


    protected override void OnValidate()
    {
        base.OnValidate();
        if(_input == null) _input = GetComponent<PlayerInput>();
    }

    protected override void Awake()
    {
        base.Awake();


        StateMachine = new PlayerStateMachine();
        States = new StateCollection(this, StateMachine);
        InputProcessor = new InputProcessor();

        foreach (PlayerActionType actionType in System.Enum.GetValues(typeof(PlayerActionType)))
        {
            _availableActions[actionType] = true;
        }
    }

    void Start()
    {
        StateMachine.Initialize(States.IdleState);
    }

    void Update()
    {

        HandleRotation();
        StateMachine.CurrentState.Update();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening<PlayerAnimationStateChangeEvent>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening<PlayerAnimationStateChangeEvent>();
    }

    public void OnMMEvent(PlayerAnimationStateChangeEvent e)
    {
        CurrentState = e.State;
    }

    private void HandleRotation()
    {
        if(Mathf.Approximately(Time.deltaTime, 0)) return;
        if(!CanRotate) return;

        if(_input.currentControlScheme == "Keyboard&Mouse")
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, transform.position);
            if (plane.Raycast(mouseRay, out float planeDistance))
            {
                _aimPoint = mouseRay.GetPoint(planeDistance);
                _aimPoint.y = transform.position.y;
                Movement.SetLookPosition(_aimPoint);
                //_arrow.transform.LookAt(_aimPoint);
            }
        }else if(_input.currentControlScheme == "Gamepad")
        {
            //TODO
        }
    }



    public void InputMovement(InputAction.CallbackContext context)
    {
        // Always process the input vector, regardless of action availability
        // This ensures we're always capturing the latest input
        Vector2 inputValue = context.ReadValue<Vector2>();

        // Store the input in the InputProcessor
        InputProcessor.ProcessInputVector(inputValue);

        // Only apply movement if the action is available
        InputProcessor.SetInputActive(IsActionAvailable(PlayerActionType.Move));
    }

    public void InputRotate(InputAction.CallbackContext context)
    {

        Vector2 inputValue = context.ReadValue<Vector2>();

        RotateInput = inputValue;
    }

    public void InputParry(InputAction.CallbackContext context)
    {
        if (context.started && IsActionAvailable(PlayerActionType.Parry))
        {
            TryParry();
        }
    }

    public void InputPauseUnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (Time.timeScale > .5f)
            {
                //Pause
                Time.timeScale = 0;
            }
            else
            {
                //Resume
                Time.timeScale = 1;
            }
        }
    }


    public void SetActionAvailable(PlayerActionType actionType, bool available)
    {
        _availableActions[actionType] = available;
    }

    private bool IsActionAvailable(PlayerActionType actionType)
    {
        return _availableActions.TryGetValue(actionType, out bool available) && available;
    }

    private bool TryParry()
    {
        return false;
    }

    
}
