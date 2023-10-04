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

    public GameObject dmgPopup;

    public HUD playerHUD;
    public HUD enemyHUD;

    // public 

    Unit PlayerUnit;
    Unit EnemyUnit;

    DmgType.Type playerDmgType;
    

    public BattleState state;
    void Start()
    {
        state = BattleState.START;
        // Debug log print all DmgType enum
        // foreach (DmgType.Type dmgType in System.Enum.GetValues(typeof(DmgType.Type)))
        // {
        //     Debug.Log(dmgType);
        // }
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
        bool isDead = giveDamage(PlayerUnit.damage, EnemyUnit);
        
        //print attack text
        encounterText.text = PlayerUnit.unitName + " attacks With " + playerDmgType +" !";
        // enemyHUD.updateDamage(PlayerUnit.damage);
        enemyHUD.updateHP(EnemyUnit.currentHP);
        Debug.Log(EnemyUnit.currentHP);
        Debug.Log(EnemyUnit.status);

        yield return new WaitForSeconds(1f);

        if(isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else if(EnemyUnit.status == UnitStatus.Status.Down)
        {
            encounterText.text = EnemyUnit.unitName + " is Down! One More";
            state = BattleState.PLAYERTURN;
            // StartCoroutine(PlayerTurn());
        }
        else
        {
            
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        bool isDead = giveDamage(EnemyUnit.damage,  PlayerUnit);
        
        //print attack text
        encounterText.text = EnemyUnit.unitName + " attacks!";
        // playerHUD.updateDamage(EnemyUnit.damage);
    
        playerHUD.updateHP(PlayerUnit.currentHP);

        yield return new WaitForSeconds(1f);

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
        private bool giveDamage(int damage, Unit unitType){
        // calculate damage with random range with max is int damage
        damage = Random.Range(1, damage);
        Debug.Log(damage);
        if(unitType == PlayerUnit){
            Instantiate(dmgPopup, enemyLocation.position, Quaternion.identity);
            dmgPopup.GetComponent<TextMeshPro>().text = damage + "!";
        }
        else{
            Instantiate(dmgPopup, playerLocation.position, Quaternion.identity);
            dmgPopup.GetComponent<TextMeshPro>().text = damage + "!";
        }
        
            
        // then call TakeDamage
        // print to console location
        // Debug.Log(location);
        return unitType.TakeDamage(damage, playerDmgType);

        

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

    public void OnAttackButton(){
        if(state != BattleState.PLAYERTURN)
            return;
        // randomize player damage type
        // playerDmgType = (DmgType.Type)Random.Range(0,5);
        playerDmgType = DmgType.Type.Fire;
        
        StartCoroutine(PlayerAttack(playerDmgType));

    }
}
