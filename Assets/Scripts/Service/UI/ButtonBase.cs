using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    #region Sub-Classes
    [System.Serializable]
    public class UIButtonEvent : UnityEvent<PointerEventData.InputButton> { }
    #endregion
    public UnityEvent OnClick;

    private RectTransform _rectTransform;
    private readonly Vector2 PressedSize = new(0.9f, 0.9f);
    private const float ResizeDuration = 0.2f;
    private Vector2 _currentSize;
    private Coroutine _resizeCoroutine; 

    private void Awake()
    {
        _rectTransform ??= GetComponent<RectTransform>();
        _currentSize = _rectTransform.localScale;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_resizeCoroutine != null)
            StopCoroutine(_resizeCoroutine);

        _resizeCoroutine = StartCoroutine(ResizeButton(PressedSize));
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_resizeCoroutine != null)
            StopCoroutine(_resizeCoroutine);

        _resizeCoroutine = StartCoroutine(ResizeButton(_currentSize));
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
        Game.Instance.Audio.OnClick();
    }

    private void OnEnable()
    {
        _rectTransform.localScale = _currentSize;
    }

    private IEnumerator ResizeButton(Vector2 targetSize)
    {
        Vector2 initialSize = _rectTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < ResizeDuration)
        {
            _rectTransform.localScale = Vector2.Lerp(initialSize, targetSize, elapsedTime / ResizeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rectTransform.localScale = targetSize;
    }
}