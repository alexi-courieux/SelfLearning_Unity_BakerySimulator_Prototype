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
    [SerializeField] private RecipesDictionarySo recipesDictionarySo;
    [SerializeField] private HoldableObjectSo trashObject;
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
                    GetHoldable().DestroySelf();
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
        if (CurrentState is not State.Idle) return;
        if (HaveHoldable())
        {
            if (!Player.Instance.HoldSystem.HaveHoldable())
            {
                _holdItem.SetParent(Player.Instance.HoldSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (Player.Instance.HoldSystem.HaveHoldable())
            {
                Player.Instance.HoldSystem.GetHoldable().SetParent(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void InteractAlt()
    {
        if (HaveHoldable())
        {
            if (CurrentState is State.Idle)
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
        OvenRecipeSo recipe = recipesDictionarySo.ovenRecipes.FirstOrDefault(r => r.input == GetHoldable().HoldableObjectSo);
        if (recipe is not null)
        {
            CurrentState = State.Processing;
            _timeToProcess = recipe.timeToProcess;
            _ovenRecipeSo = recipe;
        }
        else
        {
            CurrentState = State.Idle;
        }
    }

    public void SetHoldable(HoldableObject holdableObject)
    {
        _holdItem = holdableObject;
    }

    public HoldableObject GetHoldable()
    {
        return _holdItem;
    }
    
    public void ClearHoldable()
    {
        _holdItem = null;
    }

    public bool HaveHoldable()
    {
        return _holdItem != null;
    }

    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}
