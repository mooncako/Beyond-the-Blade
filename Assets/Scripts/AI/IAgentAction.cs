using System;
using UnityEngine;

public interface IAgentAction
{
    public void Execute();
    public void Update(float deltaTime);
    public event Action OnActionStarted;
    public event Action OnActionEnded;
}
