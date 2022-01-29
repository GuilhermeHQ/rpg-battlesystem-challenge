 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST } 

public class battlesystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    [SerializeField] float timeToNextState = 2f;
    [SerializeField] float timeToNextTurn = 1f;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    unit playerUnit;
    unit enemyUnit;

    public Text dialogText;

    public battlehud playerHUD;
    public battlehud enemyHUD;

    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<unit>();

        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<unit>();

        dialogText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(timeToNextState);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogText.text = "You attacked!";

        yield return new WaitForSeconds(timeToNextTurn);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        IEnumerator EnemyTurn()
        {
            dialogText.text = enemyUnit.unitName + " attacks!";

            yield return new WaitForSeconds(timeToNextTurn);

            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

            playerHUD.SetHP(playerUnit.currentHP);

            yield return new WaitForSeconds(timeToNextTurn);

            if(isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                PlayerTurn();
            }

        }

        void EndBattle()
        {
            if(state == BattleState.WON)
            {
                dialogText.text = "You Won!";
            }
            else if (state == BattleState.LOST)
            {
                dialogText.text = "You lost...";
            }
        }

    }

    void PlayerTurn()
    {
        dialogText.text = "Choose an action:";         
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine (PlayerAttack());


    }

}
