using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Custom/Distorted Screen Outline")]
public sealed class DistortedOutlineSettings : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter outlineWidth = new(0f, 0f, 20f);
    public ColorParameter outlineColor = new(Color.black, true, true, true);

    public TextureParameter distortTex = new(null);
    public FloatParameter distort = new(0f);
    public FloatParameter distortSpeed = new(0f);

    public BoolParameter enable = new(true);

    public bool IsActive() => enable.value && outlineWidth.value > 0f;
    public bool IsTileCompatible() => false;
}
