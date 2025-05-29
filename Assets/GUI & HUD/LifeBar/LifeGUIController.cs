using System.Collections;
using UnityEngine;

public class LifeGUIController: MonoBehaviour
{
    [SerializeField] private GameObject _fillHP;
    private PlayerController _player;
    private Animator _animator;
    
    private readonly float _fillDuration = 0.5f;
    private float _fillScaleX = 1f;

    private Transform _hpTransform;
    private Coroutine _fillCoroutine = null;


    private void Awake()
    {
        _hpTransform = _fillHP.transform;
        _animator = GetComponent<Animator>();
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

        _fillCoroutine = StartCoroutine(LerpHP(_hpTransform.localScale.x, hpPercentage));

        if (_player.Stats.HP <= 0)
            _animator.SetTrigger("Out");
    }

    private IEnumerator LerpHP(float start, float end)
    {
        float duration = _fillDuration;
        float t = 0f;

        while (t < 1f)
        {
            _fillScaleX = Mathf.Lerp(start, end, t);

            float fill = Mathf.Lerp(0f, 1f, _fillScaleX);

            _hpTransform.localScale = new Vector3(fill, _hpTransform.localScale.y, _hpTransform.localScale.z);

            t += Time.deltaTime / duration;
            yield return null;
        }

        yield return new WaitForEndOfFrame();
    }
}
