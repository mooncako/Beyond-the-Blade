using Sirenix.OdinInspector;
using UnityEngine;

public class Energy : MonoBehaviour
{
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _maxEnergy;
    [SerializeField, BoxGroup("Stats"), ReadOnly] private float _energy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
