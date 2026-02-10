import { Typography } from '@mui/material';
import { useAccount } from '../../lib/hooks/useAccount';
import { Navigate, Outlet, useLocation } from 'react-router';

export default function RequireAuth() {
  const { currentUser, loadingUserInfo } = useAccount();
  const location = useLocation();

  if (loadingUserInfo) {
    return <Typography>Loading...</Typography>;
  }

  if (!currentUser) {
    // If the user is not logged in we return to the login page and send the current location so when he will log in we will take him to the page he came from
    return <Navigate to='/login' state={{ from: location }} />;
  }

  return <Outlet />;
}
