import * as z from 'zod';

const requiredString = (fieldName: string) =>
  z.string({ error: `${fieldName} is required` });

export const activitySchema = z.object({
  title: requiredString('Title'),
  description: requiredString('Description'),
  category: requiredString('Category'),
  // z.coerce.date will try to convert the input to a date, and if it fails, it will return the error message we provided. This is useful because the DateTimePicker component returns a string, and we want to validate that it can be converted to a date.
  date: z.coerce.date({ error: 'Date is required' }),
  location: z.object({
    venue: requiredString('Venue'),
    city: z.string().optional(),
    latitude: z.coerce.number(),
    longitude: z.coerce.number(),
  }),
});

// This will create a type that represents the input of the activitySchema, which is what we will use in our form. The output of the schema is what we will get after validation, which can be different from the input if we have transformations in our schema (like z.coerce.date).
// z.infer<typeof activitySchema> will create a type that represents the output of the activitySchema, which is what we will get after validation. In this case, it will be the same as the input, but if we had transformations in our schema, it could be different and give us errors.
export type ActivitySchema = z.input<typeof activitySchema>;
