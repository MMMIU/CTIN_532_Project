using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UISaveDicAttribute : Attribute
    {
        public bool saveDic { get; }

        /// <summary>
        /// Assign the block to the UI panel
        /// </summary>
        /// <param name="block"></param>
        public UISaveDicAttribute(bool save = true)
        {
            saveDic = save;
        }
    }
}
