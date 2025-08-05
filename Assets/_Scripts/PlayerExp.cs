using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExp : MonoBehaviour
{
    [SerializeField] private Image expFill;
    [SerializeField] private ParticleSystem fullExp;
    [SerializeField] private GameObject levelUpCanvas;
    private PlayerManager player;

    public float totalExp;
    public float nextLevel = 0;
    public float currentExp;

    private void Awake()
    {
        SetStartExp();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerManager>();
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
            currentExp = 1;
            fullExp.Play();
            levelUpCanvas.SetActive(true);
            player.isPaused = true;
        }

        expFill.fillAmount = currentExp;
    }

    public void PerkSelected()
    {
        levelUpCanvas.SetActive(false);
        player.isPaused = false;
        nextLevel += Mathf.Ceil(nextLevel * 0.2f); //Verificar qué porcentaje debería variar en cada nivel
        SetStartExp();
    }
}
