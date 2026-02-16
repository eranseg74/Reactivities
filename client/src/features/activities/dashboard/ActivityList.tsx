import { Box, Typography } from '@mui/material';
import ActivityCard from './ActivityCard';
import { useActivities } from '../../../lib/hooks/useActivities';
import { useInView } from 'react-intersection-observer';
import { useEffect } from 'react';
import { observer } from 'mobx-react-lite';

const ActivityList = observer(function ActivityList() {
  const { activitiesGroup, isLoading, hasNextPage, fetchNextPage } =
    useActivities();
  // Threshold - Number between 0 and 1 indicating the percentage that should be visible before triggering. Can also be an array of numbers, to create multiple trigger points.
  const { ref, inView } = useInView({ threshold: 0.5 });

  useEffect(() => {
    if (inView && hasNextPage) {
      fetchNextPage();
    }
  }, [fetchNextPage, hasNextPage, inView]);

  if (isLoading) return <Typography>Loading...</Typography>;
  if (!activitiesGroup) return <Typography>No activities found</Typography>;

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: 3 }}>
      {activitiesGroup.pages.map((activities, index) => (
        <Box
          key={index}
          // Applying the ref only to the last page
          ref={index === activitiesGroup.pages.length - 1 ? ref : null}
          display='flex'
          flexDirection='column'
          gap={3}>
          {activities.items.map((activity) => (
            <ActivityCard key={activity.id} activity={activity as Activity} />
          ))}
        </Box>
      ))}
    </Box>
  );
});

export default ActivityList;
