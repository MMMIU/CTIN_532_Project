//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Inputs
//{
//    public class InputManager : MonoBehaviour
//    {
//        private static InputReader inputReader;
//        public static InputReader Reader
//        {
//            get
//            {
//                if (inputReader == null)
//                {
//                    Init();
//                }
//                return inputReader;
//            }
//        }

//        private static void Init()
//        {
//            var obj = Resources.Load("SOs/InputReaderSO");
//            if (obj == null)
//            {
//                Debug.LogError("Prefab not found: InputReaderSO");
//                return;
//            }
//            inputReader = Instantiate(obj) as InputReader;
//        }
//    }
//}