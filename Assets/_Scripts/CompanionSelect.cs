using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CompanionSelect : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public InteractableElement companionData;
    public AlebrijeSelected companion;
    public Sprite imageName;
    public GameObject preview;
    public GameObject previewName;

    private void Start()
    {
        if (prefab != null)
        {
            Image image = GetComponent<Image>();
            if (image.sprite == null)
            {
                image.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
            }
        }
        else
        {
            GetComponent<Button>().enabled = false;
        }
    }

    public void SelectCompanion()
    {
        preview.GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        preview.GetComponent<Animator>().runtimeAnimatorController = prefab.GetComponent<Animator>().runtimeAnimatorController;
        preview.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        previewName.GetComponent<Image>().sprite = imageName;
        previewName.SetActive(true);
        companion.selectedAlebrije = companionData;
    }
}
