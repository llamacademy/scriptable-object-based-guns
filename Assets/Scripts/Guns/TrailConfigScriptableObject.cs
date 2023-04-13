using System;
using UnityEngine;

namespace LlamAcademy.Guns
{
    [CreateAssetMenu(fileName = "Trail Config", menuName = "Guns/Trail Config", order = 4)]
    public class TrailConfigScriptableObject : ScriptableObject, ICloneable
    {
        public Material Material;
        public AnimationCurve WidthCurve;
        public float Duration = 0.5f;
        public float MinVertexDistance = 0.1f;
        public Gradient Color;

        public float MissDistance = 100f;
        public float SimulationSpeed = 100f;

        public object Clone()
        {
            TrailConfigScriptableObject config = CreateInstance<TrailConfigScriptableObject>();

            Utilities.CopyValues(this, config);

            return config;
        }
    }
}