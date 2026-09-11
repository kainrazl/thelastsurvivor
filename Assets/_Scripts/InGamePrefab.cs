using UnityEngine;

public class InGamePrefab : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;

    public GameObject GetPrefab()
    {
        return objectPrefab;
    }
}