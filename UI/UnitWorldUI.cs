using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointText;
    [SerializeField] private Unit unit;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointChanged;
        UpdateActionPointsText();
    }
    private void UpdateActionPointsText() 
    {
        actionPointText.text = unit.GetUnitActionPoints().ToString();
    }

    private void Unit_OnAnyActionPointChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
            
    }    


}

