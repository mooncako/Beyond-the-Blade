using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class UnlitLightingApplier : MonoBehaviour
{


    [SerializeField, BoxGroup("References")] private CapsuleCollider _collider;
    [SerializeField, BoxGroup("References")] private Light _light;

    [SerializeField, BoxGroup("Settings")] private Color _lightColor;
    //[SerializeField, BoxGroup("Settings"), Range(1, 2)] private float _lightStrength = 1;
    [SerializeField, BoxGroup("Settings")] private float _falloffDistance = 1.5f;
    [SerializeField, BoxGroup("Settings")] private float _falloffAngle = 1f;
    [SerializeField, BoxGroup("Settings")] private float _lightStrengthOffset = 0f;

    [ShowInInspector, ReadOnly] private float _currentDistance;
    [ShowInInspector, ReadOnly] private float _currentAngle;
    public float LightStrength;

    private void OnValidate()
    {
        if (_collider == null)
        {
            _collider = GetComponent<CapsuleCollider>();
            _collider.isTrigger = true;
        }

        if (_light == null)
        {
            _light = GetComponent<Light>();
            if (_light != null)
            {
                _lightColor = _light.color;
                CalculateIntensity();
            }
                
        }

    }


    private void Start()
    {
        if(_light != null)
        {
            _lightColor = _light.color * Mathf.CorrelatedColorTemperatureToRGB(_light.colorTemperature);
            CalculateIntensity();
        }
           
    }

    public float GetStrength(Transform other)
    {
        float dynamicStrength = 0;


        if (_light != null)
        {
            switch (_light.type)
            {
                case LightType.Spot:
                    float outerAngle = _light.spotAngle / 2;

                    _currentAngle = Vector3.Angle(GetDirection(other), transform.forward);
                    float anglePercentage = Mathf.InverseLerp(0, outerAngle + _falloffAngle, _currentAngle);
                    dynamicStrength = Mathf.Lerp(LightStrength + _lightStrengthOffset, 0, anglePercentage);
                    dynamicStrength = Mathf.Clamp(dynamicStrength, 0, 1);
                    return dynamicStrength;
                default:

                    _currentDistance = Vector3.Distance(transform.position, other.position);
                    _currentDistance = Mathf.Clamp(_currentDistance - _collider.radius, 0, _falloffDistance);
                    float distancePercentage = Mathf.InverseLerp(0, _falloffDistance, _currentDistance);
                    dynamicStrength = Mathf.Lerp(LightStrength + _lightStrengthOffset, 0, distancePercentage);

                    dynamicStrength = Mathf.Clamp(dynamicStrength, 0, 1);
                    return dynamicStrength;
            }
        }
        else
        {
            _currentDistance = Vector3.Distance(transform.position, other.position);
            _currentDistance = Mathf.Clamp(_currentDistance - _collider.radius, 0, _falloffDistance);
            float distancePercentage = Mathf.InverseLerp(0, _falloffDistance, _currentDistance);
            dynamicStrength = Mathf.Lerp(LightStrength + _lightStrengthOffset, 0, distancePercentage);

            dynamicStrength = Mathf.Clamp(dynamicStrength, 0, 1);
            return dynamicStrength;
        }

        
    }


    public Color GetColor()
    {
        return _lightColor;
    }

    public float3 GetColorF3()
    {
        return new float3(_lightColor.r, _lightColor.g, _lightColor.b);
    }

    public Vector3 GetDirection(Transform other)
    {
        return other.position - transform.position;
    }

    public float GetDistance(Transform other)
    {
        return Vector3.Distance(transform.position, other.position);
    }

    [Button]
    private void CalculateIntensity()
    {
        float originalIntensity = 0;
        float lightStrengthPercentile = 0;

        switch (_light.type)
        {
            case LightType.Spot:
                originalIntensity = Mathf.Clamp(_light.intensity, 0, LIGHTING.MaxCandelaIntensity); // 467083800 Candela
                lightStrengthPercentile = Mathf.InverseLerp(0, LIGHTING.MaxCandelaIntensity, originalIntensity);
                LightStrength = Mathf.Lerp(0, LIGHTING.MaxAppliedBrightness, lightStrengthPercentile);
                break;
            case LightType.Point:
                originalIntensity = Mathf.Clamp(_light.intensity, 0, LIGHTING.MaxCandelaIntensity); // 467083800 Candela
                lightStrengthPercentile = Mathf.InverseLerp(0, LIGHTING.MaxCandelaIntensity, originalIntensity);
                LightStrength = Mathf.Lerp(0, LIGHTING.MaxAppliedBrightness, lightStrengthPercentile);
                break;
            default:
                originalIntensity = Mathf.Clamp(_light.intensity, 0, LIGHTING.MaxNitsIntensity); // 10000000 nits
                lightStrengthPercentile = Mathf.InverseLerp(0, LIGHTING.MaxNitsIntensity, originalIntensity);
                LightStrength = Mathf.Lerp(0, LIGHTING.MaxAppliedBrightness, lightStrengthPercentile);
                break;
        }
        
    }


}
