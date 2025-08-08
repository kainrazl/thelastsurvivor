using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Image heartFill;
    [SerializeField] private TextMeshProUGUI tmpHealtPercentage;

    public float currentHealth;
    public int percentage;

    public void SetStartHealth()
    {
        currentHealth = 1f;
        percentage = 100;
        tmpHealtPercentage.text = percentage.ToString() + "%";
    }

    public void UpdateHealth(float healthPoints, bool isDamage)
    {
        if (isDamage)
        {
            currentHealth -= healthPoints;
        }
        else
        {
            currentHealth += healthPoints;
        }

        if (currentHealth >= 1)
        {
            currentHealth = 1;
            percentage = 100;
        }
        else {
            percentage = Mathf.RoundToInt(Mathf.Abs(currentHealth) * 100f);
        }

        heartFill.fillAmount = currentHealth;
        tmpHealtPercentage.text = percentage.ToString() + "%";
    }
}
