using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Execution : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] protected PlayerController _controller;

    void OnValidate()
    {
        if (_controller == null) _controller = GetComponentInParent<PlayerController>();
    }

    void OnEnable()
    {
        _controller.OnExecutionStarted.AddListener(Perform);
    }

    void OnDisable()
    {
        _controller.OnExecutionStarted.RemoveListener(Perform);
    }

    public virtual void Perform()
    {

    }
}
