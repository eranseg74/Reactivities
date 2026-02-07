// import { action, makeAutoObservable, makeObservable, observable } from "mobx";
import { makeAutoObservable } from "mobx";

export default class CounterStore {
  title = "Counter Store";
  count = 42;
  events: string[] = [`Initial count is: ${this.count}`];

  constructor() {
    // In this approach, MobX decides what are the observables and what are the actions. Basically it will take all the class properties and make them observables, and define all the class functions as actions.
    makeAutoObservable(this);
  }

  /* // In this approach we are specifically defining what are observables and what are actions
  constructor() {
    // MobX 6 requires to explicitly make the class observable. The makeObservable function is used to specify which properties and methods are observable and which are actions.
    makeObservable(this, {
      title: observable,
      count: observable,
      // increment: action.bound, // The action.bound decorator is used to automatically bind the increment method to the instance of the CounterStore class, which is necessary for it to work correctly when called from components. However, since we are using arrow functions for the increment and decrement methods, we don't need to use action.bound, and we can simply use action instead.
      increment: action,
      decrement: action,
    });
  }
  */
  // Using arrow functions in a class, for the increment and decrement methods, ensures that they are bound to the instance of the CounterStore class, which is necessary for them to work correctly when called from components. This is because when we pass these methods as callbacks to components, they lose their context (the value of this), and using arrow functions allows us to maintain the correct context without having to bind them manually in the constructor.
  increment = (amount = 1) => {
    this.count += amount;
    this.events.push(`Incremented by ${amount} - count is now ${this.count}`);
  };

  decrement = (amount = 1) => {
    this.count -= amount;
    this.events.push(`Decremented by ${amount} - count is now ${this.count}`);
  };

  // In order to use a comuted property we need to use the get keyword and specifying a propert name:
  get eventCount() {
    return this.events.length;
  }
}
