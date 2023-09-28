using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using textmeshpro;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleFlow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerLocation;
    public Transform enemyLocation;

    public TextMeshProUGUI encounterText;

    public HUD playerHUD;
    public HUD enemyHUD;

    Unit PlayerUnit;
    Unit EnemyUnit;
    

    public BattleState state;
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // create list of string that contains encounter text
        GameObject PlayerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = PlayerGO.GetComponent<Unit>();
        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        string[] encounterTexts = new string[3];
        encounterTexts[0] = "A wild "+ EnemyUnit.unitName +" appeared!";
        encounterTexts[1] = "You encountered an "+ EnemyUnit.unitName +"!";
        encounterTexts[2] = "You are being attacked!";
        encounterText.text = encounterTexts[Random.Range(0,3)];

        playerHUD.setupHUD(PlayerUnit);
        enemyHUD.setupHUD(EnemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        //print attack text
        encounterText.text = PlayerUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);
    }

    void PlayerTurn()
    {
        encounterText.text = "Choose your Move!";
    }

    public void OnAttackButton(){
        if(state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(PlayerAttack());

    }
}
