using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Utils
{
    public class RemoveKinemetic : NetworkBehaviour
    {
        // Start is called before the first frame update
        public override void OnNetworkSpawn()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}
