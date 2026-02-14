import { useLocalObservable } from 'mobx-react-lite';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { useEffect, useRef } from 'react';
import { runInAction } from 'mobx';

export const useComments = (activityId?: string) => {
  // Note that in development environment, because we are in StrictMode, each call is executed twice. We do not want this behavior when establishing a SignalR connection because this will produve 2 different connections. To avoid that we use the useRef hook. useRef elements are not re-defined on every render but only when implicitly instructing a change which is exactly what we need:
  const created = useRef(false);
  // `defining observsble properties
  const commentStore = useLocalObservable(() => ({
    comments: [] as ChatComment[],
    // This is where we will store the connection to the signalR hub. Setting it to null for now, but we will update it once we create the connection in the commentStore.ts file. So we are defining it as null but saying it can be either null or a HubConnection. This is important for TypeScript to understand the type of this variable.
    hubConnection: null as HubConnection | null, // Basically saying it can be null or a hub connection
    createHubConnection(activityId: string) {
      if (!activityId) {
        return;
      }
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(
          `${import.meta.env.VITE_COMMENTS_URL}?activityId=${activityId}`,
          // setting the withCredentials to true because we use cookies
          { withCredentials: true },
        )
        // Configures the @microsoft/signalr.HubConnection to automatically attempt to reconnect if the connection is lost. By default, the client will wait 0, 2, 10 and 30 seconds respectively before trying up to 4 reconnect attempts.
        .withAutomaticReconnect()
        // Creates a @microsoft/signalr.HubConnection from the configuration options specified in this builder.
        // Returns — The configured @microsoft/signalr.HubConnection.
        .build();

      this.hubConnection
        .start()
        .catch((error) =>
          console.log('Error establishing connection: ', error),
        );
      // We need to listen for the 'LoadComments' method. The on method registers a handler that will be invoked when the hub method with the specified method name is invoked. The 'LoadComments' must match the name defined in the CommentHub.cs file.
      this.hubConnection.on('LoadComments', (comments) => {
        // Defining the this.comments = comments; as a MobX action. Otherwise we will get a warning ([MobX] Since strict-mode is enabled, changing (observed) observable values without using an action is not allowed.)
        runInAction(() => {
          this.comments = comments;
        });
      });

      this.hubConnection.on('ReceiveComment', (comment) => {
        runInAction(() => {
          this.comments.unshift(comment); // This will put the comment at the beginning of the array and not last like when using the push method.
        });
      });
    },
    // Defining the stop connection
    stopHubConnection() {
      if (this.hubConnection?.state === HubConnectionState.Connected) {
        this.hubConnection
          .stop()
          .catch((error) => console.log('Error stopping connection: ', error));
      }
    },
  }));

  useEffect(() => {
    if (activityId && !created.current) {
      commentStore.createHubConnection(activityId);
      created.current = true;
    }
    return () => {
      commentStore.stopHubConnection();
      commentStore.comments = []; // Resetting the comments array in the commentStore
    };
  }, [activityId, commentStore]);

  return { commentStore };
};
