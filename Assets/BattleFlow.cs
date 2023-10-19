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

    public TextMeshProUGUI encounterText;

    public GameObject dmgPopup;

    public HUD playerHUD;
    public HUD enemyHUD;

    // camera
    public Camera mainCamera;
    Unit PlayerUnit;
    Unit EnemyUnit;

    public BattleState state;

    private bool extraTurn = false;
    private bool enemyExtraTurn = false;

    public List<Unit> playerParty = new List<Unit>();
    public List<Unit> enemyParty = new List<Unit>();
    private Vector3 cameraPosition;
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
        Vector3 cameraPosition = mainCamera.transform.position;

        PlayerUnit.setInitialSkills();
        EnemyUnit.setInitialSkills();
        PlayerUnit.SetupSkills();
        EnemyUnit.SetupSkills();


        // turnOrder.Add(PlayerUnit);
        // turnOrder.Add(EnemyUnit);
        // turnOrder.Sort((unit1, unit2) => unit2.speed.CompareTo(unit1.speed));

        string[] encounterTexts = new string[3];
        encounterTexts[0] = "A wild " + EnemyUnit.unitName + " appeared!";
        encounterTexts[1] = "You encountered an " + EnemyUnit.unitName + "!";
        encounterTexts[2] = "You are being attacked";
        encounterText.text = encounterTexts[Random.Range(0, 3)];

        playerHUD.setupHUD(PlayerUnit);
        enemyHUD.setupHUD(EnemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
    IEnumerator PlayerAttack(Skill selectedSkill)
{
    PlayerUnit.status = UnitStatus.Status.Idle;
    giveDamage(selectedSkill.AttackPower, EnemyUnit, selectedSkill.AttackType);
    CheckCombatStatus();
    bool isDead = EnemyUnit.isDead();

    encounterText.text = PlayerUnit.unitName + " attacks With " + selectedSkill.Name + "!";
    enemyHUD.updateHP(EnemyUnit.currentHP);
    PlayerUnit.HandleUsedSkill(selectedSkill);

    bool isWeakness = EnemyUnit.isWeakness(selectedSkill.AttackType);
    // Pastikan enemy tidak dalam status 'Down' sebelum memberi extra turn
    if (isWeakness && !PlayerUnit.hasExtraTurn && EnemyUnit.status != UnitStatus.Status.Down)
    {
        extraTurn = true;
        PlayerUnit.hasExtraTurn = true;
    }
    yield return new WaitForSeconds(1f);

    if (isDead)
    {
        state = BattleState.WON;
        EndBattle();
    }
    else if (extraTurn)
    {
        encounterText.text = EnemyUnit.unitName + " is Down! One More";
        yield return new WaitForSeconds(1f);
        PlayerUnit.status = UnitStatus.Status.Buff;
        state = BattleState.PLAYERTURN;
        extraTurn = false;
    }
    else
    {
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }
}
   IEnumerator EnemyTurn()
{
    EnemyUnit.status = UnitStatus.Status.Idle;

    int randIndex = Random.Range(0, EnemyUnit.skills.Count);
    Skill selectedSkill = EnemyUnit.skills[randIndex];

    giveDamage(selectedSkill.AttackPower, PlayerUnit, selectedSkill.AttackType);
    bool isDead = PlayerUnit.isDead();
    bool isWeakness = PlayerUnit.isWeakness(selectedSkill.AttackType);

    if (isWeakness)
    {
        enemyExtraTurn = true;
    }

    //print attack text
    encounterText.text = EnemyUnit.unitName + " attacks With " + selectedSkill.Name + "!";
    playerHUD.updateHP(PlayerUnit.currentHP);

    yield return new WaitForSeconds(2f);

    if (isDead)
    {
        state = BattleState.LOST;
        EndBattle();
    }
    else if (enemyExtraTurn && PlayerUnit.status != UnitStatus.Status.Down)
    {
        encounterText.text = PlayerUnit.unitName + " is Down! Watch Out!";
        yield return new WaitForSeconds(1f);
        state = BattleState.ENEMYTURN;
        enemyExtraTurn = false;
        StartCoroutine(EnemyTurn());
    }
    else
    {
        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }
}

    private void giveDamage(int damage, Unit unitType, DmgType dmgType)
    {
        int actualDamage = Random.Range(1, damage + 1);

        GameObject popup = Instantiate(dmgPopup,
                                       (unitType == PlayerUnit) ? enemyLocation.position : playerLocation.position,
                                       Quaternion.identity);
        popup.GetComponent<TextMeshPro>().text = actualDamage + "!";
        popup.GetComponent<TextMeshPro>().color = (unitType == PlayerUnit) ? Color.red : Color.blue;

        unitType.TakeDamage(actualDamage, dmgType);
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
            state = BattleState.WON;
            EndBattle();
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            encounterText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            encounterText.text = "You were defeated.";
        }
    }

    void PlayerTurn()
    {

        PlayerUnit.RefreshReadySkills();

        // Randomly select a skill dari ReadySkills
        int randIndex = Random.Range(0, PlayerUnit.ReadySkills.Count);
        Skill selectedSkill = PlayerUnit.ReadySkills[randIndex];

        encounterText.text = "Choose your Move!";
    }

    IEnumerator PlayerHeal()
    {
        PlayerUnit.status = UnitStatus.Status.Idle;
        int healAmount = Random.Range(1, 100);

        PlayerUnit.currentHP += healAmount;
        playerHUD.updateHP(PlayerUnit.currentHP);
        encounterText.text = "You Healed for " + healAmount + " HP!";
        yield return new WaitForSeconds(2f);
        PlayerUnit.currentMP = 100;
        playerHUD.updateMP(PlayerUnit.currentMP);
        encounterText.text = "You Restored your Mana!";
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
        if (usesHP)
        {
            if (PlayerUnit.currentHP < skillCost)
            {
                encounterText.text = "Not Enough HP!";
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
                encounterText.text = "Not Enough Mana!";
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

        int randIndex = Random.Range(0, PlayerUnit.skills.Count);
        Skill selectedSkill = PlayerUnit.skills[randIndex];

        bool usesHP = selectedSkill.UsesHP;
        int skillCost = selectedSkill.ManaCost;
        if (!extraTurn && !Skillusage(skillCost, usesHP))
            return;
        else
            StartCoroutine(PlayerAttack(selectedSkill));
    }
}