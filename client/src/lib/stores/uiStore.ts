import { makeAutoObservable } from "mobx";

// This store will track the state of desired user interface elements
export class UIStore {
  isLoading = false;

  constructor() {
    makeAutoObservable(this);
  }

  isBusy() {
    this.isLoading = true;
  }

  isIdle() {
    this.isLoading = false;
  }
}
