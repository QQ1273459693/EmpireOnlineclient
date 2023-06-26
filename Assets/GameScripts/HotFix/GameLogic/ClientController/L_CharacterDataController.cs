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
                Log.Debug("�����б����ݳ���:" + CharacterListData.Count);
            }
            else
            {
                Log.Debug("��ɫ���ݲ�����");
            }
            
        }
    }
}
