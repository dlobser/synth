using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ON.synth
{
    public class Plug_Color : MonoBehaviour
    {
        public GameObject target;
        public Oscillator oscillator;
        public Trigger trigger;

        public Gradient gradient;

        public SpriteRenderer spriteRenderer;
        public TextMesh textMesh;
        public Image image;
        public MeshRenderer meshRenderer;
        public string colorChannelName;


        float value;
        Color color;

        // Start is called before the first frame update
        void Start()
        {
            if (target == null)
            {
                target = this.gameObject;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (oscillator != null)
            {
                value = oscillator.GetValue();
                if (trigger != null)
                    value *= trigger.GetValue();
            }
            else if (trigger != null)
                value = trigger.GetValue();

            color = gradient.Evaluate(value);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            if (image != null)
            {
                image.color = color;
            }
            if (meshRenderer != null)
            {
                if (colorChannelName.Length > 0)
                    meshRenderer.sharedMaterial.SetColor(colorChannelName, color);
                else
                    meshRenderer.sharedMaterial.color = color;
            }
            if (textMesh != null)
            {
                textMesh.color = color;
            }

        }

        public Color GetColor()
        {
            return color;
        }
    }
}