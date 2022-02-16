using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, PLAYERACTION, ENEMYTURN, WON, LOST }

public class battlesystem : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject attackButton;
    public GameObject healButton;

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
        attackButton.SetActive(true);
        healButton.SetActive(true);
    }

    IEnumerator PlayerAttack()
    {
        attackButton.SetActive(false);
        healButton.SetActive(false);

        playerUnit.AttackAnim();
        yield return new WaitForSeconds(1f);

        enemyUnit.DamageAnim();
        yield return new WaitForSeconds(1f);

        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogText.text = "You attacked!";

        state = BattleState.PLAYERACTION;


        yield return new WaitForSeconds(timeToNextTurn);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    IEnumerator EnemyTurn()
    {
        enemyUnit.AttackAnim();
        yield return new WaitForSeconds(1f);

        playerUnit.DamageAnim();
        yield return new WaitForSeconds(1f);

        dialogText.text = enemyUnit.unitName + " attacks!";
        yield return new WaitForSeconds(timeToNextTurn);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(timeToNextTurn);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
            attackButton.SetActive(true);
            healButton.SetActive(true);
        }

    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogText.text = "You Won!";
        }
        else if (state == BattleState.LOST)
        {
            dialogText.text = "You lost...";
        }
    }



    void PlayerTurn()
    {
        dialogText.text = "Choose an action:";
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.Heal(5);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogText.text = "You feel renewed strength!";

        state = BattleState.PLAYERACTION;
        attackButton.SetActive(false);
        healButton.SetActive(false);

        yield return new WaitForSeconds(timeToNextTurn);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());


    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());


    }

}
