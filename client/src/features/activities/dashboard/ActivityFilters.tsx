import { FilterList, Event } from '@mui/icons-material';
import {
  Box,
  Paper,
  Typography,
  MenuList,
  MenuItem,
  ListItemText,
} from '@mui/material';
import 'react-calendar/dist/Calendar.css';
import Calendar from 'react-calendar';
import { useStore } from '../../../lib/hooks/useStore';
import { observer } from 'mobx-react-lite';

// Since we are using MobX we need to set this component as observer.
// This will allow this componenrt to observe the filter and the startDate properties from the useStore.
// Note that since we are not using here the useActivity this means that changes on the filter or startDate will not have the desired effect on this component. We also need to make the ActivityList an observer since it does uses the useActivity hook.
const ActivityFilters = observer(function ActivityFilters() {
  const {
    activityStore: { filter, setFilter, startDate, setStartDate },
  } = useStore();
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        gap: 3,
        borderRadius: 3,
      }}>
      <Paper sx={{ p: 3, borderRadius: 3 }}>
        <Box sx={{ width: '100%' }}>
          <Typography
            variant='h6'
            sx={{
              display: 'flex',
              alignItems: 'center',
              mb: 1,
              color: 'primary.main',
            }}>
            <FilterList sx={{ mr: 1 }} />
            Filters
          </Typography>
          <MenuList>
            <MenuItem
              selected={filter === 'all'}
              onClick={() => setFilter('all')}>
              <ListItemText primary='All events' />
            </MenuItem>
            <MenuItem
              selected={filter === 'isGoing'}
              onClick={() => setFilter('isGoing')}>
              <ListItemText primary="I'm going" />
            </MenuItem>
            <MenuItem
              selected={filter === 'isHost'}
              onClick={() => setFilter('isHost')}>
              <ListItemText primary="I'm hosting" />
            </MenuItem>
          </MenuList>
        </Box>
      </Paper>
      <Box component={Paper} sx={{ width: '100%', p: 3, borderRadius: 3 }}>
        <Typography
          variant='h6'
          sx={{
            display: 'flex',
            alignItems: 'center',
            mb: 1,
            color: 'primary.main',
          }}>
          <Event sx={{ mr: 1 }} />
          Select date
        </Typography>
        {/* Casting to Date to avoid the warning: Argument of type 'Value' is not assignable to parameter of type 'Date'. Type 'null' is not assignable to type 'Date' */}
        <Calendar
          value={startDate}
          onChange={(date) => setStartDate(date as Date)}
        />
      </Box>
    </Box>
  );
});

export default ActivityFilters;
