namespace Application.Activities.DTOs;

public class BaseActivityDto
{
    // Specifying the 'required' modifier before the object type (string) is useful for nullable reference type but gives us an issue when it comes to data validation because the API controller tries to construct an activity based on the JSON object it receives from the client and when in encounters the required modifier it does not have the ability to format the response the way that we want when it comes to validating the data. So, to handle that we remove all the required keywords and add the [Required] data annotation. When removing the required keyword we will get a warning that the property might be null so we set a default propery of an empty string (""). This does not matter because the [Required] annotation will force the existance of that field.
    // Because we are using the FluentValidation library to validate the incoming data, we do not need to use the data annotations for validation. However, if we want to use the data annotations for validation, we can add the [Required] attribute to the properties that we want to validate. This will ensure that when the API controller receives a request with missing required fields, it will return a 400 Bad Request response with a message indicating which fields are required. We do not want it here because it will occur before Fluent and we will miss the Fluent validation. e do not need to worry that the client will send empty data because it will be validated by Fluent.
    // [Required]
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // [Required]
    public string Description { get; set; } = string.Empty;
    // [Required]
    public string Category { get; set; } = string.Empty;

    // location details
    // [Required]
    public string City { get; set; } = string.Empty;
    // [Required]
    public string Venue { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
