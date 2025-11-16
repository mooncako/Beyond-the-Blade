using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(DontDestroy))]
[RequireComponent(typeof(ObjectPool))]
public class CurrencyFactory : MonoBehaviour,
    MMEventListener<DropCurrencyEvent>
{
    [SerializeField, BoxGroup("References")] private ObjectPool _pool;

    [SerializeField, BoxGroup("Currencies")] private GameObject _goldCoinPrefab;

    void OnValidate()
    {
        if(_pool == null) _pool = GetComponent<ObjectPool>();
    }


    public void OnMMEvent(DropCurrencyEvent e)
    {
        switch(e.CurrencyType)
        {
            case CurrencyType.Gold:
                GetCurrencies(_goldCoinPrefab, e.Amount, e.Pos);
                break;
        }
    }

    [Button]
    private void GetCurrencies(GameObject currencyPrefab, int amount, Vector3 pos)
    {
        Vector2 randomPoint;

        for(int i = 0; i < amount; i++)
        {
            randomPoint = Random.insideUnitCircle * 1.5f;
            Currency currency = _pool.Get(currencyPrefab).GetComponent<Currency>();
            currency.transform.position = new Vector3(pos.x + randomPoint.x, pos.y + .9f, pos.z + randomPoint.y);
            currency.OnCurrencyHit.AddListener(() => ReturnCurrency(currency));
        }
    }

    private void ReturnCurrency(Currency currency)
    {
        currency.OnCurrencyHit.RemoveAllListeners();
        _pool.Return(currency.gameObject);
    }
}
