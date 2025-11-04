using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

public class PlayerHealthSensor : GlobalWorldSensorBase
{
    public int PlayerHealth;

    public override void Created()
    {
        PlayerHealth = 100;
    }

    public override SenseValue Sense()
    {
        return PlayerHealth;
    }
}
