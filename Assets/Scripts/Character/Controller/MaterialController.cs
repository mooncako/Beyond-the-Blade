using PrimeTween;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct LightData
{
    public float3 direction;
    public float strength;
    public float3 color;
}

[BurstCompile]
public struct LightingComputeJob : IJobParallelFor
{
    [Unity.Collections.ReadOnly] public NativeArray<LightData> _inputLights;
    public NativeArray<LightData> _sortedLights;

    public void Execute(int index)
    {
        LightData main = default;
        LightData minor = default;
        LightData sub = default;

        float mainStrength = float.MinValue;
        float minorStrength = float.MinValue;
        float subStrength = float.MinValue;

        for (int i = 0; i < _inputLights.Length; i++)
        {
            LightData data = _inputLights[i];

            if (data.strength > mainStrength)
            {
                sub = minor;
                subStrength = minorStrength;

                minor = main;
                minorStrength = mainStrength;

                main = data;
                mainStrength = data.strength;
            }
            else if (data.strength > minorStrength)
            {
                sub = minor;
                subStrength = minorStrength;

                minor = data;
                minorStrength = data.strength;
            }
            else if (data.strength > subStrength)
            {
                sub = data;
                subStrength = data.strength;
            }
        }

        _sortedLights[0] = main;
        _sortedLights[1] = minor;
        _sortedLights[2] = sub;
    }
}



public class MaterialController : MonoBehaviour
{
    private enum LightType {
        Main,
        Minor,
        Sub
    }

    [SerializeField, BoxGroup("References")] private SkinnedMeshRenderer[] _skinnedMeshes;
    //[SerializeField, BoxGroup("References")] private Health _health;
    [SerializeField, BoxGroup("References")] private CanvasGroup _musoSelection;
    [SerializeField, BoxGroup("Settings")] private float _lightDetectionRadius = 40f;
    [SerializeField, BoxGroup("Settings")] private LayerMask _lightMask;
    [SerializeField, BoxGroup("Settings")] private float _fadeSpeed = 5f;

    [ShowInInspector] private Collider[] _colliders = new Collider[10]; 
    private Color _defaultMainLightColor;
    private Color _defaultMinorLightColor;
    private Color _defaultSubLightColor;
    private HashSet<UnlitLightingApplier> _lightingHashset = new HashSet<UnlitLightingApplier>();
    
    private Vector3 _mainLightDir = Vector3.zero;
    private float _mainLightStrength = 0;
    private Color _mainLightColor = Color.black;
    private UnlitLightingApplier _mainLight;

    private Vector3 _minorLightDir = Vector3.zero;
    private float _minorLightStrength = 0;
    private Color _minorLightColor = Color.black;
    private UnlitLightingApplier _minorLight;

    private Vector3 _subLightDir = Vector3.zero;
    private float _subLightStrength = 0;
    private Color _subLightColor = Color.black;
    private UnlitLightingApplier _subLight;


    private void OnValidate()
    {
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        //if (_health == null) _health = GetComponent<Health>();

        if ((_lightMask & (1 << 3)) == 0) // Check if _lightMask includes lighting layer, if no than add it.
        {
            _lightMask |= 1 << 3;
        }
    }

    private void Awake()
    {
        _defaultMainLightColor = Color.white;
        _defaultMinorLightColor = Color.white;
        _defaultSubLightColor = Color.white;

        if ((_lightMask & (1 << 3)) == 0) // Check if _lightMask includes lighting layer, if no than add it.
        {
            _lightMask |= 1 << 3;
        }
    }

    private void FixedUpdate()
    {
        
        int detectionNum = Physics.OverlapSphereNonAlloc(transform.position, _lightDetectionRadius, _colliders, _lightMask);
        if (detectionNum > 0)
        {
            for (int i = 0; i < detectionNum; i++)
            {
                UnlitLightingApplier lightingApplier = _colliders[i].GetComponent<UnlitLightingApplier>();
                if (lightingApplier != null)
                {
                    if (!_lightingHashset.Contains(lightingApplier))
                    {
                        _lightingHashset.Add(lightingApplier);
                    }
                }
            }
            foreach(UnlitLightingApplier applier in _lightingHashset.ToList())
            {
                if (!_colliders.Contains(applier.GetComponent<Collider>()))
                {
                    _lightingHashset.Remove(applier);
                    if (_mainLight == applier)
                    {
                        _mainLight = null;
                    }
                    else if (_minorLight == applier)
                    {
                        _minorLight = null;
                    }
                    else if (_subLight == applier)
                    {
                        _subLight = null;
                    }
                }
            }
        }
        

    }

    private void Update()
    {
        ApplyLighting();
    }

    private void OnEnable()
    {
        foreach (SkinnedMeshRenderer renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_OnDamage", 0);
            }
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        Clear();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        foreach (SkinnedMeshRenderer renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                SetMaterial(mat, Vector3.zero, _defaultMainLightColor, 0, LightType.Main);
                SetMaterial(mat, Vector3.zero, _defaultMinorLightColor, 0, LightType.Minor);
                SetMaterial(mat, Vector3.zero, _defaultSubLightColor, 0, LightType.Sub);
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Clear();
    }

    private void OnActiveSceneChanged(Scene scene, Scene newScene)
    {
        Clear();
    }

    private void Clear()
    {
        _colliders = new Collider[10];
        _lightingHashset.Clear();
        _mainLight = null;
        _minorLight = null;
        _subLight = null;
    }



    /*
    public void ApplyLighting()
    {

        if (_lightingHashset.Count > 0)
        {
            
            _mainLightDir = Vector3.zero;
            _mainLightStrength = 0;
            _mainLightColor = Color.white;
            _mainLight = null;

            _minorLightDir = Vector3.zero;
            _minorLightStrength = 0;
            _minorLightColor = Color.white;
            _minorLight = null;

            _subLightDir = Vector3.zero;
            _subLightStrength = 0;
            _subLightColor = Color.white;
            _subLight = null;
            

            foreach (UnlitLightingApplier light in _lightingHashset)
            {

                Vector3 dir = light.GetDirection(transform);
                float strength = light.GetStrength(transform);
                Color color = light.GetColor();
                if (_mainLight == null)
                {
                    _mainLight = light;
                    _mainLightDir = dir;
                    _mainLightStrength = strength;
                    _mainLightColor = color;
                }
                else
                {
                    if (strength > _mainLightStrength)
                    {

                        _subLightDir = _minorLightDir;
                        _subLightStrength = _minorLightStrength;
                        _subLightColor = _minorLightColor;

                        _minorLightDir = _mainLightDir;
                        _minorLightStrength = _mainLightStrength;
                        _minorLightColor = _mainLightColor;

                        _mainLightDir = dir;
                        _mainLightStrength = strength;
                        _mainLightColor = color;

                        if (_minorLight != null)
                        {
                            _subLight = _minorLight;
                        }

                        _minorLight = _mainLight;
                        _mainLight = light;
                    }
                    else if (strength > _minorLightStrength)
                    {
                        _subLightDir = _minorLightDir;
                        _subLightStrength = _minorLightStrength;
                        _subLightColor = _minorLightColor;

                        _minorLightDir = dir;
                        _minorLightStrength = strength;
                        _minorLightColor = color;

                        _subLight = _minorLight;
                        _minorLight = light;
                    }
                    else if (strength > _subLightStrength)
                    {
                        _subLightDir = dir;
                        _subLightStrength = strength;
                        _subLightColor = color;

                        _subLight = light;
                    }
                }

            }
        }

        _mainLightDir = Vector3.Slerp(_mainLightDir, _mainLight != null ? _mainLight.GetDirection(transform) : Vector3.zero, Time.deltaTime * _fadeSpeed);
        _mainLightStrength = Mathf.Lerp(_mainLightStrength, _mainLight != null ? _mainLight.GetStrength(transform) : 0f, Time.deltaTime * _fadeSpeed);
        _mainLightColor = Color.Lerp(_mainLightColor, _mainLight != null ? _mainLight.GetColor() : Color.black, Time.deltaTime * _fadeSpeed);

        _minorLightDir = Vector3.Slerp(_minorLightDir, _minorLight != null ? _minorLight.GetDirection(transform) : Vector3.zero, Time.deltaTime * _fadeSpeed);
        _minorLightStrength = Mathf.Lerp(_minorLightStrength, _minorLight != null ? _minorLight.GetStrength(transform) : 0f, Time.deltaTime * _fadeSpeed);
        _minorLightColor = Color.Lerp(_minorLightColor, _minorLight != null ? _minorLight.GetColor() : Color.black, Time.deltaTime * _fadeSpeed);

        _subLightDir = Vector3.Slerp(_subLightDir, _subLight != null ? _subLight.GetDirection(transform) : Vector3.zero, Time.deltaTime * _fadeSpeed);
        _subLightStrength = Mathf.Lerp(_subLightStrength, _subLight != null ? _subLight.GetStrength(transform) : 0f, Time.deltaTime * _fadeSpeed);
        _subLightColor = Color.Lerp(_subLightColor, _subLight != null ? _subLight.GetColor() : Color.black, Time.deltaTime * _fadeSpeed);


        foreach (SkinnedMeshRenderer renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                SetMaterial(mat, _mainLightDir, _mainLightColor, _mainLightStrength, LightType.Main);
                SetMaterial(mat, _minorLightDir, _minorLightColor, _minorLightStrength, LightType.Minor);
                SetMaterial(mat, _subLightDir, _subLightColor, _subLightStrength, LightType.Sub);
            }
        }
    }
    */

    public void ApplyLighting()
    {
        if (_lightingHashset.Count == 0) return;

        List<LightData> lightDataList = new List<LightData>();
        foreach(var light in _lightingHashset)
        {
            lightDataList.Add(new LightData
            {
                direction = light.GetDirection(transform),
                strength = light.GetStrength(transform),
                color = light.GetColorF3()
            });
        }

        NativeArray<LightData> input = new NativeArray<LightData>(lightDataList.ToArray(), Allocator.TempJob);
        NativeArray<LightData> output = new NativeArray<LightData>(3, Allocator.TempJob);

        var job = new LightingComputeJob
        {
            _inputLights = input,
            _sortedLights = output
        };
        job.Schedule(64, 64).Complete();

        LightData main = output[0];
        LightData minor = output[1];
        LightData sub = output[2];

        _mainLightDir = Vector3.Slerp(_mainLightDir, main.direction, Time.deltaTime * _fadeSpeed);
        _mainLightStrength = Mathf.Lerp(_mainLightStrength, main.strength, Time.deltaTime * _fadeSpeed);
        _mainLightColor = Color.Lerp(_mainLightColor, new Color(main.color.x, main.color.y, main.color.z), Time.deltaTime * _fadeSpeed);

        _minorLightDir = Vector3.Slerp(_minorLightDir, minor.direction, Time.deltaTime * _fadeSpeed);
        _minorLightStrength = Mathf.Lerp(_minorLightStrength, minor.strength, Time.deltaTime * _fadeSpeed);
        _minorLightColor = Color.Lerp(_minorLightColor, new Color(minor.color.x, minor.color.y, minor.color.z), Time.deltaTime * _fadeSpeed);

        _subLightDir = Vector3.Slerp(_subLightDir, sub.direction, Time.deltaTime * _fadeSpeed);
        _subLightStrength = Mathf.Lerp(_subLightStrength, sub.strength, Time.deltaTime * _fadeSpeed);
        _subLightColor = Color.Lerp(_subLightColor, new Color(sub.color.x, sub.color.y, sub.color.z), Time.deltaTime * _fadeSpeed);

        foreach(SkinnedMeshRenderer renderer in _skinnedMeshes)
        {
            foreach(Material mat in renderer.materials)
            {
                SetMaterial(mat, _mainLightDir, _mainLightColor, _mainLightStrength, LightType.Main);
                SetMaterial(mat, _minorLightDir, _minorLightColor, _minorLightStrength, LightType.Minor);
                SetMaterial(mat, _subLightDir, _subLightColor, _subLightStrength, LightType.Sub);
            }
        }

        input.Dispose();
        output.Dispose();
    }

    private void SetMaterial(Material mat, Vector3 dir, Color color, float strength, LightType type)
    {
        switch (type)
        {
            case LightType.Main:
                mat.SetVector("_LightDirection", dir);
                mat.SetColor("_LightColor", color);
                mat.SetFloat("_LightStrength", strength);
                break;
            case LightType.Minor:
                mat.SetVector("_MinorLightDirection", dir);
                mat.SetColor("_MinorLightColor", color);
                mat.SetFloat("_MinorLightStrength", strength);
                break;
            case LightType.Sub:
                mat.SetVector("_SubLightDirection", dir);
                mat.SetColor("_SubLightColor", color);
                mat.SetFloat("_SubLightStrength", strength);
                break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _lightDetectionRadius);
    }

}
