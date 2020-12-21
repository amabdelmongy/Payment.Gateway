using System;
using System.Collections.Generic;

namespace Domain
{
    public class Result<T>
    {
        public readonly IEnumerable<Error> Errors;
        public readonly bool IsOk;
        public readonly T Value;

        internal Result(IEnumerable<Error> errors)
        {
            IsOk = false;
            Value = default;
            Errors = errors;
        }

        internal Result()
        {
            IsOk = true;
            Value = default;
            Errors = new List<Error>();
        }

        internal Result(T value)
        {
            IsOk = true;
            Value = value;
            Errors = new List<Error>();
        }

        public bool HasErrors => !IsOk;
    }

    public static class Result
    {
        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> Ok<T>()
        {
            return new Result<T>();
        }

        public static Result<T> Failed<T>(IEnumerable<Error> errors)
        {
            return new Result<T>(errors);
        }

        public static Result<T> Failed<T>(Error error)
        {
            return new Result<T>(new List<Error> {error});
        } 
    }

    public class Error
    {
        protected Error(string subject, Exception exception, string message)
        {
            Subject = subject;
            Message = message;
            Exception = exception;
        }

        public Exception Exception { get; }
        public string Subject { get; }
        public string Message { get; }

        public static Error CreateFrom(string subject, string message = null)
        {
            return new Error(subject, null, message);
        }

        public static Error CreateFrom(string subject, Exception exception)
        {
            return new Error(subject, exception, exception.Message);
        }
    }
}
