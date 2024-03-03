using Events;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Players
{
    public partial class Player : NetworkBehaviour
    {
        bool isSpecialSkillOneActive = false;
        public void PrincessSkillOne()
        {
            //if (isSpecialSkillOneActive)
            //{
            //    isSpecialSkillOneActive = false;
            //    new VCamChangeEvent("");
            //}
            //else
            //{
            //    isSpecialSkillOneActive = true;
            //    new VCamChangeEvent("overview");
            //}
        }

    }
}