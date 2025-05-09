using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    private EnemyData _data;

    public void SetData(EnemyData data)
    {
        _data = data;
    }

    private void OnMouseDown()
    {
        if (_data != null && _data.isAlive)
        {
            _data.isAlive = false;
            Destroy(gameObject);
        }
    }
}
