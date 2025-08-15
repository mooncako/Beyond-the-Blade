#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AnimationStateController))]
public class AnimationStateControllerEditor : Editor
{
    // Movement settings
    private SerializedProperty _applyRootMotion;
    private SerializedProperty _canMove;
    private SerializedProperty _speedMultiplier;
    
    // Action availability
    private SerializedProperty _canAttack;
    private SerializedProperty _canParry;
    private SerializedProperty _canMuso;
    
    // State management
    private SerializedProperty _stateType;
    private SerializedProperty _stateEnterEvent;
    private SerializedProperty _stateExitEvent;
    
    // Animator parameters
    private SerializedProperty _resetTriggersOnEnter;
    private SerializedProperty _resetTriggersOnExit;
    private SerializedProperty _setBoolsOnEnter;
    private SerializedProperty _boolValuesOnEnter;
    private SerializedProperty _setBoolsOnExit;
    private SerializedProperty _boolValuesOnExit;
    
    // Combat controller
    private SerializedProperty _resetIsAttackingOnExit;
    private SerializedProperty _resetIsParryingOnExit;
    private SerializedProperty _resetIsChargingOnExit;
    private SerializedProperty _resetIsStaggeredOnExit;
    private SerializedProperty _resetMusoReadyOnExit;
    
    // Input processing
    private SerializedProperty _reactivateMovementInputOnExit;
    
    // Foldout states
    private bool _showMovementSettings = true;
    private bool _showActionAvailability = true;
    private bool _showStateManagement = true;
    private bool _showAnimatorParameters = true;
    private bool _showCombatController = true;
    private bool _showInputProcessing = true;

    private void OnEnable()
    {
        // Movement settings
        _applyRootMotion = serializedObject.FindProperty("_applyRootMotion");
        _canMove = serializedObject.FindProperty("_canMove");
        _speedMultiplier = serializedObject.FindProperty("_speedMultiplier");

        // Action availability
        _canAttack = serializedObject.FindProperty("_canAttack");
        _canParry = serializedObject.FindProperty("_canParry");
        _canMuso = serializedObject.FindProperty("_canMuso");

        // State management
        _stateType = serializedObject.FindProperty("_stateType");
        _stateEnterEvent = serializedObject.FindProperty("_stateEnterEvent");
        _stateExitEvent = serializedObject.FindProperty("_stateExitEvent");

        // Animator parameters
        _resetTriggersOnEnter = serializedObject.FindProperty("_resetTriggersOnEnter");
        _resetTriggersOnExit = serializedObject.FindProperty("_resetTriggersOnExit");
        _setBoolsOnEnter = serializedObject.FindProperty("_setBoolsOnEnter");
        _boolValuesOnEnter = serializedObject.FindProperty("_boolValuesOnEnter");
        _setBoolsOnExit = serializedObject.FindProperty("_setBoolsOnExit");
        _boolValuesOnExit = serializedObject.FindProperty("_boolValuesOnExit");

        // Combat controller
        _resetIsAttackingOnExit = serializedObject.FindProperty("_resetIsAttackingOnExit");
        _resetIsParryingOnExit = serializedObject.FindProperty("_resetIsParryingOnExit");
        _resetIsChargingOnExit = serializedObject.FindProperty("_resetIsChargingOnExit");
        _resetIsStaggeredOnExit = serializedObject.FindProperty("_resetIsStaggeredOnExit");
        _resetMusoReadyOnExit = serializedObject.FindProperty("_resetMusoReadyOnExit");

        // Input processing
        _reactivateMovementInputOnExit = serializedObject.FindProperty("_reactivateMovementInputOnExit");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Movement Settings
        _showMovementSettings = EditorGUILayout.Foldout(_showMovementSettings, "Movement Settings", true);
        if (_showMovementSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_applyRootMotion);
            EditorGUILayout.PropertyField(_canMove);
            EditorGUILayout.PropertyField(_speedMultiplier);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Action Availability
        _showActionAvailability = EditorGUILayout.Foldout(_showActionAvailability, "Action Availability", true);
        if (_showActionAvailability)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_canAttack);
            EditorGUILayout.PropertyField(_canParry);
            EditorGUILayout.PropertyField(_canMuso);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // State Management
        _showStateManagement = EditorGUILayout.Foldout(_showStateManagement, "State Management", true);
        if (_showStateManagement)
        {
            EditorGUI.indentLevel++;
            
            // Display a dropdown for state type
            string[] stateNames = new string[] {
                "Idle", "Moving", "Attacking", "Charging", 
                "Dead", "Parrying", "Staggered", "Muso", "Sheathe"
            };
            
            int currentState = _stateType.intValue;
            int newState = EditorGUILayout.Popup("State Type", currentState, stateNames);
            if (newState != currentState)
            {
                _stateType.intValue = newState;
            }
            
            EditorGUILayout.PropertyField(_stateEnterEvent);
            EditorGUILayout.PropertyField(_stateExitEvent);
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Animator Parameters
        _showAnimatorParameters = EditorGUILayout.Foldout(_showAnimatorParameters, "Animator Parameters", true);
        if (_showAnimatorParameters)
        {
            EditorGUI.indentLevel++;
            
            // Reset Triggers
            EditorGUILayout.PropertyField(_resetTriggersOnEnter, new GUIContent("Reset Triggers On Enter"));
            EditorGUILayout.PropertyField(_resetTriggersOnExit, new GUIContent("Reset Triggers On Exit"));
            
            // Set Bools On Enter
            EditorGUILayout.LabelField("Set Bools On Enter", EditorStyles.boldLabel);
            
            // Ensure arrays are the same length
            if (_setBoolsOnEnter.arraySize != _boolValuesOnEnter.arraySize)
            {
                _boolValuesOnEnter.arraySize = _setBoolsOnEnter.arraySize;
            }
            
            for (int i = 0; i < _setBoolsOnEnter.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_setBoolsOnEnter.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUILayout.PropertyField(_boolValuesOnEnter.GetArrayElementAtIndex(i), GUIContent.none);
                
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _setBoolsOnEnter.DeleteArrayElementAtIndex(i);
                    _boolValuesOnEnter.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Add Bool Parameter (Enter)"))
            {
                _setBoolsOnEnter.arraySize++;
                _boolValuesOnEnter.arraySize++;
            }
            
            EditorGUILayout.Space();
            
            // Set Bools On Exit
            EditorGUILayout.LabelField("Set Bools On Exit", EditorStyles.boldLabel);
            
            // Ensure arrays are the same length
            if (_setBoolsOnExit.arraySize != _boolValuesOnExit.arraySize)
            {
                _boolValuesOnExit.arraySize = _setBoolsOnExit.arraySize;
            }
            
            for (int i = 0; i < _setBoolsOnExit.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_setBoolsOnExit.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUILayout.PropertyField(_boolValuesOnExit.GetArrayElementAtIndex(i), GUIContent.none);
                
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _setBoolsOnExit.DeleteArrayElementAtIndex(i);
                    _boolValuesOnExit.DeleteArrayElementAtIndex(i);
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Add Bool Parameter (Exit)"))
            {
                _setBoolsOnExit.arraySize++;
                _boolValuesOnExit.arraySize++;
            }
            
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Combat Controller
        _showCombatController = EditorGUILayout.Foldout(_showCombatController, "Combat Controller", true);
        if (_showCombatController)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_resetIsAttackingOnExit);
            EditorGUILayout.PropertyField(_resetIsParryingOnExit);
            EditorGUILayout.PropertyField(_resetIsChargingOnExit);
            EditorGUILayout.PropertyField(_resetIsStaggeredOnExit);
            EditorGUILayout.PropertyField(_resetMusoReadyOnExit);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Input Processing
        _showInputProcessing = EditorGUILayout.Foldout(_showInputProcessing, "Input Processing", true);
        if (_showInputProcessing)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_reactivateMovementInputOnExit);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
