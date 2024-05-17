using System;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSelectionUI : MonoBehaviour
{
    [SerializeField] ClearStation clearStation;
    [SerializeField] Image icon;
    [SerializeField] Image next;
    [SerializeField] Image previous;

    private static readonly Color Active = Color.black;
    private static readonly Color Inactive = Color.gray;
    
    private ToolRecipeSo _selectedRecipe;
    private int _availableRecipesCount;

    private void Awake()
    {
        gameObject.SetActive(false);
        clearStation.OnFocus += ClearStation_OnFocus;
        clearStation.OnStopFocus += ClearStation_OnStopFocus;
        clearStation.OnRecipeSelected += ClearStation_OnRecipeSelected;
    }
    
    private void ClearStation_OnFocus(object sender, EventArgs e)
    {
        Show();
    }
    
    private void ClearStation_OnStopFocus(object sender, EventArgs e)
    {
        Hide();
    }
    
    private void ClearStation_OnRecipeSelected(object sender, RecipeSelectedEventArgs e)
    {
        _selectedRecipe = e.Recipe;
        _availableRecipesCount = e.AvailableRecipesCount;
        UpdateVisuals();
        Show();
    }

    private void Show()
    {
        if (_selectedRecipe is null) return;
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdateVisuals()
    {
        if (_selectedRecipe is null)
        {
            Hide();
            return;
        }
        icon.sprite = _selectedRecipe.output.sprite;
        next.color = _availableRecipesCount > 1 ? Active : Inactive;
        previous.color = _availableRecipesCount > 1 ? Active : Inactive;
    }
}
