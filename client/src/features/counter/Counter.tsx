import {
  Box,
  Button,
  ButtonGroup,
  List,
  ListItemText,
  Paper,
  Typography,
} from "@mui/material";
import { useStore } from "../../lib/hooks/useStore";
import { observer } from "mobx-react-lite";

// Using Observer / observer (depends on the approach) allows the component to automatically re-render whenever the observable state it uses changes, without having to manually manage subscriptions or use other state management techniques. By wrapping the relevant JSX in an Observer component, we can ensure that our Counter component will update correctly whenever the count or title properties of the counterStore change. This is a key feature of MobX that allows for efficient and reactive state management in React applications.

// Using the observer function as a high order function:
const Counter = observer(() => {
  const { counterStore } = useStore();
  return (
    <Box display='flex' justifyContent='space-between'>
      <Box sx={{ width: "60%" }}>
        <Typography variant='h4' gutterBottom>
          {counterStore.title}
        </Typography>
        <Typography variant='h6'>The count is: {counterStore.count}</Typography>

        <ButtonGroup sx={{ mt: 3 }}>
          <Button
            variant='contained'
            color='error'
            onClick={() => counterStore.decrement()}
          >
            Decrement
          </Button>
          <Button
            variant='contained'
            color='success'
            onClick={() => counterStore.increment()}
          >
            Increment
          </Button>
          <Button
            variant='contained'
            color='primary'
            onClick={() => counterStore.increment(5)}
          >
            Increment by 5
          </Button>
        </ButtonGroup>
      </Box>
      <Paper sx={{ width: "40%", p: 4 }}>
        {/* Dispaying the computed propery */}
        <Typography variant='h5'>
          Counter events: ({counterStore.eventCount})
        </Typography>
        <List>
          {counterStore.events.map((event, index) => (
            <ListItemText key={index}>{event}</ListItemText>
          ))}
        </List>
      </Paper>
    </Box>
  );
});

export default Counter;

// Using Observer as an element wrapper around the JSX that needs to react to changes in the observable state.
/*
export default function Counter() {
  const { counterStore } = useStore();
  return (
    <>
      <Observer>
        {() => (
          <>
            <Typography variant='h4' gutterBottom>
              {counterStore.title}
            </Typography>
            <Typography variant='h6'>
              The count is: {counterStore.count}
            </Typography>
          </>
        )}
      </Observer>
      <ButtonGroup sx={{ mt: 3 }}>
        <Button
          variant='contained'
          color='error'
          onClick={() => counterStore.decrement()}
        >
          Decrement
        </Button>
        <Button
          variant='contained'
          color='success'
          onClick={() => counterStore.increment()}
        >
          Increment
        </Button>
        <Button
          variant='contained'
          color='primary'
          onClick={() => counterStore.increment(5)}
        >
          Increment by 5
        </Button>
      </ButtonGroup>
    </>
  );
}
*/
