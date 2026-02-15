import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import agent from '../api/agent';
import { useMemo } from 'react';
import type { EditProfileSchema } from '../schemas/editProfileSchema';

export const useProfile = (id?: string, predicate?: string) => {
  const queryClient = useQueryClient();
  const { data: profile, isLoading: loadingProfile } = useQuery<Profile>({
    queryKey: ['profile', id],
    queryFn: async () => {
      const response = await agent.get<Profile>(`/profiles/${id}`);
      return response.data;
    },
    enabled: !!id && !predicate, // Only run the query if id is provided
  });

  const { data: photos, isLoading: loadingPhotos } = useQuery<Photo[]>({
    queryKey: ['photos', id],
    queryFn: async () => {
      const response = await agent.get<Photo[]>(`/profiles/${id}/photos`);
      return response.data;
    },
    enabled: !!id && !predicate, // Only run the query if id is provided and the predicate is not provided
  });

  const { data: followings, isLoading: loadingFollowings } = useQuery<
    Profile[]
  >({
    queryKey: ['followings', id, predicate],
    queryFn: async () => {
      const response = await agent.get<Profile[]>(
        `/profiles/${id}/follow-list?predicate=${predicate}`,
      );
      return response.data;
    },
    enabled: !!id && !!predicate,
  });

  const uploadPhoto = useMutation({
    mutationFn: async (file: Blob) => {
      const formData = new FormData();
      formData.append('file', file);
      const response = await agent.post('/profiles/add-photo', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      return response.data;
    },
    onSuccess: async (photo: Photo) => {
      await queryClient.invalidateQueries({
        queryKey: ['photos', id],
      });
      queryClient.setQueryData(['user'], (data: User) => {
        if (!data) return data;
        return {
          ...data,
          imageUrl: data.imageUrl ?? photo.url,
        };
      });
      queryClient.setQueryData(['profile', id], (data: Profile) => {
        if (!data) return data;
        return {
          ...data,
          imageUrl: data.imageUrl ?? photo.url,
        };
      });
    },
  });

  const setMainPhoto = useMutation({
    mutationFn: async (photo: Photo) => {
      await agent.put(`/profiles/${photo.id}/setMain`);
    },
    // The first argument is the data received from the API (which we do not need so we placed the _ sign).
    // The second argument is the variables passed to the mutation function (in this case - the photo itself).
    // This way, on success we have access to the variables sent to the API call + the API response.
    /* Non optimistic approach:
    onSuccess: async (_, photo) => {
      // Updating the user in the cache
      queryClient.setQueryData(['user'], (userData: User) => {
        if (!userData) {
          return userData;
        }
        return {
          ...userData,
          imageUrl: photo.url,
        };
      });
      // Also need to update the profile in the cache
      queryClient.setQueryData(['profile', id], (userProfile: Profile) => {
        if (!userProfile) {
          return userProfile;
        }
        return {
          ...userProfile,
          imageUrl: photo.url,
        };
      });
    },
    */
    // Optimistic approach:
    onMutate: async (photo: Photo) => {
      await queryClient.cancelQueries({ queryKey: ['profile', photo.id] });
      const prevProfile = queryClient.getQueryData<Profile>([
        'profile',
        photo.id,
      ]);
      const prevUser = queryClient.getQueryData<User>(['user']);

      // Updating the user in the cache
      queryClient.setQueryData<User>(['user'], (userData) => {
        if (!userData) {
          return userData;
        }
        return {
          ...userData,
          imageUrl: photo.url,
        };
      });
      // Also need to update the profile in the cache
      queryClient.setQueryData<Profile>(['profile', id], (oldProfile) => {
        if (!oldProfile) {
          return oldProfile;
        }
        return {
          ...oldProfile,
          imageUrl: photo.url,
        };
      });
      return { prevProfile, prevUser };
    },
    onError: (error, profileId, context) => {
      console.log(error);
      if (context?.prevProfile) {
        queryClient.setQueryData(['profile', profileId], context.prevProfile);
      }
      if (context?.prevUser) {
        queryClient.setQueryData(['user'], context?.prevUser);
      }
    },
  });

  const deletePhoto = useMutation({
    mutationFn: async (photoId: string) => {
      await agent.delete(`/profiles/${photoId}/photos`);
    }, // After deleting the photo on the server we need to update the photos in the cache or invalidate the data
    onSuccess: (_, photoId) => {
      queryClient.setQueryData(['photos', id], (photos: Photo[]) => {
        return photos?.filter((x) => x.id !== photoId);
      });
    },
  });

  const editProfile = useMutation({
    mutationFn: async (formData: EditProfileSchema) => {
      await agent.put('/profiles', formData);
    },
    onMutate: async (formData: EditProfileSchema) => {
      // Cancel all ongoing queries on the current user's profile
      await queryClient.cancelQueries({ queryKey: ['profile', id] });

      // Getting the current data on the user and profile from the cache
      const prevProfile = queryClient.getQueryData<Profile>(['profile', id]);
      const prevUser = queryClient.getQueryData<User>(['user']);

      // Setting the data in the cache for both the user and the profile
      queryClient.setQueryData<Profile>(['profile', id], (oldProfile) => {
        if (!oldProfile) {
          return oldProfile;
        }
        return {
          ...oldProfile,
          displayName: formData.displayName,
          bio: formData.bio,
        };
      });
      queryClient.setQueryData<User>(['user'], (userData) => {
        if (!userData) {
          return userData;
        }
        return {
          ...userData,
          displayName: formData.displayName,
        };
      });
      // Returning the current (not updated) user and profile in case of update failure.
      return { prevProfile, prevUser };
    },
    onError: (error, profileId, context) => {
      console.log(error);
      if (context?.prevProfile) {
        queryClient.setQueryData(['profiles', profileId], context.prevProfile);
      }
      if (context?.prevUser) {
        queryClient.setQueryData(['user'], context.prevUser);
      }
    },
  });

  const updateFollowing = useMutation({
    mutationFn: async () => {
      await agent.post(`/profiles/${id}/follow`);
    },
    onSuccess: () => {
      queryClient.setQueryData(['profile', id], (profile: Profile) => {
        // We also invalidate the data in the cache with the followings key so the list of photos will automatically update when the user clicks on the Follow / Unfollow button
        queryClient.invalidateQueries({
          queryKey: ['followings', id, 'followers'],
        });
        if (!profile || profile.followersCount === undefined) {
          // 0 is also true so we need to check if undefined
          return profile;
        }
        return {
          ...profile,
          following: !profile.following,
          followersCount: profile.following
            ? profile.followersCount - 1
            : profile.followersCount + 1,
        };
      });
    },
  });

  // Determine if the profile being viewed belongs to the current user. The useMemo hook is used to optimize performance by memoizing the result, so it only recalculates when the id or queryClient changes.
  // The ['user'] query is assumed to hold the current user's data, and we compare the id from the profile being viewed with the id of the current user to determine if they are the same. This is useful for conditionally rendering certain UI elements or allowing specific actions (like editing the profile) only if the user is viewing their own profile. The ['user'] comes from the query cache, which is managed by React Query, and it should have been set somewhere else in the application when the user logged in or their data was fetched.
  const isCurrentUser = useMemo(() => {
    return id === queryClient.getQueryData<User>(['user'])?.id;
  }, [id, queryClient]);

  return {
    profile,
    loadingProfile,
    photos,
    loadingPhotos,
    isCurrentUser,
    uploadPhoto,
    setMainPhoto,
    deletePhoto,
    editProfile,
    updateFollowing,
    followings,
    loadingFollowings,
  };
};
