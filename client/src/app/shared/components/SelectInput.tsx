import {
  FormControl,
  FormHelperText,
  InputLabel,
  MenuItem,
  Select,
  type SelectProps,
} from '@mui/material';
import {
  useController,
  type FieldValues,
  type UseControllerProps,
} from 'react-hook-form';

// T extends FieldValues - This means that the generic type T must be a subtype of FieldValues, which is a type provided by react-hook-form that represents the shape of the form values. By using this constraint, we ensure that when we use the SelectInput component, we can only specify a type for T that is compatible with the form values expected by react-hook-form. This helps to maintain type safety and ensures that the name prop passed to the SelectInput component corresponds to a valid field in the form values.
// {} & UseControllerProps<T> & Se - This is a TypeScript utility type that combines the props for the useController hook with the props for the MUI TextField component. The {} at the beginning is an empty object type, which means that we are not adding any additional props to this type (we could add additional props unique to our needs). The & operator is used to combine multiple types into one, so by using {} & UseControllerProps<T> & Se, we are creating a new type that includes all the properties from both UseControllerProps<T> and Se. This allows us to pass all the necessary props to both the useController hook and the TextField component when we use this TextInput component.
// Summary - This code defines a TypeScript type called Props that combines the properties of the useController hook from react-hook-form and the Se from Material-UI. The generic type T extends FieldValues, which means that it can be any type that is compatible with the form values expected by react-hook-form. This Props type is then used as the type for the props of the SelectInput component, which is currently a placeholder component that returns a simple div with the text "SelectInput".
// It gives us the ability to create a SelectInput component that can be easily integrated with react-hook-form while still having access to all the features of the MUI TextField component, such as validation and error handling, by using the useController hook to connect the SelectInput to the form state and validation.
// It gives us the ability to combine the logic of the form (useController) and the looks of the MUI component (SelectInput in this case) all in one component
type Props<T extends FieldValues> = {
  items: { text: string; value: string }[]; // This is an additional prop that we are adding to the Props type, which is an array of objects that represent the options for the Select component. Each object has a value property, which is the value that will be submitted when the form is submitted, and a label property, which is the text that will be displayed in the dropdown menu for that option. By including this items prop in our Props type, we can easily pass the options for the Select component when we use the SelectInput component.
  label: string; // This is another additional prop that we are adding to the Props type, which is a string that represents the label for the Select component. This label will be displayed above the Select component and will be associated with it for accessibility purposes. By including this label prop in our Props type, we can easily specify the label for the Select component when we use the SelectInput component.
} & UseControllerProps<T> &
  Partial<SelectProps>; // This is a TypeScript utility type that combines the props for the useController hook with the props for the MUI Select component. This allows us to pass all the necessary props to both the useController hook and the Select component when we use this SelectInput component.
// Partial<SelectProps> - This makes all the props from the MUI Select component optional, which means that when we use the SelectInput component, we can choose to pass any of the props from the Select component without being required to pass all of them. This gives us flexibility when using the SelectInput component, as we can customize the behavior and appearance of the Select component by passing different props, while still having the core functionality of integrating with react-hook-form through the useController hook.

export default function SelectInput<T extends FieldValues>(props: Props<T>) {
  // Extract the field and fieldState from the useController hook, which connects the Select component to the form state and validation. The props passed to the useController hook include all the props for the useController hook, which are specified in the Props type, as well as any additional props that are passed to the SelectInput component when it is used (such as the items prop).
  const { field, fieldState } = useController({ ...props });
  return (
    // This component is a wrapper around the MUI Select component that integrates it with react-hook-form. It uses the useController hook to connect the Select component to the form state and validation. The props for this component include all the props for the MUI Select, as well as the props for the useController hook, which are used to specify which field in the form this Select component is connected to. The items prop is used to pass the options for the Select component when we use this SelectInput component. The !!fieldState.error is used to determine if there are any validation errors for this field, and if so, it will display the error message using the FormHelperText component. If true, the label is displayed in an error state.
    <FormControl fullWidth error={!!fieldState.error}>
      <InputLabel id={`${props.label}-label`}>{props.label}</InputLabel>
      <Select
        value={field.value || ''} // This sets the value of the Select component to the current value of the field in the form state. If the field value is undefined or null, it defaults to an empty string to ensure that the Select component is controlled and does not throw a warning about being uncontrolled.
        onChange={field.onChange} // This sets the onChange handler of the Select component to the onChange function provided by the useController hook. This allows the Select component to update the form state when the user selects a different option from the dropdown menu.
        labelId={`${props.label}-label`} // This sets the labelId prop of the Select component to a unique value based on the label prop. This is used to associate the InputLabel with the Select component for accessibility purposes. The InputLabel component has a corresponding id that matches this labelId, which allows screen readers to correctly associate the label with the Select component. The label prop is used to specify the text that will be displayed as the label for the Select component, and it is passed to the InputLabel component to display the label above the Select component. The items prop is used to pass the options for the Select component when we use this SelectInput component. Each option is rendered as a MenuItem within the Select component, with the value and text specified in the items array.
        label={props.label}>
        {props.items.map((item) => (
          // This maps over the items array passed as a prop to the SelectInput component and renders a MenuItem for each option in the Select dropdown menu. The key prop is set to the value of the item to ensure that each MenuItem has a unique key, and the value prop is set to the value of the item, which will be submitted when the form is submitted. The text of the MenuItem is set to the text property of the item, which is what will be displayed in the dropdown menu for that option.
          <MenuItem key={item.value} value={item.value}>
            {item.text}
          </MenuItem>
        ))}
      </Select>
      {/* This checks if there are any validation errors for this field by checking if fieldState.error is truthy. If there are errors, it renders a FormHelperText component that displays the error message. The error message is accessed through fieldState.error.message, which is provided by the useController hook when there are validation errors for the field. This allows us to display user-friendly error messages below the Select component when the user input does not meet the validation criteria defined in our form schema. */}
      {fieldState.error && (
        <FormHelperText>{fieldState.error.message}</FormHelperText>
      )}
    </FormControl>
  );
}
