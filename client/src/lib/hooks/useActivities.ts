import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";

export const useActivities = (id?: string) => {
  const queryClient = useQueryClient();
  // Implementing React Query:
  // useQuery accepts an object with two properties: queryKey and queryFn. queryKey is a unique key that identifies the query, and queryFn is a function that returns a promise which resolves to the data we want to fetch. We can change the queryKey to trigger a refetch of the data. The useQuery hook returns an object with several properties, including data, error, and isLoading. We can use these properties to handle the state of our application while the data is being fetched.
  // We can change the data name to anything we want, but it is common to use data. We can also destructure the data object to get the activities directly. The queryKey is an array that contains a string that identifies the query. In this case, we are using "activities" as the queryKey. The queryFn is an asynchronous function that uses axios to fetch the activities from the API and returns the data.
  const { data: activities, isPending } = useQuery({
    queryKey: ["activities"],
    queryFn: async () => {
      const response = await agent.get<Activity[]>("/activities");
      return response.data;
    },
  });

  const { data: activity, isLoading: isLoadingActivity } = useQuery({
    queryKey: ["activities", id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/activities/${id}`);
      return response.data;
    },
    // The enabled property is used to conditionally enable or disable the query. In this case, we only want to fetch the activity if the id is provided. If the id is not provided, the query will be disabled and will not run. This is useful to prevent unnecessary API calls when we don't have the necessary information to fetch the data. Otherswise, if we don't use the enabled property, the query will run multiple times even when the id is not provided, which will result in an error because the API endpoint requires an id to fetch a specific activity. The !! operator is used to convert the id to a boolean value. If the id is a non-empty string, it will be truthy and the query will be enabled. If the id is an empty string or undefined, it will be falsy and the query will be disabled. This way, we ensure that the query only runs when we have a valid id to fetch the activity. This is particularly useful in scenarios where we might be rendering a component that relies on an id that may not be immediately available, such as when navigating to a details page for an activity. By using the enabled property, we can prevent unnecessary API calls and handle loading states more effectively.
    enabled: !!id,
  });

  // useMutation is a hook that allows us to perform mutations, such as creating, updating, or deleting data. It accepts an object with two properties: mutationFn and onSuccess. mutationFn is a function that performs the mutation, and onSuccess is a function that is called when the mutation is successful. In this case, we are using useMutation to update an activity. The mutationFn is an asynchronous function that uses axios to send a PUT request to the API with the updated activity. The onSuccess function invalidates the "activities" query, which triggers a refetch of the activities data.
  const updateActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      await agent.put("/activities", activity);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  const createActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      const response = await agent.post("/activities", activity);
      return response.data; // Note that the create API call as impementerd in the server, returns the activity id. We want to return it so we can use it to dispay the activity after creation
    },
    onSuccess: async () => {
      queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  const deleteActivity = useMutation({
    mutationFn: async (id: string) => {
      // Note that here we need to pass the id as a param and not a property like thecreate and update methods where we passed the activities!
      await agent.delete(`/activities/${id}`);
    },
    onSuccess: async () => {
      queryClient.invalidateQueries({
        queryKey: ["activities"],
      });
    },
  });

  // To make use of the data in this hook we return it as an object so any component that will use the useActivities hook will be able to use these properties
  return {
    activities,
    activity,
    isLoadingActivity,
    isPending,
    updateActivity,
    createActivity,
    deleteActivity,
  };
};
