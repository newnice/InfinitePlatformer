using System;

namespace Common {
    public static class CompareUtil {
        public static bool FloatEquals(this float number, float otherNumber) {
            return Math.Abs(number - otherNumber) < GameplayConstants.FLOATING_PRECISION;
        }

        public static bool FloatGreater(this float number, float otherNumber) {
            return number-otherNumber > GameplayConstants.FLOATING_PRECISION ;
        }
    }
}