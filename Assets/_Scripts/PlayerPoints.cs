using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerPoints : MonoBehaviour
{
    public int currentPoints = 0;
    private float currentTime = 0;
    private int minutes = 0;
    private int seconds = 0;
    private PlayerManager playerManager;

    [SerializeField] private int maxPoints;
    [SerializeField] private TextMeshProUGUI textPoints;

    public void UpdatePoints()
    {
        currentPoints += 5;

        textPoints.text = "Score: " + currentPoints;

        // Debug.Log(Time.deltaTime.ToString());
    }

   void Awake()
   {
      playerManager = GetComponent<PlayerManager>();
   }

   private void Update()
    {
        if (!playerManager.isDead)
        {
            currentTime += Time.deltaTime;
            seconds = (int)currentTime;
            minutes = seconds / 60;
            seconds = seconds % 60;
            textPoints.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }
}
