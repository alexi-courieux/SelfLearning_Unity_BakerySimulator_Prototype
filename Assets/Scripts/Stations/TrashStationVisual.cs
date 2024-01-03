using UnityEngine;

public class TrashStationVisual : MonoBehaviour
{
    [SerializeField] private TrashStation trashStation;
    [SerializeField] private Transform emptyPosition;
    [SerializeField] private Transform fullPosition;
    [SerializeField] private Transform trashVisual;
    private int _trashAmountMax;
    private int _trashAmount;
    private void Start()
    {
        trashStation.OnTrashAmountChanged += TrashStation_OnTrashAmountChanged;
        _trashAmount = 0;
        _trashAmountMax = trashStation.GetTrashAmountMax();
        
        UpdateVisuals();
    }

    private void OnDestroy()
    {
        trashStation.OnTrashAmountChanged -= TrashStation_OnTrashAmountChanged;
    }

    private void TrashStation_OnTrashAmountChanged(object sender, int newAmount)
    {
        _trashAmount = newAmount;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        trashVisual.gameObject.SetActive(_trashAmount > 0);
        if (_trashAmount == 0) return;
        
        float trashPercentage = _trashAmount / (float)_trashAmountMax;
        float trashHeight = Mathf.Lerp(emptyPosition.position.y, fullPosition.position.y, trashPercentage);
        Vector3 position = trashVisual.position;
        position = new Vector3(position.x, trashHeight, position.z);
        trashVisual.position = position;
    }
}