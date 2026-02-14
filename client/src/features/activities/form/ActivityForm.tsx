import { Paper, Typography, Box, Button } from '@mui/material';
import { useActivities } from '../../../lib/hooks/useActivities';
import { useNavigate, useParams } from 'react-router';
import { useForm } from 'react-hook-form';
import { useEffect } from 'react';
import {
  activitySchema,
  type ActivitySchema,
} from '../../../lib/schemas/activitySchema';
import { zodResolver } from '@hookform/resolvers/zod';
import TextInput from '../../../app/shared/components/TextInput';
import SelectInput from '../../../app/shared/components/SelectInput';
import { categoryOptions } from './categoryOptions';
import DateTimeInput from '../../../app/shared/components/DateTimeInput';
import LocationInput from '../../../app/shared/components/LocationInput';

export default function ActivityForm() {
  const { control, reset, handleSubmit } = useForm<ActivitySchema>({
    mode: 'onTouched',
    resolver: zodResolver(activitySchema),
  });
  const navigate = useNavigate();
  const { id } = useParams();
  const { updateActivity, createActivity, activity, isLoadingActivity } =
    useActivities(id);

  useEffect(() => {
    if (activity) {
      reset({
        ...activity,
        location: {
          venue: activity.venue,
          city: activity.city,
          latitude: activity.latitude,
          longitude: activity.longitude,
        },
      });
    }
  }, [activity, reset]);

  // We need to flatten the location object before sending it to the API, because the API expects the venue, city, latitude and longitude to be at the root level of the activity object, and not nested inside a location object. So we destructure the location object from the data, and then we create a new object that spreads the rest of the data (which contains the title, description, category and date), and then we spread the location object to get its properties at the root level of the new object. This way, we can send the flattenedData object to the API, which will have all the properties in the correct format.
  const onSubmit = async (data: ActivitySchema) => {
    const { location, ...rest } = data;
    const flattenedData = {
      ...rest,
      ...location,
    };
    try {
      if (activity) {
        updateActivity.mutate(
          { ...activity, ...flattenedData } as unknown as Activity,
          {
            onSuccess: () => {
              navigate(`/activities/${activity.id}`);
            },
          },
        );
      } else {
        createActivity.mutate(flattenedData as unknown as Activity, {
          onSuccess: (id) => {
            navigate(`/activities/${id}`);
          },
        });
      }
    } catch (error) {
      console.log(error);
    }
  };

  if (isLoadingActivity) {
    return <Typography>Loading activity...</Typography>;
  }

  return (
    <Paper sx={{ borderRadius: 3, padding: 3 }}>
      <Typography variant='h5' gutterBottom color='primary'>
        {activity ? 'Edit activity' : 'Create activity'}
      </Typography>
      <Box
        component='form'
        onSubmit={handleSubmit(onSubmit)}
        display='flex'
        flexDirection='column'
        gap={3}>
        <TextInput label='Title' control={control} name='title' />
        <TextInput
          label='Description'
          multiline
          rows={3}
          control={control}
          name='description'
        />
        <Box display='flex' gap={3}>
          <SelectInput
            items={categoryOptions}
            label='Category'
            control={control}
            name='category'
          />
          <DateTimeInput label='Date' control={control} name='date' />
          {/* <TextInput label='City' control={control} name='city' />
        <TextInput label='Venue' control={control} name='venue' /> */}
        </Box>
        <LocationInput
          control={control}
          label='Enter the location'
          name='location'
        />
        <Box display='flex' justifyContent='end' gap={3}>
          <Button color='inherit' onClick={() => {}}>
            Cancel
          </Button>
          <Button
            type='submit'
            color='success'
            variant='contained'
            disabled={updateActivity.isPending || createActivity.isPending}
            loading={updateActivity.isPending || createActivity.isPending}>
            Submit
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
