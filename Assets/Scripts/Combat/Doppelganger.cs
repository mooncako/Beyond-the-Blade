using Sirenix.OdinInspector;
using UnityEngine;

public class Doppelganger : MonoBehaviour, IPoolable
{
    [SerializeField, BoxGroup("Debug"), ReadOnly] public string SkillId;
    [SerializeField, BoxGroup("Debug"), ReadOnly] public Weapon Weapon;


    public void OnPoolGet()
    {

    }

    public void OnPoolReturn()
    {
        
    }
}
