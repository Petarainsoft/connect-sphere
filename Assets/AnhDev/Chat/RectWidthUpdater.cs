using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


//TODO rework on this mess
public class RectWidthUpdater : MonoBehaviour
{
    [Tooltip("Maximum width the rect can have.")] [SerializeField]
    private float maxWidth = 500f;

    [SerializeField] private float margin = 20f;

    [SerializeField] private RectTransform cellTransform;

    private RectTransform rectTransform;

    [SerializeField] private LayoutElement _cellLayoutElement;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _isFixed = false;
        var textChild = transform.GetChild(0) as RectTransform;
        textChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 100f);
        textChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100f);
        ContentSizeFitter fitter = textChild.GetComponent<ContentSizeFitter>();
        if ( fitter != null )
        {
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        // _ = UpdateRectHeightOfCell();
    }

    public async UniTaskVoid UpdateRectHeightOfCell()
    {
        await UpdateTextHeight();
        await UniTask.Yield();

        if ( _cellLayoutElement == null )
        {
            _cellLayoutElement = cellTransform.GetComponent<LayoutElement>();
        }

        if ( _cellLayoutElement != null )
        {
            _cellLayoutElement.minHeight = rectTransform.rect.height;
        }
    }

    private bool _isFixed = false;

    private async UniTask<bool> UpdateTextHeight()
    {
        await UniTask.Yield();
        await UniTask.Yield();
        var textChild = transform.GetChild(0) as RectTransform;
        var width = textChild.rect.width;
        // Debug.Log($"<color=red>TezxWID {totalWidth}</color>");
        var height = textChild.rect.height;

        if ( width > maxWidth )
        {
            foreach (RectTransform child in rectTransform)
            {
                ContentSizeFitter fitter = child.GetComponent<ContentSizeFitter>();
                if ( fitter != null )
                {
                    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                }
            }

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth + margin*2f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + margin*2f);
            textChild.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            
        }
        else
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + margin*2f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + margin*2f);
        }

        await UniTask.Yield();
        await UniTask.Yield();
        return true;
    }
}