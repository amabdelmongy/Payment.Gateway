using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public abstract class Result<T>
    {
        public readonly IEnumerable<Error> Errors;
        public readonly bool IsOk;
        public readonly T Value;

        protected Result(IEnumerable<Error> errors)
        {
            IsOk = false;
            Value = default;
            Errors = errors;
        }

        protected Result()
        {
            IsOk = true;
            Value = default;
            Errors = new List<Error>();
        }

        protected Result(T value)
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
            return new ResultI<T>(value);
        }

        public static Result<T> Ok<T>()
        {
            return new ResultI<T>();
        }

        public static Result<T> Failed<T>(IEnumerable<Error> errors)
        {
            return new ResultI<T>(errors);
        }

        public static Result<T> Failed<T>(Error error)
        {
            return new ResultI<T>(new List<Error> {error});
        }

        private class ResultI<T> : Result<T>
        {
            internal ResultI(IEnumerable<Error> errors)
                : base(errors)
            {
            }

            internal ResultI()
            {
            }

            internal ResultI(T value) : base(value)
            {
            }
        }
    }
    public class Error
    { 
        private Error(string subject, Exception exception, string message)
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
