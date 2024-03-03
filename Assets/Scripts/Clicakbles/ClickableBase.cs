using Events;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ClickableBase : NetworkBehaviour
{
    public virtual void OnClickStart() { }
    public virtual void OnClickEnd() { }
}
