//***************************************************************************************
// Writer: Stylish Esper
//***************************************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Esper.SkillWeb.Attributes
{
    /// <summary>
    /// Scales custom dataset fields that are marked with Skill Web's scaling attributes.
    /// </summary>
    public static class SkillScaler
    {
        /// <summary>
        /// Applies scaling to custom data fields.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="level">The current level of the skill..</param>
        /// <param name="maxLevel">The max level of the skill.</param>
        /// <exception cref="InvalidCastException">Unsupported numeric type.</exception>
        public static void ApplyScaling(ScriptableObject dataset, int level, int maxLevel)
        {
            var type = dataset.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var baseValues = new Dictionary<string, float>();
            var scalingSources = new Dictionary<string, (float? scaling, ScalingMode? mode, AnimationCurve curve)>();

            foreach (var field in fields)
            {
                var value = field.GetValue(dataset);

                // Base values
                if (field.GetCustomAttribute<BaseValueAttribute>() is BaseValueAttribute baseAttr)
                {
                    if (value is float f)
                    {
                        baseValues[baseAttr.id] = f;
                    }
                    else if (value is int i)
                    {
                        baseValues[baseAttr.id] = i;
                    }
                    else
                    {
                        throw new InvalidCastException($"Unsupported numeric type: {value?.GetType()}");
                    }
                }

                // Scaling values
                if (field.GetCustomAttribute<ScalingAttribute>() is ScalingAttribute scalingAttr)
                {
                    if (!scalingSources.TryGetValue(scalingAttr.id, out var entry))
                    {
                        entry = (null, null, null);
                    }

                    if (value is AnimationCurve curve)
                    {
                        entry.curve = curve;
                        entry.mode = ScalingMode.Curve;
                    }
                    else if (value is float f)
                    {
                        entry.scaling = f;

                        if (scalingAttr.mode == ScalingMode.Curve)
                        {
                            entry.mode = ScalingMode.Linear;
                        }
                        else
                        {
                            entry.mode = scalingAttr.mode;
                        }    
                    }
                    else if (value is int i)
                    {
                        entry.scaling = i;

                        if (scalingAttr.mode == ScalingMode.Curve)
                        {
                            entry.mode = ScalingMode.Linear;
                        }
                        else
                        {
                            entry.mode = scalingAttr.mode;
                        }
                    }
                    else
                    {
                        throw new InvalidCastException($"Unsupported numeric type: {value?.GetType()}");
                    }

                    scalingSources[scalingAttr.id] = entry;
                }
            }

            foreach (var field in fields)
            {
                var scaleAttr = field.GetCustomAttribute<ScalerAttribute>();
                if (scaleAttr == null)
                {
                    continue;
                }

                if (!baseValues.TryGetValue(scaleAttr.id, out var baseVal))
                {
                    continue;
                }

                scalingSources.TryGetValue(scaleAttr.id, out var source);

                float result = baseVal;

                switch (source.mode)
                {
                    case ScalingMode.Linear:
                    default:
                        if (source.scaling.HasValue)
                        {
                            result = baseVal + source.scaling.Value * Math.Max(level - 1, 0);
                        }
                        break;

                    case ScalingMode.Exponential:
                        if (source.scaling.HasValue)
                        {
                            result = baseVal * Mathf.Pow(source.scaling.Value, Math.Max(level - 1, 0));
                        }
                        break;

                    case ScalingMode.Curve:
                        var curve = source.curve;

                        if (curve != null)
                        {
                            result = curve.Evaluate((float)level / maxLevel);
                        }
                        break;
                }

                if (field.FieldType == typeof(float))
                {
                    field.SetValue(dataset, result);
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(dataset, Mathf.RoundToInt(result));
                }
            }
        }
    }
}