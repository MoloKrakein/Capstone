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
    // public Transform enemyPopupsLocation;

    // public TextMeshProUGUI encounterText;

    public GameObject dmgPopup;
    public GameObject extraTurnPopup;
    public GameObject encounterPopup;
    public Canvas canvas;

    public HUD playerHUD;
    public HUD enemyHUD;

    public Button skillButton1;
    public Button skillButton2;
    public Button skillButton3;
    public Button skillButton4;
    public Button skillButton5;
    // camera
    // public Camera mainCamera;
    Unit PlayerUnit;
    Unit EnemyUnit;

    public BattleState state;

    public Camera cam;
    public float shakeDuration = 1f;
    public float shakeMagnitude = 1f;

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

        GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = EnemyGO.GetComponent<Unit>();
        enemyParty.Add(EnemyUnit);
        // Vector3 cameraPosition = mainCamera.transform.position;

        // PlayerUnit.setInitialSkills();
        // EnemyUnit.setInitialSkills();
        PlayerUnit.SetupSkills();
        EnemyUnit.SetupSkills();


        playerHUD.setupHUD(PlayerUnit);
        enemyHUD.setupHUD(EnemyUnit);

        UpdateSkillButtons();

        yield return new WaitForSeconds(2f);
        HideSkillButtons();
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack(Skill selectedSkill)
    {
        HideSkillButtons();
        PlayerUnit.status = UnitStatus.Status.Idle;
        giveDamage(selectedSkill.AttackPower, EnemyUnit, selectedSkill.AttackType);
        CheckCombatStatus();
        bool isDead = EnemyUnit.isDead();
        bool isWeakness = EnemyUnit.isWeakness(selectedSkill.AttackType);
        PlayerUnit.HandleUsedSkill(selectedSkill);

        
        enemyHUD.updateHP(EnemyUnit.currentHP);
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
            PlayerTurn();
            EnemyUnit.status = UnitStatus.Status.Idle; // Ganti status EnemyUnit menjadi Idle setelah extra turn digunakan
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        encounterPopup.SetActive(false);
        isPlayerExtraMove = false;
        EnemyUnit.status = UnitStatus.Status.Idle;
        HideSkillButtons();
        // EnemyUnit.status = UnitStatus.Status.Idle;

        int randIndex = Random.Range(0, EnemyUnit.skills.Count);
        Skill selectedSkill = EnemyUnit.skills[randIndex];

        giveDamage(selectedSkill.AttackPower, PlayerUnit, selectedSkill.AttackType);
        bool isDead = PlayerUnit.isDead();
        bool isWeakness = PlayerUnit.isWeakness(selectedSkill.AttackType);
        bool extra = ExtraTurn(isWeakness);
        //print attack text
        // encounterText.text = EnemyUnit.unitName + " attacks With " + selectedSkill.Name + "!";
        playerHUD.updateHP(PlayerUnit.currentHP);
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
        int actualDamage = Random.Range(1, damage + 1);
        float criticalChance = 0.1f;
        float randomValue = Random.value;
        bool isDown = false;
        // Enable EncounterPopUps
        encounterPopup.SetActive(true);
        encounterPopup.GetComponent<EncounterPopUps>().SetText(dmgType.ToString() + " Damage");
        if (unitType.status == UnitStatus.Status.Down)
        {
            criticalChance = 0.6f;
        }
        if (randomValue < criticalChance)
        {
            actualDamage *= 3;
            // encounterText.text = "Critical Hit!";
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

        // dmg popups
        GameObject dmgPopUp = Instantiate(dmgPopup, canvas.transform);
        if (unitType == PlayerUnit)
        {
            dmgPopUp.GetComponent<DamagePopUps>().spawnPopups(actualDamage, false, isDown, unitType.currentHP);
        }
        else
        {
            dmgPopUp.GetComponent<DamagePopUps>().spawnPopups(actualDamage, true, isDown, unitType.currentHP);
        }
        // cam shake
        StartCoroutine(CamShake());

    }

    public void CheckCombatStatus()
    {
        if (PlayerUnit.status == UnitStatus.Status.Dead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else if (EnemyUnit.status == UnitStatus.Status.Dead)
        {
            bool enemyAmbush = 0.3f<Random.value;
            if(enemyAmbush){
                // encounterText.text = "New Enemy Has Arrived";
                SpawnNewEnemy();
            }else{
            state = BattleState.WON;
            EndBattle();
            }
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            // encounterText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            // encounterText.text = "You were defeated.";
        }
    }

    void SpawnNewEnemy(){
    GameObject EnemyGO = Instantiate(enemyPrefab, enemyLocation);
    EnemyUnit = EnemyGO.GetComponent<Unit>();
    enemyParty.Add(EnemyUnit);
    // EnemyUnit.setInitialSkills();
    EnemyUnit.SetupSkills();
    enemyHUD.setupHUD(EnemyUnit);
    
    state = BattleState.ENEMYTURN;
    StartCoroutine(EnemyTurn());
    }
    void PlayerTurn()
    {
        ShowSkillButtons();
        UpdateSkillButtons();
        PlayerUnit.status = UnitStatus.Status.Idle;

        // PlayerUnit.RefreshReadySkills();
        // encounterText.text = "Choose your Move!";
    }

    IEnumerator PlayerHeal()
    {
        PlayerUnit.status = UnitStatus.Status.Idle;
        int healAmount = Random.Range(1, 100);
        PlayerUnit.currentHP += healAmount;
        playerHUD.updateHP(PlayerUnit.currentHP);
        // encounterText.text = "You Healed for " + healAmount + " HP!";
        yield return new WaitForSeconds(2f);
        PlayerUnit.currentMP = 100;
        playerHUD.updateMP(PlayerUnit.currentMP);
        // encounterText.text = "You Restored your Mana!";
        yield return new WaitForSeconds(2f);
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
    public void OnRestoreButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
        Debug.Log("Heal");
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

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        int randIndex = Random.Range(0, PlayerUnit.ReadySkills.Count);
        Skill selectedSkill = PlayerUnit.ReadySkills[randIndex];

        bool usesHP = selectedSkill.UsesHP;
        int skillCost = selectedSkill.ManaCost;
        StartCoroutine(PlayerAttack(selectedSkill));
    }

    public void UseSkill(int skillIndex)
    {
        Skill selectedSkill = PlayerUnit.ReadySkills[skillIndex];

        bool usesHP = selectedSkill.UsesHP;
        int skillCost = selectedSkill.ManaCost;
        // Check if player has enough HP/MP
        if (!Skillusage(skillCost, usesHP))
            return;

        StartCoroutine(PlayerAttack(selectedSkill));
    }

    public void UpdateSkillButtons()
    {
        skillButton1.GetComponentInChildren<TextMeshProUGUI>().text = PlayerUnit.ReadySkills[0].Name;
        skillButton2.GetComponentInChildren<TextMeshProUGUI>().text = PlayerUnit.ReadySkills[1].Name;
        skillButton3.GetComponentInChildren<TextMeshProUGUI>().text = PlayerUnit.ReadySkills[2].Name;
        skillButton4.GetComponentInChildren<TextMeshProUGUI>().text = PlayerUnit.ReadySkills[3].Name;
        skillButton5.GetComponentInChildren<TextMeshProUGUI>().text = PlayerUnit.ReadySkills[4].Name;
    }

    private void HideSkillButtons()
    {
        skillButton1.gameObject.SetActive(false);
        skillButton2.gameObject.SetActive(false);
        skillButton3.gameObject.SetActive(false);
        skillButton4.gameObject.SetActive(false);
        skillButton5.gameObject.SetActive(false);
    }

    private void ShowSkillButtons()
    {
        skillButton1.gameObject.SetActive(true);
        skillButton2.gameObject.SetActive(true);
        skillButton3.gameObject.SetActive(true);
        skillButton4.gameObject.SetActive(true);
        skillButton5.gameObject.SetActive(true);
    }
    IEnumerator CamShake(){
        Vector3 originalPos = cam.transform.localPosition;
        float elapsed = 0.0f;

        while(elapsed < shakeDuration){
            float x = Random.Range(-1f,1f) * shakeMagnitude;
            float y = Random.Range(-1f,1f) * shakeMagnitude;

            cam.transform.localPosition = new Vector3(x,y,originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = originalPos;
        float originalZoom = cam.orthographicSize;
        // zoom in
        cam.orthographicSize = 0.2f;
        yield return new WaitForSeconds(0.5f);
        // zoom out
        cam.orthographicSize = originalZoom;


    }

    IEnumerator ExtraTurnPopup()
    {
        GameObject extraTurnPopUp = Instantiate(extraTurnPopup, canvas.transform);
        yield return new WaitForSeconds(2f);
        Destroy(extraTurnPopUp);
    }
    
}