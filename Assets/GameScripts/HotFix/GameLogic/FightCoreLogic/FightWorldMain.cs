using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class FightWorldMain : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            NetWorkManager.Instance.InitSocket();
            HallMsgHandlerConter.Instance.Initialize();
            WorldManager.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            WorldManager.Update();
        }
    }
}
