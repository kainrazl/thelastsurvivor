using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExp : MonoBehaviour
{
    [SerializeField] private Image expFill;
    [SerializeField] private ParticleSystem fullExp;
    [SerializeField] private GameObject levelUpCanvas;
    [SerializeField] private AudioClip levelUpSound;
    private PlayerManager player;
    private PlaySound play;
    private UpgradeSelection upgradeSelection;

    public float totalExp;
    public float nextLevel = 0;
    public float currentExp;

    private void Awake()
    {
        SetStartExp();
        player = GetComponent<PlayerManager>();
        play = FindAnyObjectByType<PlaySound>();
        upgradeSelection = levelUpCanvas.GetComponent<UpgradeSelection>();
    }

    public void SetStartExp()
    {
        currentExp = 0f;
        expFill.fillAmount = currentExp;
    }

    public void UpdateExp(float expPoints)
    {
        currentExp += expPoints/nextLevel;

        if (currentExp >= 1)
        {
            play.MakeSound(levelUpSound);

            currentExp = 1;
            fullExp.Play();
            levelUpCanvas.SetActive(true);
            upgradeSelection.RandomizeUpgrades();
            player.isPaused = true;
        }

        expFill.fillAmount = currentExp;
    }

    public void PerkSelected()
    {
        //upgradeSelection.EquipUpgrade();
        nextLevel += Mathf.Ceil(nextLevel * 0.2f); //Verificar qué porcentaje debería variar en cada nivel
        SetStartExp();
        player.isPaused = false;

        StartCoroutine(player.PlayerImmune());
    }
}
