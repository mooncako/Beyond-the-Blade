using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class ShadowPassEnabler : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private CustomPassVolume _volume;

    void OnValidate()
    {
        if (_volume == null)
        {
            _volume = GetComponent<CustomPassVolume>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _volume.enabled = true;
    }
}
