using System.Collections;
using System.Collections.Generic;

namespace YTPTK.Core.Tests.Data
{
    public class DurationToSecondsData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {69, "PT69S"};
            yield return new object[] {60 * 6, "PT6M"};
            yield return new object[] {60 * 6 + 9, "PT6M9S"};
            yield return new object[] {60 * 60, "PT1H"};
            yield return new object[] {60 * 60 + 60 * 6, "PT1H6M"};
            yield return new object[] {60 * 60 + 60 * 6 + 9, "PT1H6M9S"};
            yield return new object[] {60 * 60 * 24, "P1DT"};
            yield return new object[] {60 * 60 * 24 + 60 * 60, "P1DT1H"};
            yield return new object[] {60 * 60 * 24 + 60 * 60 + 60 * 6, "P1DT1H6M"};
            yield return new object[] {60 * 60 * 24 + 60 * 60 + 60 * 6 + 9, "P1DT1H6M9S"};
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}