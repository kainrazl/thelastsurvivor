using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionSelect : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    public InteractableElement companionData;
    public AlebrijeSelected companion;

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
        GameObject preview = GameObject.Find("AlebrijePreview");
        GameObject previewName = GameObject.Find("PreviewName");
        preview.GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        preview.GetComponent<Animator>().runtimeAnimatorController = prefab.GetComponent<Animator>().runtimeAnimatorController;
        previewName.GetComponent<TextMeshProUGUI>().text = prefab.name;

        companion.selectedAlebrije = companionData;
    }
}
