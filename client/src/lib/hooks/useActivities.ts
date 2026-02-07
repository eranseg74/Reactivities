import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import agent from "../api/agent";

export const useActivities = () => {
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
      await agent.post("/activities", activity);
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
    isPending,
    updateActivity,
    createActivity,
    deleteActivity,
  };
};
