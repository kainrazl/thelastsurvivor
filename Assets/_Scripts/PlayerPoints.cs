using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerPoints : MonoBehaviour
{
    public int currentPoints = 0;

    [SerializeField] private int maxPoints;
    [SerializeField] private TextMeshProUGUI textPoints;

    public void UpdatePoints()
    {
        currentPoints += 5;

        textPoints.text = "Points: " + currentPoints;

        if(currentPoints >= maxPoints)
        {
            SceneManager.LoadScene("Win");
        }
    }
}
