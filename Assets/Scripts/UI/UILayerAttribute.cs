using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UILayerAttribute : Attribute
    {
        public UIPanelLayer layer { get; }

        /// <summary>
        /// Assign the layer to the UI panel
        /// </summary>
        /// <param name="layer"></param>
        public UILayerAttribute(UIPanelLayer layer)
        {
            this.layer = layer;
        }

    }
}
