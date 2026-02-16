import {
  keepPreviousData,
  useInfiniteQuery,
  useMutation,
  useQuery,
  useQueryClient,
} from '@tanstack/react-query';
import agent from '../api/agent';
import { useLocation } from 'react-router';
import { useAccount } from './useAccount';
import { useStore } from './useStore';

export const useActivities = (id?: string) => {
  const {
    activityStore: { filter, startDate },
  } = useStore();
  const queryClient = useQueryClient();
  const { currentUser } = useAccount();
  const location = useLocation();
  // Implementing React Query:
  // useQuery accepts an object with two properties: queryKey and queryFn. queryKey is a unique key that identifies the query, and queryFn is a function that returns a promise which resolves to the data we want to fetch. We can change the queryKey to trigger a refetch of the data. The useQuery hook returns an object with several properties, including data, error, and isLoading. We can use these properties to handle the state of our application while the data is being fetched.
  // We can change the data name to anything we want, but it is common to use data. We can also destructure the data object to get the activities directly. The queryKey is an array that contains a string that identifies the query. In this case, we are using "activities" as the queryKey. The queryFn is an asynchronous function that uses axios to fetch the activities from the API and returns the data.
  // Changes after adding pagination:
  // Changing the hook from useQuery to useInfiniteQuery. This hook supports cursor-based pagination which is very efficient for large datasets. The common pagination when the user jumps between pages is the Offset Pagination.
  // We define the next cursor as string although the server expects a date but that is Ok since the server can also handle date iso strings.
  const {
    data: activitiesGroup,
    isLoading,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
  } = useInfiniteQuery<PagedList<Activity, string>>({
    // Adding the filter and the startDate as parameters in the query key. Otherwise they will not work since they all will have the same key.
    queryKey: ['activities', filter, startDate],
    queryFn: async ({ pageParam = null }) => {
      // Initializing the pageParam as null since in the first page it is null
      // We want this function to execute whenever one of the properties in the params is changed. Since it is not possible to make the hook observe the store, the idea is that any component that will use this hook an observer of the store, and that will make the hook an observer, so whenever one of the parameters will change, the hook is going to react to that change and execute the function with the updated parameters
      const response = await agent.get<PagedList<Activity, string>>(
        '/activities',
        {
          params: {
            cursor: pageParam,
            pageSize: 3,
            filter,
            startDate,
          },
        },
      );
      return response.data;
    },
    // Additional required properties for the useInfiniteQuery:
    staleTime: 1000 * 60 * 5, // Defining that the data in the cache is valid for 5 minutes. This way the data will not be refreshed whenever we are re-rendering this component (like switching to another tab and returning to this tab)
    placeholderData: keepPreviousData, // This will keep the previous data on until the new data is fetched and not reset the screen.
    initialPageParam: null, // Required but we do not do anything with it
    getNextPageParam: (lastPage) => lastPage.nextCursor, // Defining the value of the pageParam that is passed to the cursor.
    // The staleTime property is used to specify how long the data is considered fresh. During this time, React Query will not refetch the data when the component mounts or when the queryKey changes. This can help improve performance by reducing unnecessary API calls. In this case, we set the staleTime to 10 seconds (1000 milliseconds * 10 seconds), which means that the data will be considered fresh for 10 seconds after it is fetched. After 10 seconds, if the component mounts or if the queryKey changes, React Query will refetch the data from the API. The default staleTime is 0, which means that the data is considered stale immediately after it is fetched, and React Query will refetch the data every time the component mounts or when the queryKey changes. By setting a longer staleTime, we can reduce the number of API calls and improve the performance of our application, especially if the data does not change frequently. However, we should be careful when setting a long staleTime, as it may lead to displaying outdated data if the data changes on the server and the client does not refetch it for a long time.
    // staleTime: 1000 * 10,
    // Select is a property that allows us to transform the data before it is returned to the component. In this case, we are using the select property to add two properties to each activity: isHost and isGoing. The isHost property is set to true if the current user is the host of the activity, and the isGoing property is set to true if the current user is going to the activity. We can use these properties in our components to display different UI elements based on whether the user is the host or a participant of the activity. By using the select property, we can keep our components clean and avoid having to perform these calculations in multiple places throughout our application.
    enabled: !id && location.pathname === '/activities' && !!currentUser,
    select: (data) => ({
      ...data,
      pages: data.pages.map((page) => ({
        ...page,
        items: page.items.map((activity) => {
          const host = activity.attendees.find((x) => x.id === activity.hostId);
          return {
            ...activity,
            isHost: currentUser?.id === activity.hostId,
            isGoing: activity.attendees.some((x) => x.id === currentUser?.id),
            hostImageUrl: host?.imageUrl,
          };
        }),
      })),
    }),
    // The enabled property is used to conditionally enable or disable the query. In this case, we want to enable the query when the component mounts and we are on the main activities page, so we set it to true. If we set it to false, the query will not run and the data will not be fetched from the API. This can be useful in scenarios where we want to fetch data based on certain conditions, such as when a user clicks a button or when a specific event occurs. By using the enabled property, we can control when the query runs and avoid unnecessary API calls. In this case, we want to fetch the list of activities only when we are on the main activities page and not when we are on the details page for a specific activity, which is why we check the location.pathname and the presence of the id parameter.
    // We also check if there is a currentUser because if the user is not logged in we don't want him to see the activities and even more, we do not want React Query to run unnecessary fetches from the server
  });

  const { data: activity, isLoading: isLoadingActivity } = useQuery({
    queryKey: ['activities', id],
    queryFn: async () => {
      const response = await agent.get<Activity>(`/activities/${id}`);
      return response.data;
    },
    // The enabled property is used to conditionally enable or disable the query. In this case, we only want to fetch the activity if the id is provided. If the id is not provided, the query will be disabled and will not run. This is useful to prevent unnecessary API calls when we don't have the necessary information to fetch the data. Otherswise, if we don't use the enabled property, the query will run multiple times even when the id is not provided, which will result in an error because the API endpoint requires an id to fetch a specific activity. The !! operator is used to convert the id to a boolean value. If the id is a non-empty string, it will be truthy and the query will be enabled. If the id is an empty string or undefined, it will be falsy and the query will be disabled. This way, we ensure that the query only runs when we have a valid id to fetch the activity. This is particularly useful in scenarios where we might be rendering a component that relies on an id that may not be immediately available, such as when navigating to a details page for an activity. By using the enabled property, we can prevent unnecessary API calls and handle loading states more effectively.
    enabled: !!id && !!currentUser,
    select: (data) => {
      const host = data.attendees.find((x) => x.id === data.hostId);
      return {
        ...data,
        isHost: currentUser?.id === data.hostId,
        isGoing: data.attendees.some((x) => x.id === currentUser?.id),
        hostImageUrl: host?.imageUrl,
      };
    },
  });

  // useMutation is a hook that allows us to perform mutations, such as creating, updating, or deleting data. It accepts an object with two properties: mutationFn and onSuccess. mutationFn is a function that performs the mutation, and onSuccess is a function that is called when the mutation is successful. In this case, we are using useMutation to update an activity. The mutationFn is an asynchronous function that uses axios to send a PUT request to the API with the updated activity. The onSuccess function invalidates the "activities" query, which triggers a refetch of the activities data.
  const updateActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      await agent.put('/activities', activity);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ['activities'],
      });
    },
  });

  const createActivity = useMutation({
    mutationFn: async (activity: Activity) => {
      const response = await agent.post('/activities', activity);
      return response.data; // Note that the create API call as impementerd in the server, returns the activity id. We want to return it so we can use it to dispay the activity after creation
    },
    onSuccess: async () => {
      queryClient.invalidateQueries({
        queryKey: ['activities'],
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
        queryKey: ['activities'],
      });
    },
  });

  const updateAttendance = useMutation({
    mutationFn: async (id?: string) => {
      await agent.post(`/activities/${id}/attend`);
    },
    // Optimistic updating
    onMutate: async (activityId: string | undefined) => {
      // Cancelling any running queries from the activity controller, on the selected activity:
      await queryClient.cancelQueries({ queryKey: ['activities', activityId] });
      // Getting the activity from the cache
      const prevActivity = queryClient.getQueryData<Activity>([
        'activities',
        activityId,
      ]);

      // The setQueryData function is used to update the cached data for a specific query. It accepts two arguments: the queryKey and an updater function. The queryKey is an array that identifies the query we want to update, and the updater function receives the current cached data as an argument and returns the new data that we want to cache. In this case, we are updating the activity in the cache by toggling the isCancelled property if the current user is the host, and by adding or removing the current user from the attendees array based on whether they are currently attending or not. This allows us to optimistically update the UI before the mutation is completed, providing a better user experience.
      queryClient.setQueryData<Activity>(
        ['activities', activityId],
        (oldActivity) => {
          if (!oldActivity || !currentUser) {
            return oldActivity;
          }
          const isHost = oldActivity.hostId === currentUser.id;
          const isAttending = oldActivity.attendees.some(
            (x) => x.id === currentUser.id,
          );
          return {
            ...oldActivity,
            isCancelled: isHost
              ? !oldActivity.isCancelled
              : oldActivity.isCancelled,
            attendees: isAttending
              ? isHost
                ? oldActivity.attendees
                : oldActivity.attendees.filter((x) => x.id !== currentUser.id)
              : [
                  ...oldActivity.attendees,
                  {
                    id: currentUser.id,
                    displayName: currentUser.displayName,
                    imageUrl: currentUser.imageUrl,
                  },
                ],
          };
        },
      );
      return { prevActivity };
    },
    // If the mutation fails, we want to roll back to the previous state of the activity. The onError function is called when the mutation fails, and it receives the error, the variables passed to the mutationFn, and the context returned from the onMutate function. In this case, we check if there is a prevActivity in the context, and if there is, we set the query data for the activity back to the prevActivity, effectively rolling back to the previous state of the activity before the optimistic update was applied.
    onError: (error, activityId, context) => {
      console.log(error);
      if (context?.prevActivity) {
        queryClient.setQueryData(
          ['activities', activityId],
          context.prevActivity,
        );
      }
    },
  });

  // To make use of the data in this hook we return it as an object so any component that will use the useActivities hook will be able to use these properties
  return {
    activitiesGroup,
    activity,
    isLoadingActivity,
    isLoading,
    updateActivity,
    createActivity,
    deleteActivity,
    updateAttendance,
    isFetchingNextPage,
    fetchNextPage,
    hasNextPage,
  };
};
