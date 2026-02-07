import { useContext } from "react";
import { StoreContext } from "../stores/store";

// The useStore hook is a custom hook that allows components to access the store context. It uses the useContext hook to retrieve the current value of the StoreContext, which is the store object we created earlier. By calling useStore in any component, you can access the store and its properties, such as counterStore, without having to pass them down as props.
export function useStore() {
  return useContext(StoreContext);
}
