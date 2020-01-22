using Bogus;
using System;

namespace mat_process_api.Tests.V1.Helper
{
    public static class ErrorThrowerHelper
    {
        private static int _numberOfCases = 12;
        private static Faker _faker = new Faker();

        public static Exception GenerateError()
        {
            int randomErrorNumber = _faker.Random.Int(0, _numberOfCases - 1); //triangulating unexpected exceptions
            string str = _faker.Random.Hash().ToString(); //triangulating error messages

            switch (randomErrorNumber)
            {
                case 0: return new OutOfMemoryException(str);
                case 1: return new IndexOutOfRangeException(str);
                case 2: return GenerateInnerArgumentNullException(str);
                case 3: return new ArgumentOutOfRangeException(str);
                case 4: return GenerateInnerApplicationException(str);
                case 5: return GenerateInnerInvalidCastException(str);
                case 6: return new MissingFieldException(str);
                case 7: return new OverflowException(str);
                case 8: return GenerateInnerSystemException(str);
                case 9: return new TimeoutException(str);
                case 10: return new StackOverflowException(str);
                default: return GenerateInnerAggregateException(str);
            }
        }

        private static SystemException GenerateInnerSystemException(string randomMessageAddon)
        {
            try { throw new SystemException("Inner exception thrown." + randomMessageAddon); }
            catch (SystemException e) { return new SystemException("Outer exception thrown.", e); }
        }

        private static InvalidCastException GenerateInnerInvalidCastException(string randomMessageAddon)
        {
            try { throw new InvalidCastException("Inner exception thrown." + randomMessageAddon); }
            catch (InvalidCastException e) { return new InvalidCastException("Outer exception thrown.", e); }
        }

        private static ArgumentNullException GenerateInnerArgumentNullException(string randomMessageAddon)
        {
            try { throw new ArgumentNullException("Inner exception thrown." + randomMessageAddon); }
            catch (ArgumentNullException e) { return new ArgumentNullException("Outer exception thrown.", e); }
        }

        private static ApplicationException GenerateInnerApplicationException(string randomMessageAddon)
        {
            try { throw new ApplicationException("Inner exception thrown." + randomMessageAddon); }
            catch (ApplicationException e) { return new ApplicationException("Outer exception thrown.", e); }
        }

        private static AggregateException GenerateInnerAggregateException(string randomMessageAddon)
        {
            try { throw new AggregateException("Inner exception thrown." + randomMessageAddon); }
            catch (AggregateException e) { return new AggregateException("Outer exception thrown.", e); }
        }
    }
}
