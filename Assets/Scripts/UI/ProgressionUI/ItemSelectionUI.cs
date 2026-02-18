using MoreMountains.Tools;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectionUI : MonoBehaviour, MMEventListener<NewItemSelectionEvent>, MMEventListener<NewProgressionDropEvent>
{
    [SerializeField, BoxGroup("References")] private ShopItemDatabaseSO _shopItemDatabase;
    [SerializeField, BoxGroup("References")] private CanvasGroup _canvasGroup;
    [SerializeField, BoxGroup("References")] private ItemUI[] _itemUis;
    [SerializeField, BoxGroup("References")] private Button _discardButton;

    private Tween _alphaTween;

    void OnValidate()
    {
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        this.MMEventStartListening<NewItemSelectionEvent>();
        this.MMEventStartListening<NewProgressionDropEvent>();
        _discardButton.onClick.AddListener(Discard);
    }

    void OnDisable()
    {
        this.MMEventStopListening<NewItemSelectionEvent>();
        this.MMEventStopListening<NewProgressionDropEvent>();
        _discardButton.onClick.RemoveListener(Discard);
        _alphaTween.Stop();
    }

    public void OnMMEvent(NewItemSelectionEvent e)
    {
        if (e.Type == EventStateType.OnEventStarted)
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 1, .5f);
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        else
        {
            _alphaTween.Stop();
            _alphaTween = Tween.Alpha(_canvasGroup, 0, .5f);
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
    }

    public void OnMMEvent(NewProgressionDropEvent e)
    {
        if (e.ProgressionType == ProgressionType.Stats)
        {
            AssignItems();
        }
    }

    [Button]
    private void AssignItems()
    {
        int index;
        for (int i = 0; i < _itemUis.Length; i++)
        {
            index = Random.Range(0, _shopItemDatabase.ShopItems.Count);
            _itemUis[i].AssignData(_shopItemDatabase.ShopItems[index]);
        }
    }

    private void Discard()
    {
        ProgressionCanvasCloseEvent.Trigger();
        gameObject.SetActive(false);
    }
    
}
