using System;
using Cysharp.Threading.Tasks;
using TEngine;
using YooAsset;

namespace GameMain
{
    public class ProcedureMainSence : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        private IFsm<IProcedureManager> fsm;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            fsm=procedureOwner;
            GameEvent.AddEventListener(GameLogic.GameProcedureEvent.LoadFightStateEvent.EventId, ChangeFightState);
            Log.Debug("正式进入主场景");
            GameModule.Resource.LoadSceneAsync("MainCity").Completed+=StartMain;
            
            
        }
        void StartMain(SceneOperationHandle handle)
        {
            GameEvent.Send(GameLogic.GameProcedureEvent.LoadMainCityUIEvent.EventId);
        }
        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEvent.RemoveEventListener(GameLogic.GameProcedureEvent.LoadFightStateEvent.EventId, ChangeFightState);
            ExitMain().Forget();
        }
        private async UniTaskVoid ExitMain()
        {
            await UniTask.DelayFrame(2);
            UILoadMgr.HideAll();
            GameModule.UI.CloseAll();
        }
        private void ChangeFightState()
        {
            ChangeState<ProcedureFightSence>(fsm);
        }
    }
}