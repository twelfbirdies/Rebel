using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Button button;
    [SerializeField] private GameObject SelectedGameObject;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction) 
    {
        this.baseAction = baseAction;
        textMeshProUGUI.text = baseAction.GetActionName().ToUpper();
        button.onClick.AddListener(() => 
        { 
        UnitActionSystem.Instance.SetSeletedAction(baseAction);       
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        SelectedGameObject.SetActive(selectedBaseAction == baseAction); 
        
    }

}
