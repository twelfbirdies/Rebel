using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    private float timer;

    private enum State
    { 
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    
    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn()) 
        { 
            return; 
        }

        switch (state) 
        { 
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {

                    if (TryTakeEnemeyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else 
                    {
                        // no more enemies have action to take
                        TurnSystem.Instance.NextTurn();
                    }             
                }
                break;
            case State.Busy:
                break;          
        }


       
    }


    private void SetStateTakingTurn() 
    {
        timer = 0.5f;
        state = State.TakingTurn;
    
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) 
    {
        if (!TurnSystem.Instance.IsPlayerTurn()) 
        {
            state = State.TakingTurn;
            timer = 2f;
        }        
        
    }


    private bool TryTakeEnemeyAIAction(Action onEnemyAIActionComplete) 
    {
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) 
        {
            if (TryTakeEnemeyAIAction(enemyUnit, onEnemyAIActionComplete)) 
            {
                return true;
            }            
        }
        return false;
    }

    private bool TryTakeEnemeyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) 
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (BaseAction baseAction in enemyUnit.GetBaseActionsArray()) 
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) 
            {       
                //Enemy cannot afford to this action
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;

            }
            else 
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) 
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
                   
        
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else 
        {
            return false;
        }

    }

}
