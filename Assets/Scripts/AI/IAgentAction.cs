using System;
using UnityEngine;

public interface IAgentAction
{
    public void Execute(Agent agent);
    public void Stop(Agent agent);
    public void Update(Agent agent, float deltaTime);
    public event Action OnActionStarted;
    public event Action OnActionEnded;
}
