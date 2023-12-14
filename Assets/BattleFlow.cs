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
    // public Transform playerPopupsLocation;
    public Transform enemyLocation;

    // public GameObject SoundManager;
    // public Transform enemyPopupsLocation;

    // public TextMeshProUGUI encounterText;

    public GameObject dmgPopup;
    public GameObject extraTurnPopup;
    public GameObject encounterPopup;
    // public GameObject ChooseActionMenu;
    public GameObject ChangeTurnPopup;

    public GameObject DarkScreen;
    public Canvas canvas;

    public HUD playerHUD;
    public HUD enemyHUD;

    public MagicSkillList buttons;
    public ItemList itemList;
    public DamageManager DamageManager;
    public GameObject ActionButtons;
    // camera
    // public Camera mainCamera;
    Unit PlayerUnit;
    Unit EnemyUnit;

    public BattleState state;

    public Camera cam;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;
    public float zoomSize = 5f;

    // public GameObject SkillButtons;

    public List<Unit> playerParty = new List<Unit>();
    public List<Unit> enemyParty = new List<Unit>();

    private bool isPlayerExtraMove;
    private bool isEnemyExtraMove;
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject PlayerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = PlayerGO.GetComponent<Unit>();
        playerParty.Add(PlayerUnit);
        // change player Layer to UI
        PlayerUnit.gameObject.layer = 5;

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        enemyParty.Add(EnemyUnit);
        EnemyUnit.gameObject.layer = 5;
        // Randomize EnemyUnit weakness, power, level
        EnemyUnit.RandomizeUnit();

        // Vector3 cameraPosition = mainCamera.transform.position;

        // PlayerUnit.setInitialSkills();
        // EnemyUnit.setInitialSkills();
        PlayerUnit.SetupSkills();
        EnemyUnit.SetupSkills();
        

        // updateButtons();
        setupButtons();

        playerHUD.setupHUD(PlayerUnit);
        enemyHUD.setupHUD(EnemyUnit);
        hideActionButtons();
        // UpdateSkillButtons();

        yield return new WaitForSeconds(2f);
        // HideSkillButtons();
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack(Skill selectedSkill, bool isNormalAttack)
    {
        // HideSkillButtons();
        // StartCoroutine(ChangeTurn());
        CheckCombatStatus();
        buttons.HideButton();
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
            StartCoroutine(ExtraTurnPopup());
            PlayerUnit.status = UnitStatus.Status.Buff;
            state = BattleState.PLAYERTURN;
            updateButtons();
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
        encounterPopup.SetActive(false);
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
            StartCoroutine(ExtraTurnPopup());
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
        int minimumDamage = unitType.damage / 2;
        // random seed
        Random.InitState((int)System.DateTime.Now.Ticks);
        int actualDamage = Random.Range(minimumDamage, damage + 1);
        float criticalChance = 0.1f;
        float randomValue = Random.value;
        bool isDown = false;
        bool isCrit = false;
        // Enable EncounterPopUps
        encounterPopup.SetActive(true);
        encounterPopup.GetComponent<EncounterPopUps>().SetText(dmgType.ToString() + " Damage");
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

        // dmg popups
        GameObject dmgPopUp = Instantiate(dmgPopup, canvas.transform);
        if (unitType == PlayerUnit)
        {
            // dmgPopUp.GetComponent<DamagePopUps>().SetupDmgPopup(EnemyUnit.maxHP);
            dmgPopUp.GetComponent<DamagePopUps>().spawnPopups(actualDamage, false, isDown, isCrit ,PlayerUnit.currentHP, PlayerUnit.maxHP);

        }
        else
        {
            // dmgPopUp.GetComponent<DamagePopUps>().SetupDmgPopup(PlayerUnit.maxHP);
            dmgPopUp.GetComponent<DamagePopUps>().spawnPopups(actualDamage, true, isDown, isCrit ,EnemyUnit.currentHP, EnemyUnit.maxHP);
        }
        // cam shake
        StartCoroutine(CamShake());

    }

    public void CheckCombatStatus()
    {
        playerHUD.updateHP(PlayerUnit.currentHP);
        enemyHUD.updateHP(EnemyUnit.currentHP);
        playerHUD.updateMP(PlayerUnit.currentMP);
        enemyHUD.updateMP(EnemyUnit.currentMP);
        playerHUD.updateWeakness(PlayerUnit);
        enemyHUD.updateWeakness(EnemyUnit);
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
        if (state == BattleState.WON)
        {
            ChangeTurnPopup.GetComponent<ChangeTurnPopUps>().WinLosePopups(true);
        }
        else if (state == BattleState.LOST)
        {
            ChangeTurnPopup.GetComponent<ChangeTurnPopUps>().WinLosePopups(false);
        }
    }

    void SpawnNewEnemy()
    {

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        enemyParty.Add(EnemyUnit);
        // EnemyUnit.setInitialSkills();
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
        updateButtons();
        // buttons.ShowButton();
        showActionButtons();
        
        PlayerUnit.status = UnitStatus.Status.Idle;

    }

    public void NormalAttack(){
        ActionButtons.SetActive(false);
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
        hideActionButtons();
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
        itemList.HidePanel();
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

    private void setupButtons()
    {
        // public void SetButton(int index, string name, int mana, bool useHP,Sprite icon)
        for(int i = 0; i < 5; i++){
            buttons.SetButton(i, PlayerUnit.ReadySkills[i].Name, PlayerUnit.ReadySkills[i].ManaCost, PlayerUnit.ReadySkills[i].UsesHP, PlayerUnit.ReadySkills[i].SkillSprite);
        }
        // passiveSkill
        for(int i = 0; i < 5; i++){
            itemList.SetItem(i, PlayerUnit.PassiveSkill[i].ItemName, PlayerUnit.PassiveSkill[i].Cost, PlayerUnit.PassiveSkill[i].isUsingHP, PlayerUnit.PassiveSkill[i].ItemSprite);
        }

        
    }
    private void updateButtons()
    {
for(int i = 0; i < 5; i++){
            buttons.SetButton(i, PlayerUnit.ReadySkills[i].Name, PlayerUnit.ReadySkills[i].ManaCost, PlayerUnit.ReadySkills[i].UsesHP, PlayerUnit.ReadySkills[i].SkillSprite);
        }

    }

    private void hideActionButtons()
    {
        ActionButtons.SetActive(false);
    }
    private void showActionButtons()
    {
        ActionButtons.SetActive(true);
    }
    IEnumerator CamShake()
    {
        enemyHUD.hideUI();
        playerHUD.hideUI();
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        // Scale up playerunit
        Vector3 originalScalePlayer = PlayerUnit.transform.localScale;
        Vector3 originalScaleEnemy = EnemyUnit.transform.localScale;
        Vector3 OriginalPosPlayer = playerLocation.transform.localPosition;
        Vector3 OriginalPosEnemy = enemyLocation.transform.localPosition;
        Vector3 midPos = new Vector3(originalPos.x, originalPos.y-1.5f, 0);
        // GameObject PlayerLocationCopy = Instantiate(playerLocation.gameObject, OriginalPosPlayer);
        // GameObject EnemyLocationCopy = Instantiate(enemyLocation.gameObject, OriginalPosEnemy);
        // GameObject PlayerCopy = Instantiate(PlayerUnit.gameObject, PlayerLocationCopy);
        // GameObject EnemyCopy = Instantiate(EnemyUnit.gameObject, EnemyLocationCopy);
        

        if(state == BattleState.PLAYERTURN){
            PlayerUnit.transform.localScale = originalScalePlayer * 1.5f;
            // EnemyUnit.transform.localScale = originalScaleEnemy * 0.5f;
            playerLocation.transform.localPosition = midPos;
            // PlayerCopy.transform.localScale = originalScalePlayer * 0.5f;
            // PlayerCopy.transform.localPosition = PlayerLocationCopy;

        }else{
            EnemyUnit.transform.localScale = originalScaleEnemy * 1.5f;
            // PlayerUnit.transform.localScale = originalScalePlayer * 0.5f;
            enemyLocation.transform.localPosition = midPos;
            // EnemyCopy.transform.localScale = originalScaleEnemy * 0.5f;
            // EnemyCopy.transform.localPosition = EnemyLocationCopy;
        }

        DarkScreen.SetActive(true);

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cam.transform.localPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = originalPos;
        // float originalZoom = cam.orthographicSize;
        // // Vector3 originalPos = cam.transform.localPosition;
        // float targetPos = 0f;
        // float enemyPos = enemyLocation.position.x;
        // float playerPos = playerLocation.position.x;

        // if(state == BattleState.PLAYERTURN){
        //     targetPos = enemyPos;    

        // }else{
        //     targetPos = playerPos;
        // }
        // // zoom in
        // cam.orthographicSize = zoomSize;
        // // move to target pos
        // cam.transform.localPosition = new Vector3(targetPos,originalPos.y,originalPos.z);

        yield return new WaitForSeconds(1f);
        DarkScreen.SetActive(false);
        // Scale down playerunit
        PlayerUnit.transform.localScale = originalScalePlayer;
        EnemyUnit.transform.localScale = originalScaleEnemy;
        playerLocation.transform.localPosition = OriginalPosPlayer;
        enemyLocation.transform.localPosition = OriginalPosEnemy;

        playerHUD.showUI();
        enemyHUD.showUI();

        playerHUD.updateHP(PlayerUnit.currentHP);
        enemyHUD.updateHP(EnemyUnit.currentHP);


        // zoom out
        // cam.orthographicSize = originalZoom;
        // move to original pos
        // cam.transform.localPosition = originalPos;


    }

    IEnumerator ExtraTurnPopup()
    {
        GameObject extraTurnPopUp = Instantiate(extraTurnPopup, canvas.transform);
        yield return new WaitForSeconds(2f);
        Destroy(extraTurnPopUp);
    }

    private void ChangeTurn()
    {
        GameObject changeTurnPopUp = Instantiate(ChangeTurnPopup, canvas.transform);
        changeTurnPopUp.GetComponent<ChangeTurnPopUps>().spawnPopups(state == BattleState.PLAYERTURN);


    }

    // IEnumerator DarkenScreen(){
    //     DarkScreen.SetActive(true);   
        
    //     yield return new WaitForSeconds(1f);
    //     DarkScreen.SetActive(false);
    // }

    // Sound Manager
    // public void PlaySound(string soundName)
    // {
    //     SoundManager.GetComponent<SoundManager>().Play(soundName);
    // }


}