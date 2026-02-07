import { createContext } from "react";
import CounterStore from "./couterStore";
import { UIStore } from "./uiStore";

interface Store {
  counterStore: CounterStore;
  uiStore: UIStore;
}

export const store: Store = {
  counterStore: new CounterStore(),
  uiStore: new UIStore(),
};
// The StoreContext is created using React's createContext function, which allows us to pass the store down through the component tree without having to pass props manually at every level. The store is provided to the entire application in the main.tsx file using the StoreContext.Provider component.
// The createContext function creates a context object that can be used to share data (in this case, the store) across the component tree. The value of the context is set to the store object we created, which contains our CounterStore instance. This allows any component that consumes the StoreContext to access the counterStore and its properties and methods.
export const StoreContext = createContext(store);
