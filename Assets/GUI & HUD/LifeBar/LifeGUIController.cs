using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeGUIController: MonoBehaviour
{
    [SerializeField] private GameObject _fillHP;
    private PlayerController _player;
    private Animator _animator;
    
    private readonly float _fillDuration = 0.2f;
    private float _currentFillValue = 1f;

    private Transform _hpTransform;
    private Image _hpImage;
    private Coroutine _fillCoroutine = null;


    private void Awake()
    {
        _hpTransform = _fillHP.transform;
        _hpImage = _fillHP.GetComponent<Image>();
        _animator = GetComponent<Animator>();
        _currentFillValue = 1f;
    }

    private void Start()
    {
        StartCoroutine(WaitPlayer());
    }

    private IEnumerator WaitPlayer()
    {
        GameObject playerGO;
        do
        {
            playerGO = GameObject.FindGameObjectWithTag(Tags.Player);
            yield return null;
        } while (playerGO == null);


        _player = playerGO.GetComponent<PlayerController>();
        
        _player.OnHPChange += UpdateHP;

        //Set the HP bar initial value
        _currentFillValue = (float)_player.Stats.HP / (float)_player.Stats.MaxHP;
        SetHPBar(_currentFillValue);


    }

    private void OnDisable()
    {
        if (_player != null)
			_player.OnHPChange -= UpdateHP;
    }

    public void UpdateHP()
    {
        float hpPercentage = (float)_player.Stats.HP / (float)_player.Stats.MaxHP;
        
        if (_fillCoroutine != null) 
            StopCoroutine(_fillCoroutine);

        _fillCoroutine = StartCoroutine(LerpHP(hpPercentage));

        if (_player.Stats.HP <= 0)
            _animator.SetTrigger("Out");
    }

    private IEnumerator LerpHP(float targetFill)
    {
        //Currrent Time
        float t = 0f;
        //Initical Fill Value
        float initFill = _currentFillValue;

        //Loop
        while (t < _fillDuration)
        {
            //Update fill
            _currentFillValue = Mathf.Lerp(initFill, targetFill, t/ _fillDuration);
            SetHPBar(_currentFillValue);

            //Increase time and wait to next frame
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //Make sure, the fill is correct
        _hpImage.fillAmount = targetFill;

        yield return new WaitForEndOfFrame();
    }

    private void SetHPBar(float v)
    {
        _hpImage.fillAmount = v;
    }
}
