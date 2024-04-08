using Inputs;
using Managers;
using Quest;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Playables;

namespace Ending
{
    public class EndingSceneManager : MonoBehaviour
    {
        [SerializeField]
        EndingSceneCauldron knightCauldron;

        [SerializeField]
        EndingSceneCauldron princessCauldron;

        [SerializeField]
        EndingSceneArena knightArena;

        [SerializeField]
        PlayableDirector endingTimeline;

        [SerializeField]
        InputReader inputReader;

        [SerializeField]
        int playerCount = 0;

        private void Start()
        {
            knightArena.OnKnightEnter += OnKnightEnter;
            knightArena.OnKnightExit += OnKnightExit;
            knightArena.OnPrincessEnter += OnPrincessEnter;
            knightArena.OnPrincessExit += OnPrincessExit;
        }

        private void OnKnightEnter()
        {
            if (GameManager.Instance.gameover)
            {
                return;
            }
            knightCauldron.StartFlare();
            if (++playerCount == 2)
            {
                GameEnd();
            }
        }

        private void OnKnightExit()
        {
            if (GameManager.Instance.gameover)
            {
                return;
            }
            knightCauldron.StopFlare();
            playerCount--;
        }

        private void OnPrincessEnter()
        {
            if (GameManager.Instance.gameover)
            {
                return;
            }
            princessCauldron.StartFlare();
            if(++playerCount == 2)
            {
                GameEnd();
            }
        }

        private void OnPrincessExit()
        {
            if (GameManager.Instance.gameover)
            {
                return;
            }
            princessCauldron.StopFlare();
            playerCount--;
        }

        private void GameEnd()
        {
            GameManager.Instance.gameover = true;
            endingTimeline.Play();
            inputReader.EnableUIInput();
            UIManager.Instance.CloseAll();
        }
    }
}