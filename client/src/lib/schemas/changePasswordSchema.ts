import z from 'zod';
import { requiredString } from '../util/util';

export const changePasswordSchema = z
  .object({
    currentPassword: requiredString('currentPassword'),
    newPassword: requiredString('newPassword'),
    confirmPassword: requiredString('confirmPassword'),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    error: 'Passwords must match',
    path: ['confirmPassword'], // The field to which we want to provide the validation message (error) for
  });

export type ChangePasswordSchema = z.infer<typeof changePasswordSchema>;
