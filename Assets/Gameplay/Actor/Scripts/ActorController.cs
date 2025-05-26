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
    protected Stats stats = new Stats();
	protected Jobs job;
	protected GameObject model;

    protected Animator animator;

	protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public virtual void Initialize(ActorData actorData, string name)
    {
        job = actorData.job;
		stats.UpdateStats(actorData.stats);
		model = actorData.model;
		nickname = name;

		GameObject modelGO = Instantiate(model, transform.position, transform.rotation, transform);

		// Get default components inside the model prefab
		animator = modelGO.GetComponentInChildren<Animator>();
	}

	// We need to refactor in future the code between
	// Player and Enemy, so we can use the same actor controller

	public void Interact()
    {
        throw new System.NotImplementedException();
    }

	public Jobs Job { get { return job; } }

	public Stats Stats { get { return stats; } }

	public string Name { get { return nickname; } }

	public GameObject Model { get { return model; } }

}
