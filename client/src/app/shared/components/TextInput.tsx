import { TextField, type TextFieldProps } from '@mui/material';
import {
  useController,
  type FieldValues,
  type UseControllerProps,
} from 'react-hook-form';

// This component is a wrapper around the MUI TextField component that integrates it with react-hook-form. It uses the useController hook to connect the TextField to the form state and validation. The props for this component include all the props for the MUI TextField, as well as the props for the useController hook, which are used to specify which field in the form this TextField is connected to.
// In the Props type, we use a generic type T that extends FieldValues, which allows us to specify the type of the form values when we use this component. This is important for type safety, as it ensures that the name prop passed to the TextInput component corresponds to a valid field in the form values.
// In short, we are saying that we extend the FieldValues proprties with the UseControllerProps and the TextFieldProps, so that we can use all the props from both of these types when we use the TextInput component. This allows us to easily integrate the TextField with react-hook-form while still having access to all the features of the MUI TextField component.
type Props<T extends FieldValues> = {} & UseControllerProps<T> & TextFieldProps; // This is a TypeScript utility type that combines the props for the useController hook with the props for the MUI TextField component. This allows us to pass all the necessary props to both the useController hook and the TextField component when we use this TextInput component.

export default function TextInput<T extends FieldValues>(props: Props<T>) {
  const { field, fieldState } = useController({ ...props }); // This hook connects the TextField to the form state and validation. It returns an object with two properties: field, which contains the props that need to be passed to the TextField component to connect it to the form, and fieldState, which contains information about the validation state of the field, such as whether there are any errors. The props passed to the useController hook include all the props for the useController hook, which are specified in the Props type, as well as any additional props that are passed to the TextInput component when it is used.
  return (
    <TextField
      {...field}
      {...props}
      value={field.value || ''} // This sets the value of the TextField component to the current value of the field in the form state. If the field value is undefined or null, it defaults to an empty string to ensure that the TextField component is controlled and does not throw a warning about being uncontrolled.
      fullWidth
      variant='outlined'
      error={!!fieldState.error}
      helperText={fieldState.error?.message}
    />
  );
}
