using System.Collections;
using UnityEngine;

public enum CreatureType
{
    Companion,
    Boss
}

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private AbilityType activeAbilityType = AbilityType.Fear;
    [SerializeField] private float abilityPower = 3f;
    [SerializeField] private float abilityRadius = 3f;
    [SerializeField] private float abilityCooldown = 12f;
    [Tooltip("Únicamente indicar cuando se trate de un Boss")] [SerializeField] private GameObject abilityAnimation; //Prefab de la animación de la habilidad
    [SerializeField] private CreatureType creatureType;
    [Tooltip("Duración de la habilidad, solo se usa para habilidades que tengan duración")][SerializeField] private float abilityDuration = 5f; // Duración de la habilidad, solo se usa para habilidades que tengan duración
    private Ability activeAbility;
    private GameObject source;
    private GameObject abilityContainer;
    private float lastAbilityTime = 0f;

    private void Awake()
    {
        activeAbility = new Ability();
    }

    private void Start()
    {
        if (creatureType == CreatureType.Companion)
        {
            source = GameObject.FindGameObjectWithTag("Player");
            abilityContainer = source.transform.Find("CompanionAbility").gameObject; // Busca el objeto de habilidad dentro del jugador
        }
        else if (creatureType == CreatureType.Boss)
        {
            abilityContainer = Instantiate(abilityAnimation, transform.position, Quaternion.identity, transform);
            source = gameObject; // Script se agrega al GameObject del Boss, por lo que el source es el mismo GameObject
        }

        if (abilityContainer != null)
        {
            abilityContainer.SetActive(false);
        }
    }

    void Update()
    {
        lastAbilityTime += Time.deltaTime;
        
        ExecuteAbility();
    }
    public void ExecuteAbility()
    {
        if (lastAbilityTime < abilityCooldown)
        {
            return;
        }

        activeAbility.ActivateAbility(activeAbilityType, source, abilityPower, abilityRadius, abilityCooldown, creatureType, abilityDuration);
        lastAbilityTime = 0f;

        if (abilityContainer != null)
        {
            StartCoroutine(ShowAbilityAnimation(1.5f));
        }

        StartCoroutine(AbilityCooldownIndicator());
    }

    public AbilityType GetActiveAbilityType()
    {
        return activeAbilityType;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(source.transform.position, abilityRadius);
    }

    private IEnumerator AbilityCooldownIndicator()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < abilityCooldown)
        {
            // Agregar una barra de cooldown o un indicador visual
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Cooldown terminado, actualizar el indicador visual para mostrar que la habilidad está lista
    }

    public IEnumerator ShowAbilityAnimation(float duration)
    {
        abilityContainer.SetActive(true);
        yield return new WaitForSeconds(duration); //Espera duration segundos antes de ocultar la animación
        abilityContainer.SetActive(false);
    }
}