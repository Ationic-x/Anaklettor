using UnityEngine;

public enum Jobs
{
	Knight,
	Mage,
	Rogue,
	Barbarian,
    Archer,
}

public abstract class ActorController : MonoBehaviour, IActor
{
    protected Rigidbody rb;

    // Scriptable object values
    protected string nickname;
    protected Stats stats;
	protected Jobs job;
	protected GameObject model;

    protected Animator animator;

	protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(ActorData actorData)
    {
        nickname = actorData.nickname;
        job = actorData.job;
		stats = actorData.stats;
		model = actorData.model;

        model = Instantiate(model, transform.position, transform.rotation, transform);

		// Get default components inside the model prefab
		animator = model.GetComponentInChildren<Animator>();
	}

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public virtual void TakeDamage(int amount)
    {
		stats.HP -= amount;
        if (stats.HP < 0) stats.HP = 0;
    }

    public Jobs GetJob()
	{
		return job;
	}

    public string GetNickname()
    {
		return nickname;
	}
}
