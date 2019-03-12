using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class JumpToUpperGroundState : InAirState {
        private Vector2 _requiredPointToFly;

        public JumpToUpperGroundState(BoxCollider2D player, PlayerMovement movement, Vector2 requiredPointToFly) : base(
            player, movement) {
            _requiredPointToFly = requiredPointToFly;
        }

        public override State Move(bool isVerbose) {
            var distance = _requiredPointToFly - PlayerRightBottom;
            if (distance.x > 0 && Movement.CurrentVelocity.y > 0) {
                if (distance.x.FloatGreater(distance.y))
                    Movement.Move(1, false, false);
                else {
                    Movement.Move(0, false, false);
                }

                return this;
            }

            return base.Move(isVerbose);
        }
    }
}