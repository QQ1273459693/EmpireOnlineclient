using UnityEngine;
using Spine.Unity;
using Cysharp.Threading.Tasks;
using Spine;
using GameLogic;
using Spine.Unity.AttachmentTools;

namespace TEngine
{
    public class UnitUiLoader : ReferenceDisposer
    {
        SpineEquipmentSBO SpineDataEquip;
        //动画组件
        protected SkeletonGraphic m_Animation;
        public SkeletonGraphic Animation
        {
            get { return m_Animation; }
        }
        GameObject ItemGO;
        /// <summary>
        /// 初始化骨骼动画
        /// </summary>
        /// <param name="go"></param>
        public void Load(GameObject go)
        {
            ItemGO = go;
            m_Animation = go.GetComponent<SkeletonGraphic>();
            SpineDataEquip = GameModule.Resource.LoadAsset<SpineEquipmentSBO>("SpineAnimaConfig");

        }
        public void Equip(int EquipID)
        {
            var EquipBase = ConfigLoader.Instance.Tables.TbEquipment.Get(EquipID);
            if (EquipBase==null)
            {
                Log.Debug("出错,找不到装备ID");
                return;
            }
            GameLogic.EquipHook howToEquip = null;
            switch (EquipBase.EquipType)
            {
                case GameConfig.equipment.EquipmentFlag.Sword:
                    howToEquip = SpineDataEquip.equippables[0];
                    break;
                case GameConfig.equipment.EquipmentFlag.ShIeId:
                    howToEquip = SpineDataEquip.equippables[1];
                    break;
                case GameConfig.equipment.EquipmentFlag.Armor:
                    howToEquip = SpineDataEquip.equippables[2];
                    break;
                case GameConfig.equipment.EquipmentFlag.Shoulder:
                    howToEquip = SpineDataEquip.equippables[3];
                    break;
                case GameConfig.equipment.EquipmentFlag.Foot:
                    howToEquip = SpineDataEquip.equippables[4];
                    break;
                case GameConfig.equipment.EquipmentFlag.Helmet:
                    howToEquip = SpineDataEquip.equippables[5];
                    break;
                case GameConfig.equipment.EquipmentFlag.Arm:
                    howToEquip = SpineDataEquip.equippables[6];
                    break;
            }
            var skeletonData = SpineDataEquip.skeletonDataAsset.GetSkeletonData(true);

            var templateSkin = skeletonData.FindSkin(howToEquip.templateSkin);
            for (int i = 0; i < howToEquip.slot.Count; i++)
            {
                int slotIndex = skeletonData.FindSlotIndex(howToEquip.slot[i]);
                Sprite sprite = GameModule.Resource.LoadAsset<Sprite>(EquipBase.SpineIcon[i]);
                if (sprite == null)
                {
                    Debug.LogError("错误,加载资源出错!");
                    return;
                }
                string AttachmentSr = howToEquip.templateAttachment[i];
                var attachment = GenerateAttachmentFromEquipAsset(sprite, slotIndex, howToEquip.templateSkin, AttachmentSr);
                //RegionAttachment attachment1 = attachment as RegionAttachment;
                ////attachment1.Rotation = 100;
                //Log.Debug($"插槽:{attachment.Name},旋转角度:{attachment1.Rotation},X:{attachment1.X},Y:{attachment1.Y}");

                templateSkin.SetAttachment(slotIndex, AttachmentSr, attachment);
            }
            Animation.Skeleton.SetSkin(templateSkin);
            RefreshSkeletonAttachments();
        }
        void RefreshSkeletonAttachments()
        {
            Animation.Skeleton.SetSlotsToSetupPose();
            Animation.AnimationState.Apply(Animation.Skeleton);
        }
        Attachment GenerateAttachmentFromEquipAsset(Sprite sprite, int slotIndex, string templateSkinName, string templateAttachmentName)
        {
            var skeletonData = SpineDataEquip.skeletonDataAsset.GetSkeletonData(true);
            var templateSkin = skeletonData.FindSkin(templateSkinName);
            Attachment templateAttachment = templateSkin.GetAttachment(slotIndex, templateAttachmentName);
            return templateAttachment.GetRemappedClone(sprite, SpineDataEquip.sourceMaterial, pivotShiftsMeshUVCoords: false, cloneMeshAsLinked: false, premultiplyAlpha: true);
        }
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}