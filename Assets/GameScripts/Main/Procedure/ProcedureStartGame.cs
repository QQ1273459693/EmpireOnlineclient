using System;
using Cysharp.Threading.Tasks;
using GameLogic;
using TEngine;

namespace GameMain
{
    public class ProcedureStartGame : ProcedureBase
    {
        public override bool UseNativeDialog { get; }
        private IFsm<IProcedureManager> m_procedureOwner;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_procedureOwner= procedureOwner;
            SpineHelper.PreloadInit();
            StartGame().Forget();
            GameEvent.AddEventListener(GameProcedureEvent.LoadMainStateEvent.EventId, LoadMainSence);
        }
        private void LoadMainSence()
        {
            ChangeState<ProcedureMainSence>(m_procedureOwner);
        }

        private async UniTaskVoid StartGame()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f));
            UILoadMgr.HideAll();
        }
        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            GameEvent.RemoveEventListener(GameProcedureEvent.LoadMainStateEvent.EventId, LoadMainSence);
        }
    }
}