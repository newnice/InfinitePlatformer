using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class JumpFromCliffState : InAirState {
        private Vector2 _cliffPoint;

        public JumpFromCliffState(BoxCollider2D player, PlayerMovement movement, Vector2 cliffPoint) : base(
            player, movement) {
            _cliffPoint = cliffPoint;
        }

        public override State Move(bool isVerbose) {
            if (_cliffPoint.x.FloatGreater(PlayerLeftBottom.x-GroundEdgePrecision) && !IsObstacle()) {
                Movement.Move(1, false, false);
                return this;
            }

            return base.Move(isVerbose);
        }
    }
}