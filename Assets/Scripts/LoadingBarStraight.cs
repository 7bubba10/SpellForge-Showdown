using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modularify.LoadingBars3D
{
    public class LoadingBarStraight : MonoBehaviour
    {
        public static LoadingBarStraight Instance;

        [Header("Inner Part Reference")]
        [SerializeField] private GameObject _innerPart;

        private Material _innerPartMaterial;

        [Header("Loading Bar Settings and Colors")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float _percentage = 0.5f;

        [ColorUsage(true, true)]
        [SerializeField] private Color _emptyColor = Color.red;

        [ColorUsage(true, true)]
        [SerializeField] private Color _fullColor = Color.green;

        private void Awake()
        {
            Instance = this;
            Initialize();
        }

        private void Update()
        {
            _innerPartMaterial.SetFloat("Percentage", _percentage);
        }

        public void SetPercentage(float percentage)
        {
            _percentage = Mathf.Clamp01(percentage);
        }

        public void Initialize()
        {
            _innerPartMaterial = new Material(_innerPart.GetComponent<MeshRenderer>().sharedMaterial);
            _innerPart.GetComponent<MeshRenderer>().material = _innerPartMaterial;

            _innerPartMaterial.SetFloat("Percentage", _percentage);
            _innerPartMaterial.SetColor("SideColorEmpty", _emptyColor);
            _innerPartMaterial.SetColor("SideColorFull", _fullColor);
        }
    }
}
