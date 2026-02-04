
using Sirenix.OdinInspector;
using UnityEngine;

public class SpawnPos : DrawPos
{
    void OnValidate()
    {
        _color = new Color(0.023f, 0.972f, 0.876f, 1.000f);
    }
}
