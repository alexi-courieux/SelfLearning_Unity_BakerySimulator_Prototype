using System;
using System.Linq;
using UnityEngine;

public class OvenStation : MonoBehaviour, ICanBeInteracted, ICanBeInteractedAlt, ICanHold
{
    public EventHandler OnPutIn;
    public EventHandler OnTakeOut;
    public EventHandler<State> OnStateChanged;

    public enum State
    {
        Idle,
        Processing,
        Burning
    }
    
    [SerializeField] private Transform holdPoint;
    [SerializeField] private OvenRecipesDictionarySo ovenRecipesDictionarySo;
    [SerializeField] private HoldableObjectSo trashObject;
    private HoldableObject _holdItem;
    private bool _active;
    private State _state;
    private State CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged?.Invoke(this, _state);
        }
    }
    private OvenRecipeSo _ovenRecipeSo;
    private float _timeToProcess;
    
    private void Update()
    {
        switch (CurrentState)
        {
            case State.Idle:
                break;
            case State.Processing:
                _timeToProcess -= Time.deltaTime;
                if (_timeToProcess <= 0f)
                {
                    GetHeldItem().DestroySelf();
                    if (_ovenRecipeSo.burnt)
                    {
                        CurrentState = State.Burning;
                        HoldableObject.SpawnHoldableObject(trashObject, this);
                    }
                    else
                    {
                        HoldableObject.SpawnHoldableObject(_ovenRecipeSo.output, this);
                        CheckForRecipe();
                    }
                }
                break;
            case State.Burning:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Interact()
    {
        if (_active) return;
        if (IsHoldingItem())
        {
            if (!Player.Instance.HoldSystem.IsHoldingItem())
            {
                _holdItem.SetParent(Player.Instance.HoldSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (Player.Instance.HoldSystem.IsHoldingItem())
            {
                Player.Instance.HoldSystem.GetHeldItem().SetParent(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void InteractAlt()
    {
        if (GetHeldItem() != null)
        {
            _active = !_active;
            if (_active)
            {
                CheckForRecipe();
            }
            else
            {
                CurrentState = State.Idle;
            }
        }
        
    }

    private void CheckForRecipe()
    {
        OvenRecipeSo recipe = ovenRecipesDictionarySo.recipes.FirstOrDefault(r => r.input == GetHeldItem().HoldableObjectSo);
        if (recipe != null)
        {
            CurrentState = State.Processing;
            _timeToProcess = recipe.timeToProcess;
            _ovenRecipeSo = recipe;
        }
        else
        {
            _active = false;
        }
    }

    public void SetHeldItem(HoldableObject holdableObject)
    {
        _holdItem = holdableObject;
    }

    public HoldableObject GetHeldItem()
    {
        return _holdItem;
    }
    
    public void ClearHeldItem()
    {
        _holdItem = null;
    }

    public bool IsHoldingItem()
    {
        return _holdItem != null;
    }

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}
