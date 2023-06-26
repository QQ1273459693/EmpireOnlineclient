using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class HeroSpineController : MonoBehaviour, IHasSkeletonDataAsset
    {

        public bool isChangeAnim;
        public string AnimSr;
        public string WeaponIcon;
        public Sprite WeaponSprite;
        public bool isUISpine;
        public int SpineEquipType;
        //动画组件
        protected SkeletonGraphic m_Animation;
        public SkeletonGraphic Animation
        {
            get { return m_Animation; }
        }
        public SkeletonAnimation skeletonAnimation;
        //骨骼换装
        [SpineSkin]
        public string templateSkinName;

        Spine.Skin equipsSkin;
        Spine.Skin collectedSkin;


        public List<EquipHook> equippables = new List<EquipHook>();//骨骼需要换装的部位
        [System.Serializable]
        public class EquipHook
        {
            public EquipType type;
            [SpineSlot]
            public string slot;
            [SpineSkin]
            public string templateSkin;
            [SpineAttachment(skinField: "templateSkin")]
            public string templateAttachment;
            [Header("多部位同时换装列表")]
            [SpineSlot]
            public List<string> ListSlot;
            [SpineAttachment(skinField: "templateSkin")]
            public List<string> ListTemplateAttachment;
        }

        public enum EquipType
        {
            WEAPON,
            SHIELD,
            BODY,
            SHOULDER,//护肩,双A
            LEG,//护腿
            HELMET,//头盔
            ARM,//护臂
        }

        public SkeletonDataAsset skeletonDataAsset;
        SkeletonDataAsset IHasSkeletonDataAsset.SkeletonDataAsset { get { return this.skeletonDataAsset; } }

        public Material sourceMaterial;



        public void Equip(EquipType type,string Icon)
        {
            EquipHook howToEquip=null;
            int RangeMax = 0;
            List<string> m_ListChange=new List<string>();
            switch (type)
            {
                case EquipType.WEAPON:
                    RangeMax = 55;
                    Icon = $"MELEE {Random.Range(1, RangeMax)}";
                    howToEquip =equippables[0];
                    break;
                case EquipType.SHIELD:
                    RangeMax = 35;
                    Icon = $"SHIELD {Random.Range(1, RangeMax)}";
                    howToEquip = equippables[1];
                    break;
                case EquipType.BODY:
                    RangeMax = 32;

                    int RandeBody = Random.Range(1, RangeMax);
                    string ListBodyChangeA = $"{RandeBody}BODY";
                    string ListHipChangeB = $"{RandeBody}HIP";
                    m_ListChange.Add(ListBodyChangeA);
                    m_ListChange.Add(ListHipChangeB);
                    howToEquip = equippables[2];
                    break;
                case EquipType.SHOULDER:
                    RangeMax = 24;
                    int Rande = Random.Range(1, RangeMax);
                    string ListChangeA = $"{Rande}SHOULDER A";
                    string ListChangeB = $"{Rande}SHOULDER B";
                    m_ListChange.Add(ListChangeA);
                    m_ListChange.Add(ListChangeB);
                    howToEquip = equippables[3];
                    break;
                case EquipType.LEG:
                    RangeMax = 26;
                    int Rande1 = Random.Range(1, RangeMax);
                    string ListChangeA1 = $"{Rande1}LEG A1";
                    string ListChangeA2 = $"{Rande1}LEG A2";
                    string ListChangeA3 = $"{Rande1}LEG A3";
                    string ListChangeB1 = $"{Rande1}LEG B1";
                    string ListChangeB2 = $"{Rande1}LEG B2";
                    string ListChangeB3 = $"{Rande1}LEG B3";
                    m_ListChange.Add(ListChangeA1);
                    m_ListChange.Add(ListChangeA2);
                    m_ListChange.Add(ListChangeA3);
                    m_ListChange.Add(ListChangeB1);
                    m_ListChange.Add(ListChangeB2);
                    m_ListChange.Add(ListChangeB3);
                    howToEquip = equippables[4];
                    break;
                case EquipType.HELMET:
                    RangeMax = 34;
                    Icon = $"{Random.Range(1, RangeMax)}HELMET";
                    howToEquip = equippables[5];
                    break;
                case EquipType.ARM:
                    RangeMax = 35;
                    int Rande2 = Random.Range(1, RangeMax);
                    string ListChangeA11 = $"{Rande2}ARM A1";
                    string ListChangeA22 = $"{Rande2}ARM A2";
                    string ListChangeA32 = $"{Rande2}ARM A3";
                    string ListChangeB12 = $"{Rande2}ARM B1";
                    string ListChangeB22 = $"{Rande2}ARM B2";
                    string ListChangeB32 = $"{Rande2}ARM B3B";
                    string ListChangeB33 = $"{Rande2}ARM B3F";
                    m_ListChange.Add(ListChangeA11);
                    m_ListChange.Add(ListChangeA22);
                    m_ListChange.Add(ListChangeA32);
                    m_ListChange.Add(ListChangeB12);
                    m_ListChange.Add(ListChangeB22);
                    m_ListChange.Add(ListChangeB32);
                    m_ListChange.Add(ListChangeB33);
                    howToEquip = equippables[6];
                    break;
            }
            var skeletonData = skeletonDataAsset.GetSkeletonData(true);

            //这里判断是不是多部位换装
            if (howToEquip.ListSlot.Count>0)
            {
                //说明是多部位换装
                for (int i = 0; i < howToEquip.ListSlot.Count; i++)
                {
                    int slotIndex = skeletonData.FindSlotIndex(howToEquip.ListSlot[i]);
                    Sprite sprite = WeaponSprite != null ? WeaponSprite : GameModule.Resource.LoadAsset<Sprite>(m_ListChange[i]);
                    if (sprite == null)
                    {
                        Debug.LogError("错误,加载资源出错!");
                        return;
                    }
                    var attachment=GenerateAttachmentFromEquipAsset(sprite, slotIndex, howToEquip.templateSkin, howToEquip.ListTemplateAttachment[i]);
                    equipsSkin.SetAttachment(slotIndex, howToEquip.ListTemplateAttachment[i], attachment);
                }
                ListEquip();
            }
            else
            {
                int slotIndex = skeletonData.FindSlotIndex(howToEquip.slot);
                Sprite sprite = WeaponSprite != null ? WeaponSprite : GameModule.Resource.LoadAsset<Sprite>(Icon);
                if (sprite == null)
                {
                    Debug.LogError("错误,加载资源出错!");
                    return;
                }
                var attachment = GenerateAttachmentFromEquipAsset(sprite, slotIndex, howToEquip.templateSkin, howToEquip.templateAttachment);
                Equip(slotIndex, howToEquip.templateAttachment, attachment);
            }
        }
        /// <summary>
        /// 多个部位同时换装设置附件
        /// </summary>
        void ListEquip()
        {
            if (isUISpine)
            {
                Animation.Skeleton.SetSkin(equipsSkin);
            }
            else
            {
                skeletonAnimation.Skeleton.SetSkin(equipsSkin);
            }

            RefreshSkeletonAttachments();
        }
        public void Equip(int slotIndex, string attachmentName, Attachment attachment)
        {
            equipsSkin.SetAttachment(slotIndex, attachmentName, attachment);
            if (isUISpine)
            {
                Animation.Skeleton.SetSkin(equipsSkin);
            }
            else
            {
                skeletonAnimation.Skeleton.SetSkin(equipsSkin);
            }

            RefreshSkeletonAttachments();
        }
        Attachment GenerateAttachmentFromEquipAsset(Sprite sprite, int slotIndex, string templateSkinName, string templateAttachmentName)
        {

            var skeletonData = skeletonDataAsset.GetSkeletonData(true);
            var templateSkin = skeletonData.FindSkin(templateSkinName);
            Attachment templateAttachment = templateSkin.GetAttachment(slotIndex, templateAttachmentName);
            if (isUISpine)
            {
                return templateAttachment.GetRemappedClone(sprite, sourceMaterial, pivotShiftsMeshUVCoords:false,cloneMeshAsLinked:false,premultiplyAlpha: true);
            }
            else
            {
                return templateAttachment.GetRemappedClone(sprite, sourceMaterial, premultiplyAlpha: true);
            }
            
        }

        void RefreshSkeletonAttachments()
        {
            if (isUISpine)
            {
                Animation.Skeleton.SetSlotsToSetupPose();
                Animation.AnimationState.Apply(Animation.Skeleton); //skeletonAnimation.Update(0);
            }
            else
            {
                skeletonAnimation.Skeleton.SetSlotsToSetupPose();
                skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton); //skeletonAnimation.Update(0);
            }

        }

        //骨骼换装




        // Start is called before the first frame update
        void Start()
        {
            if (isUISpine)
            {
                m_Animation = GetComponent<SkeletonGraphic>();

                equipsSkin = new Skin("Equips");

                // OPTIONAL: Add all the attachments from the template skin.
                var templateSkin = m_Animation.Skeleton.Data.FindSkin(templateSkinName);
                if (templateSkin != null)
                    equipsSkin.AddAttachments(templateSkin);

                m_Animation.Skeleton.Skin = equipsSkin;
                RefreshSkeletonAttachments();
            }
            else
            {

                equipsSkin = new Skin("Equips");
                // OPTIONAL: Add all the attachments from the template skin.
                var templateSkin = skeletonAnimation.Skeleton.Data.FindSkin(templateSkinName);
                if (templateSkin != null)
                    equipsSkin.AddAttachments(templateSkin);

                skeletonAnimation.Skeleton.Skin = equipsSkin;
                RefreshSkeletonAttachments();
            }
            

        }
        private void Update()
        {
            if (isChangeAnim)
            {
                if (isUISpine)
                {
                    m_Animation.AnimationState.SetAnimation(0, AnimSr, true);
                }
                else
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, AnimSr, true);
                }
                Equip((EquipType)SpineEquipType, WeaponIcon);
                isChangeAnim =false;
            }
        }
    }
}
