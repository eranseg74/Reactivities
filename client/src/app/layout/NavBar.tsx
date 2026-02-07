import { Group } from "@mui/icons-material";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Container,
  MenuItem,
} from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";

export default function NavBar() {
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position='static'
        sx={{
          backgroundImage:
            "linear-gradient(135deg, #182a73 0%, #218aae 69%, #20a7ac 89%)",
        }}
      >
        <Container maxWidth='xl'>
          <Toolbar sx={{ display: "flex", justifyContent: "space-between" }}>
            <Box>
              <MenuItem
                component={NavLink}
                to='/'
                sx={{ display: "flex", gap: 2 }}
              >
                <Group fontSize='large' />
                <Typography variant='h4' fontWeight='bold'>
                  Reactivities
                </Typography>
              </MenuItem>
            </Box>
            <Box sx={{ display: "flex" }}>
              <MenuItemLink
                // Adding the '/' prefix to ensure the link is resolved correctly and doesn't cause issues with relative paths. Otherswise, if the current URL is something like '/activities', clicking on the 'Activities' link without the '/' prefix could lead to a URL like '/activities/activities', which is not intended.
                to='/activities'
              >
                Activities
              </MenuItemLink>
              <MenuItemLink to='/createActivity'>Create Activity</MenuItemLink>
            </Box>
            <MenuItem>User Menu</MenuItem>
          </Toolbar>
        </Container>
      </AppBar>
    </Box>
  );
}
