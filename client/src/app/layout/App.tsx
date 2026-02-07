import { useState } from "react";

import { Box, Container, CssBaseline, Typography } from "@mui/material";

import NavBar from "./NavBar";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import { useActivities } from "../../lib/hooks/useActivities";

function App() {
  // In this approach we are using useEffext to get the activities and store them in a local state using the useState hook. This is not required when using Reacy Query which its implementation is defined in the custom hooks (in the lib/hooks folder) in order to extract all the logic from the component
  const { activities, isPending } = useActivities();
  // const [activities, setActivities] = useState<Activity[]>([]); // No need because of React Query
  const [selectedActivity, setSelectedActivity] = useState<
    Activity | undefined
  >(undefined);
  const [editMode, setEditMode] = useState(false);

  /* No need because of React Query
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
  */

  const handleSelectActivity = (id: string) => {
    setSelectedActivity(activities!.find((x) => x.id === id));
  };
  const handleCancelSelectActivity = () => {
    setSelectedActivity(undefined);
  };

  const handleOpenForm = (id?: string) => {
    if (id) {
      handleSelectActivity(id); // Storing the activity inside our local state
    } else {
      handleCancelSelectActivity();
    }
    setEditMode(true);
  };

  const handleFormClose = () => {
    setEditMode(false);
  };

  /* This function is not required anymore since the state of the activities is managed by the React Query
  const handleSubmitForm = (activity: Activity) => {
    // if (activity.id) {
    //   setActivities(
    //     activities.map((x) => (x.id === activity.id ? activity : x)),
    //   );
    //   setSelectedActivity(activity);
    // } else {
    //   const newActivity = { ...activity, id: activities.length.toString() };
    //   setSelectedActivity(newActivity);
    //   setActivities([...activities, newActivity]);
    // }
    console.log(activity);
    setEditMode(false); // Close the form after submittion
  };
  */

  // No need for that method. Handled by React Query
  // const handleDelete = (id: string) => {
  //   // setActivities(activities.filter((x) => x.id !== id));
  //   console.log(id);
  // };

  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      {/* Kickstart an elegant, consistent, and simple baseline to build upon. Removes the margin and padding */}
      <CssBaseline />
      <NavBar openForm={handleOpenForm} />
      <Container maxWidth='xl' sx={{ mt: 3 }}>
        {!activities || isPending ? (
          <Typography>Loading...</Typography>
        ) : (
          <ActivityDashboard
            activities={activities}
            selectActivity={handleSelectActivity}
            cancelSelectActivity={handleCancelSelectActivity}
            selectedActivity={selectedActivity}
            editMode={editMode}
            openForm={handleOpenForm}
            closeForm={handleFormClose}
            // submitForm={handleSubmitForm}
            // deleteActivity={handleDelete}
          />
        )}
      </Container>
    </Box>
  );
}

export default App;
