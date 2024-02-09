using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIBlockAttribute : Attribute
    {
        public bool block { get; }

        /// <summary>
        /// Assign the block to the UI panel
        /// </summary>
        /// <param name="block"></param>
        public UIBlockAttribute(bool block = true)
        {
            this.block = block;
        }
    }
}
