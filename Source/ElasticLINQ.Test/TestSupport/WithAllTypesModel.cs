// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;

namespace ElasticLinq.Test.TestSupport
{
    class WithAllTypes
    {
        public string String { get; set; }

        public int Int { get; set; }
        public int? IntNullable { get; set; }
        public long Long { get; set; }
        public long? LongNullable { get; set; }

        public float Float { get; set; }
        public float? FloatNullable { get; set; }
        public double Double { get; set; }
        public double? DoubleNullable { get; set; }
        public decimal Decimal { get; set; }
        public decimal? DecimalNullable { get; set; }

        public DateTime DateTime { get; set; }
        public DateTime? DateTimeNullable { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public DateTimeOffset? DateTimeOffsetNullable { get; set; }

        public static IEnumerable<WithAllTypes> CreateSequence(int count)
        {
            for (var i = 1; i <= count; i++)
                yield return CreateNumbered(i);
        }

        public static WithAllTypes CreateNumbered(int number)
        {
            var isOdd = number % 2 == 1;
            var oddNumber = isOdd ? (int?)null : number;
            var date = new DateTime(2014, 12, 31).AddDays(number);

            return new WithAllTypes
            {
                String = number.ToString(),
                Int = number,
                IntNullable = oddNumber,
                Long = number,
                LongNullable = oddNumber,
                Float = number,
                FloatNullable = oddNumber,
                Double = number,
                DoubleNullable = oddNumber,
                Decimal = number,
                DecimalNullable = oddNumber,
                DateTime = date,
                DateTimeNullable = isOdd ? (DateTime?)null : date,
                DateTimeOffset = date,
                DateTimeOffsetNullable = isOdd ? (DateTimeOffset?)null : date
            };
        }
    }
}