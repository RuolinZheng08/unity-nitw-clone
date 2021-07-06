using System.Collections.Generic;
using Yarn.Unity;

public class ActionDialogue : SceneAction
{
    private DialogueRunner dialogueRunner;
    private float interactionRadius = 10f;

    private void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    public override void Interact()
    {
        CheckForNearbyNPC();
    }

    /// Find all DialogueParticipants
    /** Filter them to those that have a Yarn start node and are in range;
     * then start a conversation with the first one
     */
    public void CheckForNearbyNPC()
    {
        var allParticipants = new List<NPC>(FindObjectsOfType<NPC>()); // Retrieves all NPC in the scene
        var player = FindObjectOfType<PlayerController2D>(); // Retrieves the player in the scene
        var target = allParticipants.Find(delegate (NPC p) { // Returns the NPC
            return string.IsNullOrEmpty(p.talkToNode) == false && // has a conversation node?
            (p.transform.position - player.transform.position).magnitude <= interactionRadius; // is in range?
        });
        if (target != null)
        {
            // Kick off the dialogue at this node.
            dialogueRunner.StartDialogue(target.talkToNode);
        }
    }
}
