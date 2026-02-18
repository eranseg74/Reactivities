import z from 'zod';
import { requiredString } from '../util/util';

export const registerSchema = z.object({
  email: z.email(),
  displayName: requiredString('displayName'),
  password: requiredString('password'), // This validation is only in the client side and checks that the password is not empty. There are more validations on the server side that will be presented to the user if they occur.
});

export type RegisterSchema = z.infer<typeof registerSchema>;
