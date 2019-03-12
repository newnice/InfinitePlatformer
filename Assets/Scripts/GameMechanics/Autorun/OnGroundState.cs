using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class OnGroundState : State {
        private Vector2 _groundEnd = Vector2.negativeInfinity;

        protected override bool IsGroundPassed(Vector2 ground) {
            return PlayerLeftBottom.x + GroundEdgePrecision >= ground.x;
        }

        public override State Move(bool isVerbose) {
            var collidersUnder = GetCollidersUnder();
            if (Vector2.negativeInfinity.Equals(_groundEnd) || IsGroundPassed(_groundEnd)) {
                if (IsGroundUnderNow(collidersUnder, out var groundCollider))
                    _groundEnd = GetColliderRightTop(groundCollider);
            }


            if (IsEnemyUnderNow(collidersUnder, out var enemyPosition)) {
                if(isVerbose) Debug.Log($"Enemy under so: JumpFromCliffState: point = {enemyPosition}");
                Movement.Move(1, true, false);
                return new JumpFromCliffState(PlayerCollider, Movement, enemyPosition);
            }

            var obstacle = IsObstacle();
            if (IsGroundPassed(_groundEnd)) {
                Vector2 next = CheckLongestGroundUnder(collidersUnder);
                if (!Vector2.negativeInfinity.Equals(next) && _groundEnd.x < next.x) {
                    Movement.Move(1, false, false);
                    if(isVerbose) Debug.Log($"WalkFromPromontoryState: point = {_groundEnd}");
                    return new WalkFromPromontoryState(PlayerCollider, Movement, _groundEnd);
                }
            }


            var nearestUpper = CheckUpperGround();

            if (!Vector2.positiveInfinity.Equals(nearestUpper)) {
                Movement.Move(1, true, false);
                if(isVerbose) Debug.Log($"JumpToUpperGroundState: nearestUpper = {nearestUpper}");
                return new JumpToUpperGroundState(PlayerCollider, Movement, nearestUpper);
            }


            if (!obstacle && !IsGroundPassed(_groundEnd)) {
                Movement.Move(1, false, false);
                return this;
            }

            Movement.Move(1, true, false);
            if(isVerbose) Debug.Log($"JumpFromCliffState: point = {(obstacle ? PlayerLeftBottom : _groundEnd)}");
            return new JumpFromCliffState(PlayerCollider, Movement, obstacle ? PlayerLeftBottom : _groundEnd);
        }


        public OnGroundState(BoxCollider2D player, PlayerMovement movement) : base(player, movement) { }
    }
}