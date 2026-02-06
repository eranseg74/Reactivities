import { List, ListItem, ListItemText, Typography } from "@mui/material";
import axios from "axios";
import { useEffect, useState } from "react";

function App() {
  const [activities, setActivities] = useState<Activity[]>([]);

  useEffect(() => {
    // Using axios. Axios will automatically convert to json and since we defined the returned type the response will be of type Activity[]
    axios<Activity[]>("https://localhost:5001/api/activities").then(
      (response) => setActivities(response.data),
    );
    // Using the native JS fetch command
    // fetch("https://localhost:5001/api/activities")
    //   .then((response) => response.json())
    //   .then((data) => setActivities(data));
  }, []);

  return (
    <>
      <Typography variant='h3'>Reactivities</Typography>
      <List>
        {activities.map((activity) => (
          <ListItem key={activity.id}>
            <ListItemText>{activity.title}</ListItemText>
          </ListItem>
        ))}
      </List>
    </>
  );
}

export default App;
