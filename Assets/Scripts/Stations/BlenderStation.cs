using System;
using System.Linq;
using UnityEngine;

public class BlenderStation : MonoBehaviour, ICanBeInteracted, ICanBeInteractedAlt, ICanHold
{
    public EventHandler OnPutIn;
    public EventHandler OnTakeOut;
    public EventHandler<State> OnStateChanged;

    public enum State
    {
        Idle,
        Processing
    }
    
    [SerializeField] private Transform holdPoint;
    [SerializeField] private RecipesDictionarySo recipesDictionarySo;
    private HoldableObject _holdItem;
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
    private BlenderRecipeSo _blenderRecipeSo;
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
                    HoldableObject.SpawnHoldableObject(_blenderRecipeSo.output, this);
                    CheckForRecipe();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Interact()
    {
        if (CurrentState is not State.Idle) return;
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
        if (IsHoldingItem())
        {
            if (CurrentState == State.Idle)
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
        BlenderRecipeSo recipe = recipesDictionarySo.blenderRecipes.FirstOrDefault(r => r.input == GetHeldItem().HoldableObjectSo);
        if (recipe != null)
        {
            CurrentState = State.Processing;
            _timeToProcess = recipe.timeToProcess;
            _blenderRecipeSo = recipe;
        }
        else
        {
            CurrentState = State.Idle;
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
