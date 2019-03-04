using System;

namespace Common {
    public static class CompareUtil {
        public static bool FloatEquals(this float number, float otherNumber) {
            return Math.Abs(number - otherNumber) < GameplayConstants.FLOATING_PRECISION;
        }
    }
}