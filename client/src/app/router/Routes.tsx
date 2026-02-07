import { createBrowserRouter } from "react-router";
import App from "../layout/App";
import HomePage from "../../features/home/HomePage";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import ActivityForm from "../../features/activities/form/ActivityForm";
import ActivityDetailPage from "../../features/activities/details/ActivityDetailPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      { path: "", element: <HomePage /> },
      // Note that we used to pass arguments to the ActivityDashboard and the ActivityForm. This causes an error here because these elements require properties. We could pass here props but it is better to avoid that
      { path: "activities", element: <ActivityDashboard /> },
      { path: "activities/:id", element: <ActivityDetailPage /> },
      // Note that we are using the same component for both creating and editing an activity. We can differentiate between the two cases by checking if the id parameter is present in the URL. If the id parameter is present, it means that we are editing an existing activity, and if it is not present, it means that we are creating a new activity. This is a common pattern when we want to use the same form component for both creating and editing an entity. By using the same component, we can avoid code duplication and keep our codebase cleaner. In the ActivityForm component, we can use the useParams hook to access the id parameter from the URL and determine whether we are in create or edit mode. Based on that, we can fetch the existing activity data if we are in edit mode or initialize an empty form if we are in create mode.
      // We also added a key to the createActivity route to force the component to remount when we navigate to the create activity page. This is necessary because when we navigate from the edit activity page to the create activity page, the ActivityForm component does not unmount and remount, it just updates the props. This causes an issue because the form fields are populated with the data of the activity we were editing, and when we navigate to the create activity page, we want the form fields to be empty. By adding a key to the route, we force the component to remount and reset its state, which allows us to have an empty form when we navigate to the create activity page.
      { path: "createActivity", element: <ActivityForm key='create' /> },
      { path: "manage/:id", element: <ActivityForm /> },
    ],
  },
]);
