using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField]private Unit selectedUnit;
    [SerializeField]LayerMask unitLayerMask;
    bool isBusy;
    BaseAction selectedAction;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.Log("There is more than 1 UnitActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;        
        }
        Instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }

    void Update()
    {
        if (isBusy) 
        { 
            return; 
        }
        if (!TurnSystem.Instance.IsPlayerTurn()) 
        { 
            return ;
        }

        if (EventSystem.current.IsPointerOverGameObject()) 
        {
            return;
        }
        if (TryHandleUnitSelection()) 
        { 
            return; 
        }
        HandleSelectedAction();
    }

    private void HandleSelectedAction()
    {
        if (InputManager.Instance.IsMouseButtonDown())
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) 
                { 
                SetBusy();
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);

                    OnActionStarted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }

    private void SetBusy()
    {
        isBusy = true;
        OnBusyChanged?.Invoke(this,isBusy);
    }
    private void ClearBusy() 
    {
        isBusy = false;
        OnBusyChanged?.Invoke(this,isBusy);
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDown())
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
        {
            if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
            {                 
                    if (unit == selectedUnit) 
                    {
                        // Unit is already selected
                        return false;
                    }

                    if (unit.IsEnemy()) 
                    {
                        // Unit is Enemy
                        return false;
                    }

                    SetSelectedUnit(unit);
                    return true;
            }  
        }
        }
        return false;
    }

    private void SetSelectedUnit(Unit unit) 
    {
        selectedUnit = unit;
        SetSeletedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSeletedAction(BaseAction baseAction) 
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() 
    {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction() 
    {
        return selectedAction;
    }
}
