using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    internal sealed class ImageLoad : ReferenceDisposer
    {
        public Image m_Image;
        public ImageLoad imagLoad;
        public ImageLoad()
        {
            m_Image = null;
        }

        public static ImageLoad Create(string IconName,Image img)
        {
            //һ��ʼû�оʹ���һ�����Żس�����ѭ������
            ImageLoad imgLoad = ReferencePool.Acquire<ImageLoad>();
            imgLoad.m_Image = img;
            imgLoad.m_Image.sprite = GameModule.Resource.LoadAsset<Sprite>(IconName);
            imgLoad.imagLoad = imgLoad;
            return imgLoad;
        }
        public void LoadSprite(string IconName, Image img)
        {
            m_Image = img;
            m_Image.sprite = GameModule.Resource.LoadAsset<Sprite>(IconName);
        }

        public void Clear()
        {
            imagLoad?.Dispose();
            m_Image = null;
            imagLoad = null;
        }
    }
}
