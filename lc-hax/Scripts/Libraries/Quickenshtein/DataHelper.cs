using System;
using System.Runtime.CompilerServices;

namespace Quickenshtein.Internal {
    internal static class DataHelper {
        /// <summary>
        /// Fills <paramref name="targetPtr"/> with a number sequence from 1 to the length specified.
        /// </summary>
        /// <param name="targetPtr"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SequentialFill(int* targetPtr, int length) {
            int value = 0;

            while (length >= 8) {
                length -= 8;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
            }

            if (length > 4) {
                length -= 4;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
                *targetPtr = ++value;
                targetPtr++;
            }

            while (length > 0) {
                length--;
                *targetPtr = ++value;
                targetPtr++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetIndexOfFirstNonMatchingCharacter(char* sourcePtr, char* targetPtr, int sourceLength, int targetLength) {
            int searchLength = Math.Min(sourceLength, targetLength);
            int index = 0;

            while (index < searchLength && sourcePtr[index] == targetPtr[index]) {
                index++;
            }

            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void TrimLengthOfMatchingCharacters(char* sourcePtr, char* targetPtr, ref int sourceLength, ref int targetLength) {
            int searchLength = Math.Min(sourceLength, targetLength);

            sourcePtr += sourceLength - 1;
            targetPtr += targetLength - 1;

            while (searchLength > 0 && sourcePtr[0] == targetPtr[0]) {
                sourcePtr--;
                targetPtr--;
                sourceLength--;
                targetLength--;
                searchLength--;
            }
        }
    }
}
