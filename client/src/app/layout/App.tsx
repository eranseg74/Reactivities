import { Box, Container, CssBaseline } from '@mui/material';

import NavBar from './NavBar';
import { Outlet, ScrollRestoration, useLocation } from 'react-router';
import HomePage from '../../features/home/HomePage';

function App() {
  const location = useLocation();

  return (
    <Box sx={{ bgcolor: '#eeeeee', minHeight: '100vh' }}>
      {/* Kickstart an elegant, consistent, and simple baseline to build upon. Removes the margin and padding */}
      <ScrollRestoration />{' '}
      {/* A component from react-router. This component emulates the browser's scroll restoration on location changes. Apps should only render one of these, right before the Scripts component. Basically it will always put us at the top of the page when changing views */}
      <CssBaseline />
      {location.pathname === '/' ? (
        <HomePage />
      ) : (
        <>
          <NavBar />
          <Container maxWidth='xl' sx={{ pt: 14 }}>
            {/* The outlet is the place where the routed components are rendered. When we route to a specific path, the component that is defined as its element will be rendered here */}
            <Outlet />
          </Container>
        </>
      )}
    </Box>
  );
}

export default App;
