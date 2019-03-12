using Common;
using Player;
using UnityEngine;

namespace GameMechanics.Autorun {
    public class InAirState : State {
        protected Vector2 _groundUnderEnd = Vector2.negativeInfinity;
        public new float GroundEdgePrecision = 0.1f;

        public InAirState(BoxCollider2D player, PlayerMovement movement) : base(
            player, movement) { }


        protected override bool IsGroundPassed(Vector2 ground) {
            return PlayerRightBottom.x - GroundEdgePrecision >= ground.x;
        }


        public override State Move(bool isVerbose) {
            var collidersUnder = GetCollidersUnder();
            var groundPoint = Vector2.negativeInfinity;
            if (IsGroundUnderNow(collidersUnder, out var currentGround)) {
                groundPoint = GetColliderRightTop(currentGround);
            }

            if (groundPoint.x.FloatGreater(PlayerLeftBottom.x)) {
                Movement.Move(1, false, false);
                if (isVerbose) Debug.Log($"Change state on OnGroundState in point {groundPoint} ");
                return new OnGroundState(PlayerCollider, Movement);
            }

            if (IsEnemyUnderNow(collidersUnder, out var enemyPosition)) {
                if (isVerbose) Debug.Log($"Enemy under so: JumpFromCliffState: point = {enemyPosition}");
                Movement.Move(1, true, false);
                return new JumpFromCliffState(PlayerCollider, Movement, enemyPosition);
            }


            var longestGround = CheckLongestGroundUnder(collidersUnder);
            if (!IsGroundPassed(longestGround) && longestGround.x.FloatGreater(_groundUnderEnd.x)) {
                _groundUnderEnd = longestGround;
            }


            if (!IsGroundPassed(_groundUnderEnd)) {
                Movement.Move(1, false, false);
                return this;
            }


            if (IsEnemyUnderForFooting(collidersUnder, out enemyPosition)) {
                if (enemyPosition.x.FloatGreater(PlayerPosition.x - GroundEdgePrecision))
                    Movement.Move(1, false, false);
                else
                    Movement.Move(0, false, false);
                return this;
            }

            if (IsOnlyKillZone(collidersUnder)) {
                Movement.Move(1, false, false);
                return this;
            }

            Movement.Move(0, false, false);
            return this;
        }
    }
}