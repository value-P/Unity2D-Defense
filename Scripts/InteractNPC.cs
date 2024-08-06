using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractNPC : InteractBase
{
    public override void Interact()
    {
        InteractWithName(interactName);
    }

    protected void InteractWithName(string name)
    {
        switch (name)
        {
            case "°­È­" :
                CloserableUIBase.OpenUI("Reinforce");
            break;
            default: return;
        }
    }
}
