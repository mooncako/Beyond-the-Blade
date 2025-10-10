using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class MaterialController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private SkinnedMeshRenderer[] _skinnedMeshes;
    [SerializeField, BoxGroup("References")] private Health _health;
    [SerializeField, BoxGroup("References")] private Material _damageFlash;
    private List<Material> _defaultMaterials = new List<Material>();

    private Tween _delayTween;
    private Tween _iframeTween;

    void OnValidate()
    {
        _skinnedMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (_health == null) _health = GetComponentInParent<Health>();
    }

    void OnEnable()
    {
        if (_health != null)
        {
            _health.OnDamage.AddListener(OnDamage);
            _health.OnIframe.AddListener(OnIframe);
        }

        foreach (var renderer in _skinnedMeshes)
        {
            for(int i = 0; i < renderer.materials.Length; i++)
            {
                _defaultMaterials.Add(renderer.materials[i]);
            }
        }
    }

    void OnDisable()
    {
        if (_health != null)
        {
            _health.OnDamage.RemoveListener(OnDamage);
            _health.OnIframe.RemoveListener(OnIframe);
        }
        _delayTween.Stop();
        _iframeTween.Stop();
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_IsIframe", 0);
            }
        }
        Recover();
    }

    private void OnIframe(float duration)
    {
        _iframeTween.Stop();
        foreach (var renderer in _skinnedMeshes)
        {
            foreach (Material mat in renderer.materials)
            {
                mat.SetInt("_IsIframe", 1);
            }
        }
        _delayTween = Tween.Delay(duration).OnComplete(() =>
        {
            foreach (var renderer in _skinnedMeshes)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetInt("_IsIframe", 0);
                }
            }
        });
    }

    [Button]
    private void OnDamage(DamageInfo info)
    {
        _delayTween.Stop();
        foreach (var renderer in _skinnedMeshes)
        {
            int slots = renderer.sharedMesh ? renderer.sharedMesh.subMeshCount : renderer.sharedMaterials.Length;
            var mats = new Material[slots];
            for (int i = 0; i < slots; i++)
            {
                mats[i] = _damageFlash;
            }

            renderer.materials = mats;
        }

        _delayTween = Tween.Delay(.15f).OnComplete(Recover);
    }

    private void Recover()
    {
        int index = 0;
        foreach (var renderer in _skinnedMeshes)
        {
            int slots = renderer.sharedMesh ? renderer.sharedMesh.subMeshCount : renderer.sharedMaterials.Length;
            var mats = new Material[slots];
            for (int i = 0; i < slots; i++)
            {
                mats[i] = _defaultMaterials[index];
                index++;
            }

            renderer.materials = mats;
        }
    }

}
