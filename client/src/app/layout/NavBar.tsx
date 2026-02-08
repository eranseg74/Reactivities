import { Group } from "@mui/icons-material";
import {
  Box,
  AppBar,
  Toolbar,
  Typography,
  Container,
  MenuItem,
  LinearProgress,
} from "@mui/material";
import { NavLink } from "react-router";
import MenuItemLink from "../shared/components/MenuItemLink";
import { useStore } from "../../lib/hooks/useStore";
import { Observer } from "mobx-react-lite";

export default function NavBar() {
  const { uiStore } = useStore();
  return (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar
        position='relative'
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
              <MenuItemLink to='/counter'>Counter</MenuItemLink>
              <MenuItemLink to='/errors'>Errors</MenuItemLink>
            </Box>
            <MenuItem>User Menu</MenuItem>
          </Toolbar>
        </Container>
        <Observer>
          {() =>
            uiStore.isLoading ? (
              <LinearProgress
                color='secondary'
                sx={{
                  position: "absolute",
                  bottom: 0,
                  left: 0,
                  right: 0,
                  height: 4,
                }}
              />
            ) : null
          }
        </Observer>
      </AppBar>
    </Box>
  );
}
