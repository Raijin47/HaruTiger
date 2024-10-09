using System.Collections;
using UnityEngine;


public class Tutor : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private CanvasGroup _blockingCanvas;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private RectTransform _supportTransform;
    [SerializeField] private float _scaleSpeed;
    [SerializeField] private float _movementSpeed;

    [SerializeField] private RectTransform _moneyTransform;
    [SerializeField] private RectTransform _blockingRaycast;
    [SerializeField] private RectTransform _targetSkill;
    [SerializeField] private ButtonBase _buttonSkill;
    [SerializeField] private ButtonBase _buttonUse;
    [SerializeField] private RectTransform _targetUse;

    private Coroutine _tutorialCoroutine;

    private void Start() 
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) != 1)
        {
            Game.Action.OnEndMove += Complated1Step;
            _canvas.alpha = 1;
            _rect.transform.position = _camera.WorldToScreenPoint(_transforms[0].position);
            _blockingCanvas.blocksRaycasts = true;
            _tutorialCoroutine = StartCoroutine(Step1Tutorial());
        }
    }

    private IEnumerator Step1Tutorial()
    {
        while (true)
        {          
            while(_rect.localScale != Vector3.one * 0.7f)
            {
                yield return null;
                _rect.localScale = Vector3.MoveTowards(_rect.localScale, Vector3.one * 0.7f, Time.deltaTime * _scaleSpeed);
            }

            _supportTransform.position = _camera.WorldToScreenPoint(_transforms[1].position);

            while (_rect.anchoredPosition != _supportTransform.anchoredPosition)
            {
                yield return null;
                _rect.anchoredPosition = Vector3.MoveTowards(_rect.anchoredPosition, _supportTransform.anchoredPosition, Time.deltaTime * _movementSpeed);
            }

            while (_rect.localScale != Vector3.one)
            {
                yield return null;
                _rect.localScale = Vector3.MoveTowards(_rect.localScale, Vector3.one * 1f, Time.deltaTime * _scaleSpeed);
            }

            _supportTransform.position = _camera.WorldToScreenPoint(_transforms[0].position);

            while (_rect.anchoredPosition != _supportTransform.anchoredPosition)
            {
                yield return null;
                _rect.anchoredPosition = Vector3.MoveTowards(_rect.anchoredPosition, _supportTransform.anchoredPosition, Time.deltaTime * _movementSpeed);
            }

            yield return null;
        }
    }

    private void Complated1Step()
    {
        Game.Action.OnEndMove -= Complated1Step;

        _blockingCanvas.alpha = 1;
        _canvas.alpha = 0;
        _blockingRaycast.sizeDelta = _moneyTransform.sizeDelta;
        _blockingRaycast.transform.position = _moneyTransform.transform.position;

        Release();
        _tutorialCoroutine = StartCoroutine(Step2Tutorial());
    }

    private readonly WaitForSeconds Dalay = new(2f);
    private IEnumerator Step2Tutorial()
    {
        yield return Dalay;

        Game.Wallet.Add(112);

        yield return Dalay;

        Complated2Step();
    }

    private void Complated2Step()
    {
        _buttonSkill.OnClick.AddListener(Complated3Step);

        _canvas.alpha = 1;
        _blockingCanvas.alpha = 0;
        _blockingRaycast.sizeDelta = _targetSkill.sizeDelta;
        _blockingRaycast.transform.position = _targetSkill.transform.position;
        _rect.transform.position = _targetSkill.transform.position;

        Release();
        _tutorialCoroutine = StartCoroutine(Step34Tutorial());
    }

    private IEnumerator Step34Tutorial()
    {
        while (true)
        {
            while (_rect.localScale != Vector3.one * 0.7f)
            {
                yield return null;
                _rect.localScale = Vector3.MoveTowards(_rect.localScale, Vector3.one * 0.7f, Time.deltaTime * _scaleSpeed);
            }

            while (_rect.localScale != Vector3.one)
            {
                yield return null;
                _rect.localScale = Vector3.MoveTowards(_rect.localScale, Vector3.one * 1f, Time.deltaTime * _scaleSpeed);
            }
        }
    }

    public void Complated3Step()
    {
        _buttonSkill.OnClick.RemoveListener(Complated3Step);
        _buttonUse.OnClick.AddListener(Complated4Step);

        TileIllusion.Instance.onBlockAction = true;
        _rect.transform.position = _targetUse.transform.position;
        _blockingRaycast.sizeDelta = _targetUse.sizeDelta;
        _blockingRaycast.transform.position = _targetUse.transform.position;

        Release();
        _tutorialCoroutine = StartCoroutine(Step34Tutorial());
    }

    private void Complated4Step()
    {
        _buttonUse.OnClick.RemoveListener(Complated4Step);

        _blockingCanvas.alpha = 1;
        _canvas.alpha = 0;
        _blockingRaycast.sizeDelta = _moneyTransform.sizeDelta;
        _blockingRaycast.transform.position = _moneyTransform.transform.position;

        Release();
        _tutorialCoroutine = StartCoroutine(Step5Tutorial());
    }

    private IEnumerator Step5Tutorial()
    {
        yield return Dalay;
        Complated5Step();
    }

    private void Complated5Step()
    {
        _buttonSkill.OnClick.AddListener(ComplatedTutorial);

        _canvas.alpha = 1;
        _blockingCanvas.alpha = 0;
        _blockingRaycast.sizeDelta = _targetSkill.sizeDelta;
        _blockingRaycast.transform.position = _targetSkill.transform.position;
        _rect.transform.position = _targetSkill.transform.position;

        Release();
        _tutorialCoroutine = StartCoroutine(Step34Tutorial());
    }

    private void ComplatedTutorial()
    {
        _buttonSkill.OnClick.RemoveListener(ComplatedTutorial);
        Release();
        TileIllusion.Instance.onBlockAction = false;
        PlayerPrefs.SetInt("Tutorial", 1);
        _blockingCanvas.blocksRaycasts = false;
        _canvas.alpha = 0;
    }

    private void Release()
    {
        if(_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }
    }
}