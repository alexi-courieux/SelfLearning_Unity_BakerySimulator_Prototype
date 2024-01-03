using UnityEngine;

public class BlenderStationVisual : MonoBehaviour
    {
        private static readonly int Processing = Animator.StringToHash("Processing");

        [SerializeField] private BlenderStation blenderStation;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            blenderStation.OnStateChanged += BlenderStation_OnStateChanged;
        }

        private void BlenderStation_OnStateChanged(object sender, BlenderStation.State state)
        {
            _animator.SetBool(Processing, state == BlenderStation.State.Processing);
        }
    }