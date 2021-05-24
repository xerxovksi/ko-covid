namespace KO.Covid.Application
{
    using KO.Covid.Application.Exceptions;
    using System;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;

    public class ExecutionPolicy
    {
        public T WithRetry<T>(
            Func<T> operation,
            Func<T, bool> isSuccessful,
            int maximumRetryCount,
            Exception exceptionIfMaximumRetriesExceeded = null,
            bool shouldThrowIfMaximumRetriesExceeded = false)
        {
            var intervalPeriod = 500;
            var retryCount = 0;

            while (retryCount < maximumRetryCount)
            {
                ExceptionDispatchInfo exceptionInfo = null;
                try
                {
                    var result = operation();
                    if (isSuccessful(result))
                    {
                        return result;
                    }

                    Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }
                catch (AggregateException exception)
                {
                    exceptionInfo = ExceptionDispatchInfo.Capture(exception);
                    Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }
                catch (Exception exception)
                {
                    exceptionInfo = ExceptionDispatchInfo.Capture(exception);
                    Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }

                if (exceptionInfo != null
                    && retryCount.Equals(maximumRetryCount))
                {
                    exceptionInfo.Throw();
                }
            }

            if (exceptionIfMaximumRetriesExceeded != null)
            {
                throw exceptionIfMaximumRetriesExceeded;
            }

            if (shouldThrowIfMaximumRetriesExceeded)
            {
                throw new MaximumRetryExceededException(maximumRetryCount);
            }

            return default;
        }

        public async Task<T> WithRetryAsync<T>(
            Func<Task<T>> operation,
            Func<T, bool> isSuccessful,
            int maximumRetryCount,
            Exception exceptionIfMaximumRetriesExceeded = null,
            bool shouldThrowIfMaximumRetriesExceeded = false)
        {
            var intervalPeriod = 500;
            var retryCount = 0;

            while (retryCount < maximumRetryCount)
            {
                ExceptionDispatchInfo exceptionInfo = null;
                try
                {
                    var result = await operation();
                    if (isSuccessful(result))
                    {
                        return result;
                    }

                    await Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }
                catch (AggregateException exception)
                {
                    exceptionInfo = ExceptionDispatchInfo.Capture(exception);
                    await Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }
                catch (Exception exception)
                {
                    exceptionInfo = ExceptionDispatchInfo.Capture(exception);
                    await Task.Delay((int)Math.Pow(2, ++retryCount) * intervalPeriod);
                }

                if (exceptionInfo != null
                    && retryCount.Equals(maximumRetryCount))
                {
                    exceptionInfo.Throw();
                }
            }

            if (exceptionIfMaximumRetriesExceeded != null)
            {
                throw exceptionIfMaximumRetriesExceeded;
            }

            if (shouldThrowIfMaximumRetriesExceeded)
            {
                throw new MaximumRetryExceededException(maximumRetryCount);
            }

            return default;
        }
    }
}