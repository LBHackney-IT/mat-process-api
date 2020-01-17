using System;

namespace mat_process_api.Tests.V1.Helper
{
    public static class ErrorThrowerHelper
    {
        private static int _numberOfCases = 12;

        public static void GenerateError(int randomNumber, string randomString)
        {
            int randomErrorNumber = Math.Abs(randomNumber % _numberOfCases);
            string str = randomString;

            switch (randomErrorNumber)
            {
                case 0: throw new OutOfMemoryException(str);
                case 1: throw new IndexOutOfRangeException(str);
                case 2: GenerateInnerArgumentNullException(str); break;
                case 3: throw new ArgumentOutOfRangeException(str);
                case 4: GenerateInnerApplicationException(str); break;
                case 5: GenerateInnerInvalidCastException(str); break;
                case 6: throw new MissingFieldException(str);
                case 7: throw new OverflowException(str);
                case 8: GenerateInnerSystemException(str); break;
                case 9: throw new TimeoutException(str);
                case 10: throw new StackOverflowException(str);
                default: GenerateInnerAggregateException(str); break;
            }
        }

        private static void GenerateInnerSystemException(string randomMessageAddon)
        {
            try { throw new SystemException("Exception in ThrowInner method." + randomMessageAddon); }
            catch (SystemException e) { throw new SystemException("Error in CatchInner caused by calling the ThrowInner method.", e); }
        }

        private static void GenerateInnerInvalidCastException(string randomMessageAddon)
        {
            try { throw new InvalidCastException("Exception in ThrowInner method." + randomMessageAddon); }
            catch (InvalidCastException e) { throw new InvalidCastException("Error in CatchInner caused by calling the ThrowInner method.", e); }
        }

        private static void GenerateInnerArgumentNullException(string randomMessageAddon)
        {
            try { throw new ArgumentNullException("Exception in ThrowInner method." + randomMessageAddon); }
            catch (ArgumentNullException e) { throw new ArgumentNullException("Error in CatchInner caused by calling the ThrowInner method.", e); }
        }

        private static void GenerateInnerApplicationException(string randomMessageAddon)
        {
            try { throw new ApplicationException("Exception in ThrowInner method." + randomMessageAddon); }
            catch (ApplicationException e) { throw new ApplicationException("Error in CatchInner caused by calling the ThrowInner method.", e); }
        }

        private static void GenerateInnerAggregateException(string randomMessageAddon)
        {
            try { throw new AggregateException("Exception in ThrowInner method." + randomMessageAddon); }
            catch (AggregateException e) { throw new AggregateException("Error in CatchInner caused by calling the ThrowInner method.", e); }
        }
    }
}
