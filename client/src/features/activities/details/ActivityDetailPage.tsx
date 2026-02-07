import { Typography, Grid } from "@mui/material";
import { useParams } from "react-router";
import { useActivities } from "../../../lib/hooks/useActivities";
import ActivityDetailsHeader from "./ActivityDetailsHeader";
import ActivityDetailsInfo from "./ActivityDetailsInfo";
import ActivityDetailsChat from "./ActivityDetailsChat";
import ActivityDetailsSidebar from "./ActivityDetailsSidebar";

export default function ActivityDetailPage() {
  const { id } = useParams(); // useParams is a hook that allows us to access the parameters of the current route. In this case, we are using it to access the id parameter from the URL. This id will be used to fetch the details of the specific activity that we want to display. The useParams hook returns an object with key-value pairs, where the keys are the names of the parameters defined in the route and the values are the corresponding values from the URL. By destructuring the id from the object returned by useParams, we can easily access it and use it in our component to fetch the activity details or perform any other operations that require the id.
  const { activity, isLoadingActivity } = useActivities(id);
  if (isLoadingActivity) return <Typography>Loading...</Typography>;
  if (!activity) return <Typography>No activity found</Typography>;
  return (
    <Grid container spacing={3}>
      <Grid size={8}>
        <ActivityDetailsHeader activity={activity} />
        <ActivityDetailsInfo activity={activity} />
        <ActivityDetailsChat />
      </Grid>
      <Grid size={4}>
        <ActivityDetailsSidebar />
      </Grid>
    </Grid>
  );
}
