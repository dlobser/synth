using UnityEngine;
using System.Collections.Generic;

namespace ON.synth
{
    public class Plug_ShaderVariable : MonoBehaviour
    {
        public enum ShaderPropertyType { Float, Vector, Color }

        public Oscillator oscillator;
        public Trigger trigger;
        public GameObject target;
        public bool affectChildren;
        public string channel;
        public ShaderPropertyType propertyType;

        public float minFloat = 0f;
        public float maxFloat = 1f;
        public Vector4 minVector = Vector4.zero;
        public Vector4 maxVector = Vector4.one;
        public Color minColor = Color.black;
        public Color maxColor = Color.white;

        private List<Material> materials;

        private void Start()
        {
            InitializeMaterials();
        }

        private void InitializeMaterials()
        {
            materials = new List<Material>();
            if(target != null){
                if (affectChildren)
                {
                    var renderers = target.GetComponentsInChildren<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        materials.AddRange(renderer.materials);
                    }
                }
                else
                {
                    var renderer = target.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        materials.AddRange(renderer.materials);
                    }
                }
            }
        }

        void Update()
        {
            float value = Synth_Util.GetOscTrigValue(oscillator, trigger);
            // if(materials == null || materials.Count==0)
                InitializeMaterials();
            // else
                ApplyShaderProperty(value);
        }

        void ApplyShaderProperty(float controlValue)
        {
            foreach (var mat in materials)
            {
                switch (propertyType)
                {
                    case ShaderPropertyType.Float:
                        float floatValue = Mathf.Lerp(minFloat, maxFloat, controlValue);
                        mat.SetFloat(channel, floatValue);
                        break;

                    case ShaderPropertyType.Vector:
                        Vector4 vectorValue = Vector4.Lerp(minVector, maxVector, controlValue);
                        mat.SetVector(channel, vectorValue);
                        break;

                    case ShaderPropertyType.Color:
                        Color colorValue = Color.Lerp(minColor, maxColor, controlValue);
                        mat.SetColor(channel, colorValue);
                        break;
                }
            }
        }
    }
}
