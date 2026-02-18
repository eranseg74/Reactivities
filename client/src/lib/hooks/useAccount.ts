import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import agent from '../api/agent';
import { useNavigate } from 'react-router';
import type { RegisterSchema } from '../schemas/registerSchema';
import { toast } from 'react-toastify';
import type { ChangePasswordSchema } from '../schemas/changePasswordSchema';

export const useAccount = () => {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  // Queries
  const { data: currentUser, isLoading: loadingUserInfo } = useQuery({
    queryKey: ['user'],
    queryFn: async () => {
      const response = await agent.get<User>('/account/user-info');
      return response.data;
    },
    // We don't want to run this function every time we navigate to a page because it will make an API call every time. We only want to run this function when the user is not logged in because if the user is logged in we already have the user info in the cache. So we check if the user info is in the cache and if it is not we run the function to get the user info. This way we only run the function when we need to and we don't make unnecessary API calls.
    enabled: !queryClient.getQueryData(['user']),
  });

  // Mutations
  // This function is the login function which is provided by Identity framework. If authenticated it will automatically create a cookie for the user. On success it will run the user-info API call to get the user details because the browser cannot access the cookie. This is done by invalidating the user info on he cache which will make the application run the query again to get the data. This way the browser knows which user is entered for UI purposes.
  const loginUser = useMutation({
    mutationFn: async (creds: LoginStatus) => {
      await agent.post('/login?useCookies=true', creds);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['user'] });
      // await navigate('/activities'); // We need to await this because we need to make sure the user info is updated before we navigate to the activities page. Otherwise the user info will be null and the activities page will not render correctly.
    },
  });

  const registerUser = useMutation({
    mutationFn: async (creds: RegisterSchema) => {
      await agent.post('/account/register', creds);
    },
  });

  const logoutUser = useMutation({
    mutationFn: async () => {
      await agent.post('/account/logout');
    },
    onSuccess: async () => {
      queryClient.removeQueries({ queryKey: ['user'] });
      queryClient.removeQueries({ queryKey: ['activities'] });
      await navigate('/');
    },
  });

  // The URL can be validated in the dotnet documentation since we are using the endpoint provided by Identity
  const verifyEmail = useMutation({
    mutationFn: async ({ userId, code }: { userId: string; code: string }) => {
      await agent.get(`/confirmEmail?userId=${userId}&code=${code}`);
    },
  });

  const resendConfirmationEmail = useMutation({
    mutationFn: async ({
      email,
      userId,
    }: {
      email?: string;
      userId?: string | null;
    }) => {
      // await agent.get(`/account/resendConfirmEmail?email=${email}&userId={userId}`); // In case of many params it is better to write it like this:
      await agent.get(`/account/resendConfirmEmail`, {
        params: {
          email,
          userId,
        },
      });
    },
    onSuccess: () => {
      toast.success('Email sent - please check your email');
    },
  });

  const changePassword = useMutation({
    mutationFn: async (data: ChangePasswordSchema) => {
      await agent.post('/account/change-password', data);
    },
  });

  const forgotPassword = useMutation({
    mutationFn: async (email: string) => {
      await agent.post('/forgotPassword', { email });
    },
  });

  const resetPassword = useMutation({
    mutationFn: async (data: ResetPassword) => {
      await agent.post('/resetPassword', data);
    },
  });

  return {
    loginUser,
    currentUser,
    logoutUser,
    loadingUserInfo,
    registerUser,
    verifyEmail,
    resendConfirmationEmail,
    changePassword,
    forgotPassword,
    resetPassword,
  };
};
