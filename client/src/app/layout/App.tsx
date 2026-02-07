import { Box, Container, CssBaseline } from "@mui/material";

import NavBar from "./NavBar";
import { Outlet, useLocation } from "react-router";
import HomePage from "../../features/home/HomePage";

function App() {
  const location = useLocation();

  return (
    <Box sx={{ bgcolor: "#eeeeee", minHeight: "100vh" }}>
      {/* Kickstart an elegant, consistent, and simple baseline to build upon. Removes the margin and padding */}
      <CssBaseline />
      {location.pathname === "/" ? (
        <HomePage />
      ) : (
        <>
          <NavBar />
          <Container maxWidth='xl' sx={{ mt: 3 }}>
            {/* The outlet is the place where the routed components are rendered. When we route to a specific path, the component that is defined as its element will be rendered here */}
            <Outlet />
          </Container>
        </>
      )}
    </Box>
  );
}

export default App;
