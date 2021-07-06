using UnityEngine;

public abstract class SceneAction : MonoBehaviour
{
    [SerializeField] private Sprite actionIcon = null;

    public abstract void Interact();

    public Sprite GetActionIcon() {
        return actionIcon;
    }
}
