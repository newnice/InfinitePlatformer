using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class WalkFromPromontoryState : InAirState {
        private Vector2 _promontoryPoint;
        

        public WalkFromPromontoryState(BoxCollider2D player, PlayerMovement movement, Vector2 promontoryPoint) : base(player,
            movement) {
            _promontoryPoint = promontoryPoint;
        }

        public override State Move(bool isVerbose) {
            if (_promontoryPoint.x.FloatGreater(PlayerLeftBottom.x - GroundEdgePrecision ) ) {
                Movement.Move(1, false, false);
                return this;
            }

            return base.Move(isVerbose);
        }
    }
}