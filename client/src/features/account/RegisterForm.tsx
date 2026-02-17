import { useForm, useWatch } from 'react-hook-form';
import { useAccount } from '../../lib/hooks/useAccount';
import { zodResolver } from '@hookform/resolvers/zod';
import { Box, Button, Paper, Typography } from '@mui/material';
import { LockOpen } from '@mui/icons-material';
import TextInput from '../../app/shared/components/TextInput';
import { Link } from 'react-router';
import {
  registerSchema,
  type RegisterSchema,
} from '../../lib/schemas/registerSchema';
import { useState } from 'react';
import RegisterSuccess from './RegisterSuccess';

export default function RegisterForm() {
  const { registerUser } = useAccount();
  const [registerSuccess, setRegisterSuccess] = useState(false);
  const {
    control,
    handleSubmit,
    setError, // The setError allows us to manually set errors from the errors that are comming from the API and they will appear as field based errors so we will be able to set the email or password field error. The implementation is in the onSubmit function
    formState: { isValid, isSubmitting },
  } = useForm<RegisterSchema>({
    mode: 'onTouched',
    resolver: zodResolver(registerSchema),
  });

  const email = useWatch({ control, name: 'email' });

  const onSubmit = async (data: RegisterSchema) => {
    await registerUser.mutateAsync(data, {
      onSuccess: () => setRegisterSuccess(true),
      onError: (error) => {
        if (Array.isArray(error)) {
          error.forEach((err) => {
            if (err.includes('Email')) {
              setError('email', { message: err });
            } else if (err.includes('Password')) {
              setError('password', { message: err });
            }
          });
        }
      },
    });
  };

  return (
    <>
      {registerSuccess ? (
        <RegisterSuccess email={email} />
      ) : (
        <Paper
          component='form'
          onSubmit={handleSubmit(onSubmit)}
          sx={{
            display: 'flex',
            flexDirection: 'column',
            p: 3,
            gap: 3,
            maxWidth: 'md',
            mx: 'auto',
            borderRadius: 3,
          }}>
          <Box
            display='flex'
            alignItems='center'
            justifyContent='center'
            gap={3}
            color='secondary.main'>
            <LockOpen fontSize='large' />
            <Typography variant='h4'>Register</Typography>
          </Box>
          <TextInput label='Email' control={control} name='email'></TextInput>
          <TextInput
            label='Display Name'
            control={control}
            name='displayName'></TextInput>
          <TextInput
            label='Password'
            type='password'
            control={control}
            name='password'></TextInput>
          <Button
            type='submit'
            variant='contained'
            disabled={!isValid || isSubmitting}
            loading={isSubmitting}
            size='large'>
            Register
          </Button>
          <Typography sx={{ textAlign: 'center' }}>
            Already have an account?
            <Typography
              component={Link}
              to='/login'
              color='primary'
              sx={{ p: 2 }}>
              Sign in
            </Typography>
          </Typography>
        </Paper>
      )}
    </>
  );
}
