// index.d.ts usually refers to types definition. This is the file that we will define the types in our application so we will be able to use them to satisfy typescript

// The best way to define types while avoiding typing errors is to take the object data from the API call and paste it in the Json to Typescript site

type Activity = {
  id: string;
  title: string;
  date: string;
  description: string;
  category: string;
  city: string;
  venue: string;
  latitude: number;
  longitude: number;
};
