using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorDataSO", menuName = "Scriptable Objects/ActorDataSO")]
public class ActorDataSO : ScriptableObject
{
    public GameObject model;
    public Job job;
    public Stats stats;
    public AnimatorController animatorController;
}
