using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class OvenStationVisual : MonoBehaviour
    {
        private static readonly int Door = Animator.StringToHash("Door");
        
        [SerializeField] private OvenStation ovenStation;
        [SerializeField] private Transform burnParticlesTransform;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            burnParticlesTransform.gameObject.SetActive(false);
        }

        private void Start()
        {
            ovenStation.OnPutIn += OvenStation_OnPutInOven;
            ovenStation.OnTakeOut += OvenStation_OnTakeOutOfOven;
            ovenStation.OnStateChanged += OvenStation_OnStateChanged;
        }

        private void OvenStation_OnStateChanged(object sender, OvenStation.State state)
        {
            burnParticlesTransform.gameObject.SetActive(state == OvenStation.State.Burning);
        }

        private void OvenStation_OnTakeOutOfOven(object sender, EventArgs e)
        {
            _animator.SetTrigger(Door);
        }

        private void OvenStation_OnPutInOven(object sender, EventArgs e)
        {
            _animator.SetTrigger(Door);
        }
        
        
    }
}