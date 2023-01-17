using System;
using FSM;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIBaseState : StateBase
    {
        protected ITimer timer;
        protected BaseCharacter character;
        protected Func<StateBase, bool> canExit;
        
        public AIBaseState(BaseCharacter character, bool needsExitTime) : base(needsExitTime)
        {
            this.timer = new Timer();
            this.character = character;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();

            timer.Reset();
        }
        
        public override void OnLogic()
        {
            base.OnLogic();
        }
        
        public override void OnExitRequest()
        {
            base.OnExitRequest();
            if (!needsExitTime || canExit != null && canExit(this))
            {
                fsm.StateCanExit();
            }
        }
    }
}