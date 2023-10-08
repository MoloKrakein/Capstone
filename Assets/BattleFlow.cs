using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject dmgPopup;

    public HUD playerHUD;
    public HUD enemyHUD;

    public DmgType dmgTypeInstance; // Reference to the DmgType scriptable object

    Unit PlayerUnit;
    Unit EnemyUnit;

    DmgType.Type playerDmgType;
    DmgType.Type enemyDmgType;
    public BattleState state;

    private bool extraTurn = false;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }
    
    IEnumerator SetupBattle()
    {
        GameObject PlayerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = PlayerGO.GetComponent<Unit>();
        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        string[] encounterTexts = new string[3];
        encounterTexts[0] = "A wild "+ EnemyUnit.unitName +" appeared!";
        encounterTexts[1] = "You encountered an "+ EnemyUnit.unitName +"!";
        encounterTexts[2] = "You are being attacked";
        encounterText.text = encounterTexts[Random.Range(0,3)];

        playerHUD.setupHUD(PlayerUnit);
        enemyHUD.setupHUD(EnemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack(DmgType.Type dmgType)
    {
        bool isDead = giveDamage(PlayerUnit.damage, EnemyUnit, dmgType);
        
        //print attack text
        encounterText.text = PlayerUnit.unitName + " attacks With " + playerDmgType +" !";
        enemyHUD.updateHP(EnemyUnit.currentHP);
        playerHUD.updateMP(PlayerUnit.currentMP);
        Debug.Log(EnemyUnit.currentHP);
        Debug.Log(EnemyUnit.status);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else if(!extraTurn)
        {
            extraTurn = true; // Activate the extra turn
            encounterText.text = EnemyUnit.unitName + " is Down! One More";
            PlayerUnit.status = UnitStatus.Status.Buff;
            state = BattleState.PLAYERTURN;
        }
        else
        {    
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        enemyDmgType = (DmgType.Type)Random.Range(0,5);
        bool isDead = giveDamage(EnemyUnit.damage,  PlayerUnit, enemyDmgType);
        
        //print attack text
        // encounterText.text = EnemyUnit.unitName + " attacks!";
        encounterText.text = EnemyUnit.unitName + " attacks With " + enemyDmgType +" !";

        playerHUD.updateHP(PlayerUnit.currentHP);

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else if(PlayerUnit.status == UnitStatus.Status.Down)
        {
            encounterText.text = PlayerUnit.unitName + " is Down! Watch Out!";
            PlayerUnit.status = UnitStatus.Status.Idle;
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

private bool giveDamage(int damage, Unit unitType, DmgType.Type dmgType)
{
    int actualDamage = Random.Range(1, damage + 1);
    Debug.Log(actualDamage);

    GameObject popup = Instantiate(dmgPopup, 
                                   (unitType == PlayerUnit) ? enemyLocation.position : playerLocation.position, 
                                   Quaternion.identity);
    popup.GetComponent<TextMeshPro>().text = actualDamage + "!";
    popup.GetComponent<TextMeshPro>().color = (unitType == PlayerUnit) ? Color.red : Color.blue;

    Debug.Log("Enemy HP: " + EnemyUnit.currentHP);
    Debug.Log("Player HP: " + PlayerUnit.currentHP);

    return unitType.TakeDamage(actualDamage, dmgType);
}


    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            encounterText.text = "You won the battle!";
        }
        else if(state == BattleState.LOST)
        {
            encounterText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {
        encounterText.text = "Choose your Move!";
    }

    public void OnRestoreButton(){
        if(state != BattleState.PLAYERTURN)
            return;
        PlayerUnit.currentMP = 100;
        playerHUD.updateMP(PlayerUnit.currentMP);
        PlayerUnit.currentHP = 100;
        playerHUD.updateHP(PlayerUnit.currentHP);
        encounterText.text = "You Restored All your Stats!";
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    public void OnAttackButton()
    {
        if(state != BattleState.PLAYERTURN)
            return;

        playerDmgType = (DmgType.Type)Random.Range(0,5);
        // playerDmgType = DmgType.Type.Physical;
        bool usesHP = dmgTypeInstance.damageInfos[(int)playerDmgType].UsesHP;

        if(extraTurn)
        {   
            extraTurn = false;
            PlayerUnit.status = UnitStatus.Status.Idle;
            StartCoroutine(PlayerAttack(playerDmgType));
        }
        
        if (usesHP)
        {   
            int hpCost = dmgTypeInstance.damageInfos[(int)playerDmgType].ManaCost;
            if(PlayerUnit.currentHP < hpCost)
            {
                encounterText.text = "Not Enough HP!";
                return;
            }
            PlayerUnit.currentHP -= hpCost;
            playerHUD.updateHP(PlayerUnit.currentHP);
            StartCoroutine(PlayerAttack(playerDmgType));
        }
        else
        {   
        int manaCost = dmgTypeInstance.damageInfos[(int)playerDmgType].ManaCost;
            if(PlayerUnit.currentMP < manaCost)
            {
                encounterText.text = "Not Enough Mana!";
                return;
            }
            PlayerUnit.currentMP -= manaCost;
            StartCoroutine(PlayerAttack(playerDmgType));
        }
    }
}
