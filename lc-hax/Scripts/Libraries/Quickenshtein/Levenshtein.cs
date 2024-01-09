using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Quickenshtein.Internal;

namespace Quickenshtein {
    /// <summary>
    /// A modified Levenshtein Distance Calculator based on https://github.com/Turnerj/Quickenshtein
    /// </summary>
    public static partial class Levenshtein {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int GetDistance(ReadOnlySpan<char> source, ReadOnlySpan<char> target) {
            int sourceLength = source.Length;
            int targetLength = target.Length;

            if (sourceLength == 0) {
                return targetLength;
            }
            if (targetLength == 0) {
                return sourceLength;
            }

            fixed (char* sourcePtr = source)
            fixed (char* targetPtr = target) {
                return CalculateDistance(sourcePtr, targetPtr, sourceLength, targetLength);
            }
        }

        private static unsafe int CalculateDistance(char* sourcePtr, char* targetPtr, int sourceLength, int targetLength) {
            //Identify and trim any common prefix or suffix between the strings
            int offset = DataHelper.GetIndexOfFirstNonMatchingCharacter(sourcePtr, targetPtr, sourceLength, targetLength);
            sourcePtr += offset;
            targetPtr += offset;
            sourceLength -= offset;
            targetLength -= offset;
            DataHelper.TrimLengthOfMatchingCharacters(sourcePtr, targetPtr, ref sourceLength, ref targetLength);

            //Check the trimmed values are not empty
            if (sourceLength == 0) {
                return targetLength;
            }
            if (targetLength == 0) {
                return sourceLength;
            }

            //Switch around variables so outer loop has fewer iterations
            if (targetLength < sourceLength) {
                char* tempSourcePtr = sourcePtr;
                sourcePtr = targetPtr;
                targetPtr = tempSourcePtr;

                (sourceLength, targetLength) = (targetLength, sourceLength);
            }


            int[] pooledArray = ArrayPool<int>.Shared.Rent(targetLength);

            fixed (int* previousRowPtr = pooledArray) {
                DataHelper.SequentialFill(previousRowPtr, targetLength);

                int rowIndex = 0;

                CalculateRows_4Rows(previousRowPtr, sourcePtr, sourceLength, ref rowIndex, targetPtr, targetLength);

                //Calculate Single Rows
                for (; rowIndex < sourceLength; rowIndex++) {
                    int lastSubstitutionCost = rowIndex;
                    int lastInsertionCost = rowIndex + 1;

                    char sourcePrevChar = sourcePtr[rowIndex];

                    CalculateRow(previousRowPtr, targetPtr, targetLength, sourcePrevChar, lastInsertionCost, lastSubstitutionCost);
                }

                int result = previousRowPtr[targetLength - 1];
                ArrayPool<int>.Shared.Return(pooledArray);
                return result;
            }
        }
    }
}
