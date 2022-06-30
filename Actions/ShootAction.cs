using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private int rifleDamage = 40;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootingUnit;
    }
    private enum State
    {
        aiming,
        shooting,
        cooloff,
    }

    [SerializeField] LayerMask obstaclesLayerMask;
    [SerializeField] int maxShootDistance = 7;
    private State state;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, rotateSpeed * Time.deltaTime);
                break;
            case State.shooting:
                if (canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }

    }

    private void NextState()
    {
        switch (state)
        {
            case State.aiming:
                if (stateTimer <= 0f)
                {
                    state = State.shooting;
                    float shootingStateTime = 0.1f;
                    stateTimer = shootingStateTime;
                }
                break;
            case State.shooting:
                if (stateTimer <= 0f)
                {
                    state = State.cooloff;
                    float coolOffStateTime = 0.5f;
                    stateTimer = coolOffStateTime;
                }
                break;
            case State.cooloff:
                if (stateTimer <= 0f)
                {
                    ActionComplete();
                }
                break;
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();  

        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }


                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    //if grid position is empty, no unit
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    // both unit on same team
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                if (Physics.Raycast(
                     unitWorldPosition + Vector3.up * unitShoulderHeight,
                     shootDir,
                     Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                     obstaclesLayerMask)) 
                {
                    //Blocked by obstacles
                    continue;
                }
  



                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;

    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        canShootBullet = true;

        ActionStart(onActionComplete);
    }

    public override int GetActionPointsCost()
    {
        return 1;
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });

        targetUnit.TakeDamage(rifleDamage);
    }

    public Unit GetTargeUnit()
    {
        return targetUnit;
    }

    public int GetMaxShootDistance()
    {
        return maxShootDistance;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
       Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
       return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1- targetUnit.GetHealthNormalized())*100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) 
    {
       return GetValidActionGridPositionList(gridPosition).Count;
    }
}

