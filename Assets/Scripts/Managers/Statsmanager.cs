using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statsmanager
{
    private static Statsmanager instance;
    public static Statsmanager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Statsmanager();
            }
            return instance;
        }
    }
    // Character
    public float m_totalChracterCount = 0;

    // Quest
    public int m_maxQuestCount = 3;
    public float m_questAssignProbability = 0.1f;
    public float m_randomEventProbability = 0.8f;
    public float m_totalQuestCompleteCount = 0;
}
