using Application.Activities.Commands;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators;

// The CreateActivityValidator class is responsible for validating the data received when creating a new activity. It inherits from the BaseActivityValidator class, which provides common validation rules for both creating and editing activities. The CreateActivityValidator constructor calls the base constructor with a lambda expression that specifies how to access the CreateActivityDto object from the CreateActivity.Command. This allows the validator to apply the defined validation rules to the properties of the CreateActivityDto when a create activity command is executed. By using FluentValidation, we can ensure that the incoming data for creating an activity meets the specified criteria before it is processed further in the application.
public class CreateActivityValidator : BaseActivityValidator<CreateActivity.Command, CreateActivityDto>
{
    public CreateActivityValidator() : base(x => x.ActivityDto)
    {

    }
}
