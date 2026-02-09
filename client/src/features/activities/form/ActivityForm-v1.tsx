import { Paper, Typography, Box, TextField, Button } from "@mui/material";
import type { SubmitEvent } from "react";
import { useActivities } from "../../../lib/hooks/useActivities";
import { useNavigate, useParams } from "react-router";

export default function ActivityForm() {
  const { id } = useParams();
  const { updateActivity, createActivity, activity, isLoadingActivity } =
    useActivities(id);
  const navigate = useNavigate();
  // The type of the event is SubmitEvent and the generic type is HTMLFormElement since this event is triggered on a form element
  const handleSubmit = async (event: SubmitEvent<HTMLFormElement>) => {
    event.preventDefault();
    // FormData is a built-in JS class that allows us to easily extract the data from a form element. We need to pass the form element as an argument to the constructor and we can get the form element from the currentTarget property of the event object whicj is the element that the event is triggered on. In this case, since the event is triggered on a form element, the currentTarget will be the form element.
    const formData = new FormData(event.currentTarget);
    // FormData is an iterable object that contains the form data as key-value pairs. We can use the forEach method to iterate over the form data and extract the data into a plain object. The forEach method takes a callback function that is called for each key-value pair in the form data. The callback function takes two arguments: the value and the key of the current key-value pair. We can use these arguments to populate our plain object with the form data. The FormDataEntryValue type is a union type that represents the possible types of the values in the form data. The value can be either a string or a File object, depending on the type of the form field. In our case, since we are only using text fields, the value will always be a string.
    // The key of the form data is the name attribute of the form field, so we need to make sure that we set the name attribute for each form field in our form. This will allow us to easily extract the form data using the FormData API.
    const data: { [key: string]: FormDataEntryValue } = {};
    formData.forEach((value, key) => {
      data[key] = value;
    });
    if (activity) {
      // In case of activity update
      data.id = activity.id;

      // The mutate is a mutation function we can call with variables to trigger the mutation and optionally hooks on additional callback options.
      // @param variables — The variables object to pass to the mutationFn.
      // @param options.onSuccess — This function will fire when the mutation is successful and will be passed the mutation's result.
      // @param options.onError — This function will fire if the mutation encounters an error and will be passed the error.
      // @param options.onSettled — This function will fire when the mutation is either successfully fetched or encounters an error and be passed either the data or error.
      // @remarks
      // If we make multiple requests, onSuccess will fire only after the latest call you've made.
      // All the callback functions (onSuccess, onError, onSettled) are void functions, and the returned value will be ignored.
      // Here we are using the async version of the mutate function because we want to close the form after the mutation finished.
      // Since the data object is of type { [key: string]: FormDataEntryValue }, we need to cast it to the Activity type before we can pass it to the submitForm function. We can use the as keyword to perform a type assertion and tell TypeScript that we know that the data object is of type Activity. This is necessary because the submitForm function expects an argument of type Activity, and without the type assertion, TypeScript would give us an error since it cannot guarantee that the data object is of the correct type. We use the unknown type as an intermediate step in the type assertion to bypass TypeScript's type checking and allow us to assert that the data object is of type Activity. This is a common pattern when we need to perform a type assertion on an object that has a more general type, such as { [key: string]: FormDataEntryValue }, and we want to assert that it is of a more specific type, such as Activity. What it does is it first asserts that the data object is of type unknown, which is a type that can represent any value, and then it asserts that the unknown value is of type Activity. This allows us to bypass TypeScript's type checking and assert that the data object is of the correct type without getting an error.
      // submitForm(data as unknown as Activity);
      await updateActivity.mutateAsync(data as unknown as Activity);
      navigate(`/activities/${activity.id}`);
    } else {
      // Here we are not using async version of the mutate function because we want to navigate to the activity details page after the mutation is successful, and we can achieve that by using the onSuccess callback of the mutate function. The onSuccess callback is called after the mutation is successful and it receives the result of the mutation as an argument. In this case, since our createActivity mutation returns the id of the created activity, we can use that id to navigate to the activity details page after the activity is created.
      // If we were to use the async version of the mutate function, we would have to wait for the mutation to complete before we can navigate to the activity details page, which would make the user experience less smooth. By using the onSuccess callback, we can navigate to the activity details page immediately after the mutation is successful without having to wait for the mutation to complete.
      createActivity.mutate(data as unknown as Activity, {
        onSuccess: (id) => {
          navigate(`/activities/${id}`);
        },
      });
    }
  };

  if (isLoadingActivity) {
    return <Typography>Loading activity...</Typography>;
  }

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant='h5' gutterBottom color='primary'>
        {activity ? "Edit activity" : "Create activity"}
      </Typography>
      <Box
        component='form'
        onSubmit={handleSubmit}
        display='flex'
        flexDirection='column'
        gap={3}
      >
        <TextField name='title' label='Title' defaultValue={activity?.title} />
        <TextField
          name='description'
          label='Description'
          defaultValue={activity?.description}
          multiline
          rows={3}
        />
        <TextField
          name='category'
          label='Category'
          defaultValue={activity?.category}
        />
        <TextField
          name='date'
          label='Date'
          defaultValue={
            activity?.date
              ? new Date(activity.date + "Z").toISOString().split("T")[0]
              : new Date().toISOString().split("T")[0]
          }
          type='date'
        />
        <TextField name='city' label='City' defaultValue={activity?.city} />
        <TextField name='venue' label='Venue' defaultValue={activity?.venue} />
        <Box display='flex' justifyContent='end' gap={3}>
          <Button color='inherit' onClick={() => {}}>
            Cancel
          </Button>
          {/* When we define the button type as 'submit' this means that clicking on the button will execute the onSubmit function in the form element */}
          <Button
            type='submit'
            color='success'
            variant='contained'
            disabled={updateActivity.isPending || createActivity.isPending}
            loading={updateActivity.isPending || createActivity.isPending}
          >
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
