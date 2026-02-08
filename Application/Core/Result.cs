namespace Application.Core
{
    // The purpose of the Result class is to provide a standardized way to represent the outcome of an operation, including whether it was successful or not, any relevant data or error messages, and an optional status code. By using this class, we can handle success and failure cases in a consistent way across our application and make it easier to manage error handling and response formatting in our API controllers. The Result class has three properties: IsSuccess, which indicates whether the operation was successful or not; Value, which holds the relevant data if the operation was successful; and Error, which holds an error message if the operation failed. The class also includes two static factory methods, Success and Failure, which allow us to create instances of the Result class in a consistent and intuitive way. The Success method takes a value of type T and returns a Result instance with IsSuccess set to true and the Value property set to the provided value. The Failure method takes an error message and an optional status code and returns a Result instance with IsSuccess set to false, the Error property set to the provided error message, and the Code property set to the provided status code. By using the Result class in our handlers, we can return a standardized response that indicates whether the operation was successful or not, along with any relevant data or error messages, making it easier to manage error handling and response formatting in our API controllers.
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Value { get; set; }
        public string? Error { get; set; }
        public int Code { get; set; }

        // We are using static factory methods to create instances of the Result class. This allows us to control how instances of the Result class are created and ensure that they are always created in a consistent state. By using static factory methods, we can also provide a more intuitive and readable way to create instances of the Result class, as we can use descriptive method names like Success and Failure to indicate the intent of the code.
        // We are going to use the Result class in our handlers to return a standardized response that indicates whether the operation was successful or not, along with any relevant data or error messages. This allows us to handle success and failure cases in a consistent way across our application and makes it easier to manage error handling and response formatting in our API controllers.
        public static Result<T> Success(T value)
        {
            return new Result<T> { IsSuccess = true, Value = value };
        }
        public static Result<T> Failure(string error, int code)
        {
            return new Result<T> { IsSuccess = false, Error = error, Code = code };
        }
    }
}
