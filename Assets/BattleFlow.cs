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

    Unit PlayerUnit;
    Unit EnemyUnit;
    

    public BattleState state;
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        GameObject PlayerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = PlayerGO.GetComponent<Unit>();
        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();

        encounterText.text = EnemyUnit.unitName;

        
    }
}
