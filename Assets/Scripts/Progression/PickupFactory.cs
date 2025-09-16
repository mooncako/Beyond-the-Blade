using Sirenix.OdinInspector;
using UnityEngine;

public class PickupFactory : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private GameObject _itemPickup;
    [SerializeField, BoxGroup("References")] private GameObject _abilityPickup;
    [SerializeField, BoxGroup("References")] private SkillPickup _skillPickup;
    [SerializeField, BoxGroup("References")] private SkillUpgradePickup _skillUpgradePickup;

    public void SpawnPickup(Vector3 pos)
    {

    }

}
