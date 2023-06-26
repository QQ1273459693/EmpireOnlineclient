using GameBase;
using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class L_CharacterDataController: Singleton<L_CharacterDataController>
    {
        public List<S_Character> CharacterListData = new List<S_Character>();

        public void LoadCharacterData()
        {
            if (LocalSaveManager.KeyExists("CharacterListData"))
            {
                CharacterListData = (List<S_Character>)LocalSaveManager.Load("CharacterListData");
                Log.Debug("看下列表数据长度:" + CharacterListData.Count);
            }
            else
            {
                Log.Debug("角色数据不存在");
            }
            
        }
    }
}
