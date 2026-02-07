import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  CardActions,
  Button,
} from "@mui/material";
import { Link, useNavigate, useParams } from "react-router";
import { useActivities } from "../../../lib/hooks/useActivities";

export default function ActivityDetails() {
  const navigate = useNavigate();
  const { id } = useParams(); // useParams is a hook that allows us to access the parameters of the current route. In this case, we are using it to access the id parameter from the URL. This id will be used to fetch the details of the specific activity that we want to display. The useParams hook returns an object with key-value pairs, where the keys are the names of the parameters defined in the route and the values are the corresponding values from the URL. By destructuring the id from the object returned by useParams, we can easily access it and use it in our component to fetch the activity details or perform any other operations that require the id.
  const { activity, isLoadingActivity } = useActivities(id);
  if (isLoadingActivity) return <Typography>Loading...</Typography>;
  if (!activity) return <Typography>No activity found</Typography>;
  return (
    <Card sx={{ borderRadius: 3 }}>
      <CardMedia
        component='img'
        src={`/images/categoryImages/${activity.category}.jpg`}
      />
      <CardContent>
        <Typography variant='h5'>{activity.title}</Typography>
        <Typography variant='subtitle1' fontWeight='light'>
          {activity.date}
        </Typography>
        <Typography variant='body1'>{activity.description}</Typography>
      </CardContent>
      <CardActions>
        {/* This approach is possible but Link does not give us the active state (if the link is selected and active) as oppose to the NavLink. We can also use the navigate hook */}
        <Button component={Link} to={`/manage/${activity.id}`} color='primary'>
          Edit
        </Button>
        <Button color='inherit' onClick={() => navigate("/activities")}>
          Cancel
        </Button>
      </CardActions>
    </Card>
  );
}
