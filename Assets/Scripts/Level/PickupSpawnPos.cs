using UnityEngine;

public class PickupSpawnPos : DrawPos
{
    void OnValidate()
    {
        _color = new Color(0.462f, 1.000f, 0.000f, 1.000f);
    }
}
