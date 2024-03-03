using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Utils
{
    public class RemoveKinemetic : NetworkBehaviour
    {
        // Start is called before the first frame update
        Rigidbody rb;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        private void Update()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }
    }
}
