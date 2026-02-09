import { useEffect, useMemo, useState } from 'react';
import {
  useController,
  type FieldValues,
  type UseControllerProps,
} from 'react-hook-form';
import type { LocationIQSuggestion } from '../../../lib/types/index.d.ts';
import {
  Box,
  debounce,
  List,
  ListItemButton,
  TextField,
  Typography,
} from '@mui/material';
import axios from 'axios';

type Props<T extends FieldValues> = { label: string } & UseControllerProps<T>;

export default function LocationInput<T extends FieldValues>(props: Props<T>) {
  const { field, fieldState } = useController({ ...props });
  const [loading, setLoading] = useState(false);
  const [suggestions, setSuggestions] = useState<LocationIQSuggestion[]>([]);
  const [inputValue, setInputValue] = useState(field.value || '');

  // We use useEffect here to update the inputValue state whenever the field.value changes. This is necessary because when we select a suggestion from the list, we will update the field.value with the selected suggestion's display_name, and we want to reflect that change in the input field. If we don't do this, the input field will not update with the selected suggestion, and it will still show the previous value until the user types something else. By using useEffect, we ensure that the input field always shows the current value of field.value, which is what we want for a controlled component. We also check if the field.value is an object (which it will be when we select a suggestion, because we will set the field.value to the entire suggestion object), and if it is, we set the inputValue to the suggestion's venue property (or an empty string if it doesn't exist). If the field.value is not an object, we just set the inputValue to the field.value directly. This way, we can handle both cases: when the user types in the input field (field.value will be a string), and when the user selects a suggestion (field.value will be an object).
  useEffect(() => {
    if (field.value && typeof field.value === 'object') {
      setInputValue(field.value.venue || '');
    } else {
      setInputValue(field.value || '');
    }
  }, [field.value]);

  const locationUrl =
    'https://api.locationiq.com/v1/autocomplete?key=pk.241eeb2abaa398ce5a20032858c25ced&limit=5&dedupe=1&';

  // We use useMemo to create a debounced version of the fetchSuggestions function, which will only be called after the user has stopped typing for 500ms. This is to avoid making too many API calls while the user is typing. The debounce function is from lodash, and it takes a function and a delay as arguments, and returns a new function that will only call the original function after the delay has passed since the last time it was called. We also use useMemo to ensure that the debounced function is only created once, and not on every render, which would defeat the purpose of debouncing. We also include locationUrl in the dependency array of useMemo, so that if the locationUrl changes, a new debounced function will be created with the new URL.
  // Theoretically, we use useCallback here, but since the function we are creating is a debounced function, which is a new function every time it is created, it doesn't make sense to use useCallback, because it would return a new function every time anyway. So we use useMemo instead, which will only create a new debounced function when the locationUrl changes.
  const fetchSuggestions = useMemo(
    () =>
      debounce(async (query: string) => {
        // debounce will return a new function that will only call the original function after the user has stopped typing for 500ms, which is useful to avoid making too many API calls while the user is typing
        if (!query || query.length < 3) {
          setSuggestions([]);
          return;
        }
        setLoading(true);
        try {
          const res = await axios.get<LocationIQSuggestion[]>(
            `${locationUrl}q=${query}`,
          );
          setSuggestions(res.data);
        } catch (e) {
          console.error('Error fetching suggestions:', e);
        } finally {
          setLoading(false);
        }
      }, 500),
    [locationUrl],
  );

  const handleChange = async (value: string) => {
    field.onChange(value);
    await fetchSuggestions(value);
  };

  // This function will be called when the user clicks on a suggestion from the list. It will take the selected suggestion, extract the city, venue, latitude, and longitude from it, and then update the form field with this information. It will also set the inputValue to the selected suggestion's display_name, so that it shows in the input field, and it will clear the suggestions list. We extract the city from the suggestion's address, which can be in different properties (city, town, or village), so we check for all of them. The venue is the display_name of the suggestion, and the latitude and longitude are also extracted from the suggestion. We then call field.onChange with an object that contains the city, venue, latitude, and longitude, which will update the form state with this information. Finally, we clear the suggestions list by setting it to an empty array.
  const handleSelect = (location: LocationIQSuggestion) => {
    const city =
      location.address?.city ||
      location.address?.town ||
      location.address?.village;
    const venue = location.display_name;
    const latitude = location.lat;
    const longitude = location.lon;
    setInputValue(venue);
    field.onChange({ city, venue, latitude, longitude });
    setSuggestions([]);
  };

  return (
    <Box>
      <TextField
        {...props}
        value={inputValue}
        onChange={(e) => handleChange(e.target.value)}
        fullWidth
        variant='outlined'
        error={!!fieldState.error}
        helperText={fieldState.error?.message}
      />
      {loading && <Typography>Loading...</Typography>}
      {suggestions.length > 0 && (
        <List sx={{ border: 1 }}>
          {suggestions.map((suggestion) => (
            <ListItemButton
              divider
              key={suggestion.place_id}
              onClick={() => handleSelect(suggestion)}>
              {suggestion.display_name}
            </ListItemButton>
          ))}
        </List>
      )}
    </Box>
  );
}
