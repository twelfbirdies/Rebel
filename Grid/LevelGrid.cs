using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{

    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private GridSystem<GridObject> gridSystem;
    public event EventHandler OnAnyUnitMovedGridPosition;
    private void Awake()
    {     
            if (Instance != null)
            {
                Debug.Log("There is more than 1 LevelGrid" + transform + "-" + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

        gridSystem = new GridSystem<GridObject>(width, height, cellSize,
            (GridSystem < GridObject > g, GridPosition gridPosition) => new GridObject(g, gridPosition));
      //  gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start()
    {
        Pathfinding.Instance.SetUp(width,height,cellSize);
    }
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) 
    { 
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }
    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }
    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) 
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
        OnAnyUnitMovedGridPosition?.Invoke(this, EventArgs.Empty);
    }
    public int GetGridHeight() 
    {
        return gridSystem.GetHeight();
    }
    public int GetGridWidth()
    {
        return gridSystem.GetWidth();
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return gridSystem.GetWorldPosition(gridPosition);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) 
    {
        return gridSystem.GetGridPosition(worldPosition);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) 
    { 
        return gridSystem.IsValidGridPosition(gridPosition);
    }
    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition) 
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

}
