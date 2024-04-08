using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ending
{
    public class EndingSceneCauldron : MonoBehaviour
    {
        [SerializeField]
        private GameObject flarePS;

        private void Start()
        {
            flarePS.SetActive(false);
        }

        public void StartFlare()
        {
            flarePS.SetActive(true);
        }

        public void StopFlare()
        {
            flarePS.SetActive(false);
        }
    }
}