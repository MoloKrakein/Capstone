using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleFlow : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;


    public Transform playerLocation;

    public Transform enemyLocation;

    public HUD playerHUD;
    public HUD enemyHUD;

    public DamageManager DamageManager;
    // dont show PlayerUnit and EnemyUnit in inspector
    [HideInInspector]public Unit PlayerUnit;
    [HideInInspector]public Unit EnemyUnit;

    public BattleState state;
    private CameraModule cameraModule;
    public HudModule hudModule;
    // public GameObject SkillButtons;



    private bool isPlayerExtraMove;
    private bool isEnemyExtraMove;
    void Start()
    {
        StartCoroutine(SetupBattle());
        state = BattleState.START;
        cameraModule = GetComponent<CameraModule>();
    
    }
    private void Awake() {
        GameObject PlayerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = PlayerGO.GetComponent<Unit>();
        // playerParty.Add(PlayerUnit);
        // change player Layer to UI
        PlayerUnit.gameObject.layer = 5;

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        // enemyParty.Add(EnemyUnit);
        EnemyUnit.gameObject.layer = 5;
        // Randomize EnemyUnit weakness, power, level
        EnemyUnit.RandomizeUnit();

        PlayerUnit.SetupSkills();
        EnemyUnit.SetupSkills();
        // hudModule.setupHUD();

        
        
    }

    IEnumerator SetupBattle()
    {
        CheckCombatStatus();
        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack(Skill selectedSkill, bool isNormalAttack)
    {
        // HideSkillButtons();
        // StartCoroutine(ChangeTurn());
        CheckCombatStatus();
        hudModule.hideActionButtons();
        PlayerUnit.status = UnitStatus.Status.Idle;
        giveDamage(selectedSkill.AttackPower, EnemyUnit, selectedSkill.AttackType);
        PlayerUnit.attack();
        bool isDead = EnemyUnit.isDead();
        bool isWeakness = EnemyUnit.isWeakness(selectedSkill.AttackType);
        if(!isNormalAttack){
            PlayerUnit.HandleUsedSkill(selectedSkill);
        }


        bool extra = ExtraTurn(isWeakness);
        yield return new WaitForSeconds(2f);
        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else if (extra)
        {
            // encounterText.text = PlayerUnit.unitName + " has an Extra Turn!";
            yield return new WaitForSeconds(1f);
            // show extra turn popup
            StartCoroutine(hudModule.ExtraTurnPopup());
            PlayerUnit.status = UnitStatus.Status.Buff;
            state = BattleState.PLAYERTURN;
            hudModule.UpdateHUD();
            PlayerTurn();
            EnemyUnit.status = UnitStatus.Status.Idle; // Ganti status EnemyUnit menjadi Idle setelah extra turn digunakan
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        enemyHUD.updateHP(EnemyUnit.currentHP);
    }

    IEnumerator EnemyTurn()
    {
        // StartCoroutine(ChangeTurn());
        CheckCombatStatus();

        isPlayerExtraMove = false;
        EnemyUnit.status = UnitStatus.Status.Idle;
        // HideSkillButtons();
        // EnemyUnit.status = UnitStatus.Status.Idle;

        int randIndex = Random.Range(0, EnemyUnit.skills.Count);
        Skill selectedSkill = EnemyUnit.skills[randIndex];

        giveDamage(selectedSkill.AttackPower, PlayerUnit, selectedSkill.AttackType);
        EnemyUnit.attack();
        bool isDead = PlayerUnit.isDead();
        bool isWeakness = PlayerUnit.isWeakness(selectedSkill.AttackType);
        bool extra = ExtraTurn(isWeakness);
        //print attack text
        // encounterText.text = EnemyUnit.unitName + " attacks With " + selectedSkill.Name + "!";
        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else if (extra)
        {
            // encounterText.text = EnemyUnit.unitName + " has an Extra Turn!";
            yield return new WaitForSeconds(1f);
            // show extra turn popup
            StartCoroutine(hudModule.ExtraTurnPopup());
            EnemyUnit.status = UnitStatus.Status.Buff;
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
            PlayerUnit.status = UnitStatus.Status.Idle; // Ganti status PlayerUnit menjadi Idle setelah extra turn digunakan
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        playerHUD.updateHP(PlayerUnit.currentHP);

    }

    public bool ExtraTurn(bool IsWeakness)
    {
        if (state == BattleState.PLAYERTURN)
        {
            if (EnemyUnit.isDown() && IsWeakness)
            {
                if (isPlayerExtraMove)
                {
                    // Player already has an extra turn, don't give another one
                    return false;
                }
                else
                {
                    // Enemy is down and Player hits a weakness, give an extra turn
                    isPlayerExtraMove = true;
                    return true;
                }
            }
            else
            {
                // Reset extra turn flag
                isPlayerExtraMove = false;
                return false;
            }
        }
        else
        {
            if (PlayerUnit.isDown() && IsWeakness)
            {
                if (isEnemyExtraMove)
                {
                    // Enemy already has an extra turn, don't give another one
                    return false;
                }
                else
                {
                    // Player is down and Enemy hits a weakness, give an extra turn
                    isEnemyExtraMove = true;
                    return true;
                }
            }
            else
            {
                // Reset extra turn flag
                isEnemyExtraMove = false;
                return false;
            }
        }
    }
    private void giveDamage(int damage, Unit unitType, DmgType dmgType)
    {
        hudModule.hideActionButtons();
        hudModule.hideItemPanel();
        hudModule.hideSkillbtn();
        int minimumDamage = unitType.damage / 2;
        // random seed
        Random.InitState((int)System.DateTime.Now.Ticks);
        int actualDamage = Random.Range(minimumDamage, damage + 1);
        float criticalChance = 0.1f;
        float randomValue = Random.value;
        bool isDown = false;
        bool isCrit = false;


        // Play Attack Sound
        DamageManager.PlayAttackSound(dmgType);
        
        if (unitType.status == UnitStatus.Status.Down)
        {
            criticalChance = 0.6f;
        }
        if (randomValue < criticalChance)
        {
            actualDamage *= 3;
            isCrit = true;
            unitType.status = UnitStatus.Status.Down;
            if (BattleState.PLAYERTURN == state)
            {
                isPlayerExtraMove = true;
            }
            else
            {
                isEnemyExtraMove = true;
            }
        }
        // isDown = true only when attackType is weakness And unitStatus is Down
        if (unitType.isWeakness(dmgType))
        {
            isDown = true;
        }
        // apply damage
        DamageManager.PlayHitSoundEffect();
        unitType.TakeDamage(actualDamage);

        // dmg popup
        // int damage, bool isPlayer, bool isDown, bool isCrit, int Health, int maxHealth
        if(unitType == PlayerUnit){
            hudModule.SpawnDamagePopup(actualDamage, true, isDown, isCrit, PlayerUnit.currentHP, PlayerUnit.maxHP);
        }else{
            hudModule.SpawnDamagePopup(actualDamage, false, isDown, isCrit, EnemyUnit.currentHP, EnemyUnit.maxHP);
        }
       
        // cam shake
        StartCoroutine(cameraModule.ShakeCamera());

    }

    public void CheckCombatStatus()
    {
        hudModule.UpdateHUD();
        PlayerUnit.damage = PlayerUnit.damage + 1;
        EnemyUnit.damage = EnemyUnit.damage + 1;
        if (PlayerUnit.status == UnitStatus.Status.Dead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else if (EnemyUnit.status == UnitStatus.Status.Dead)
        {
            SpawnNewEnemy();
        }
    }

    void EndBattle()
    {

    }

    void SpawnNewEnemy()
    {

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        EnemyUnit.SetupSkills();
        enemyHUD.setupHUD(EnemyUnit);
        EnemyUnit.RandomizeUnit();

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    void PlayerTurn()
    {
        CheckCombatStatus();
        // ChooseActionMenu.SetActive(true);
        if(!isPlayerExtraMove){
            ChangeTurn();
        }
        hudModule.updateButtons();
        // buttons.ShowButton();
        hudModule.showActionButtons();
        
        PlayerUnit.status = UnitStatus.Status.Idle;

    }

    public void NormalAttack(){
        hudModule.hideActionButtons();
        StartCoroutine(PlayerAttack(PlayerUnit.NormalAttack, true));
        
    }


    public bool Skillusage(int skillCost, bool usesHP)
    {
        if (isPlayerExtraMove)
        {
            skillCost = 0;
        }

        if (usesHP)
        {
            if (PlayerUnit.currentHP < skillCost)
            {
                // encounterText.text = "Not Enough HP!";
                return false;
            }
            PlayerUnit.currentHP -= skillCost;
            playerHUD.updateHP(PlayerUnit.currentHP);
            return true;
        }
        else
        {
            if (PlayerUnit.currentMP < skillCost)
            {
                // encounterText.text = "Not Enough Mana!";
                return false;
            }
            PlayerUnit.currentMP -= skillCost;
            playerHUD.updateMP(PlayerUnit.currentMP);
            return true;
        }
    }
    public void onDefendButton()
    {
        // PlayerUnit.status = UnitStatus.Status.Defend;
        PlayerUnit.OnDefend();
        hudModule.hideActionButtons();
        StartCoroutine(EnemyTurn());
    }


    public void UseSkill(int skillIndex)
    {
        Skill selectedSkill = PlayerUnit.ReadySkills[skillIndex];

        bool usesHP = selectedSkill.UsesHP;
        int skillCost = selectedSkill.ManaCost;
        // Check if player has enough HP/MP
        if (!Skillusage(skillCost, usesHP))
            return;

        StartCoroutine(PlayerAttack(selectedSkill, false));
    }

    public void UseItem(int itemIndex)
    {
        Item selectedItem = PlayerUnit.PassiveSkill[itemIndex];
        StartCoroutine(UseItem(selectedItem));
        hudModule.hideItemPanel();
    }

    IEnumerator UseItem(Item selectedItem)
    {
        PlayerUnit.status = UnitStatus.Status.Idle;
        bool useHP = selectedItem.isUsingHP;
        int skillCost = selectedItem.Cost;
        // Check if player has enough HP/MP
        if (!Skillusage(skillCost, useHP))
            yield break;

        PlayerUnit.UsePassive(selectedItem);
        playerHUD.updateHP(PlayerUnit.currentHP);
        playerHUD.updateMP(PlayerUnit.currentMP);

        // encounterText.text = "You Used " + selectedItem.Name + "!";
        yield return new WaitForSeconds(2f);

        // encounterText.text = "You Restored your Mana!";
        yield return new WaitForSeconds(2f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }


    private void ChangeTurn()
    {
        hudModule.ChangeTurn(BattleState.PLAYERTURN);

    }




}