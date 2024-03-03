using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Quest;
using Manager;
namespace Events {
    public class JoinCodeAssignEvent : EventBase
    {
        public bool success;
        public string joinCode;
        public JoinCodeAssignEvent(bool success, string code, float delay = 0f, string name = nameof(JoinCodeAssignEvent)) : base(name, delay)
        {
            this.success = success;
            this.joinCode = code;
        }
    }
}