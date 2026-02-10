import { useForm } from 'react-hook-form';
import { useAccount } from '../../lib/hooks/useAccount';
import { loginSchema, type LoginSchema } from '../../lib/schemas/loginSchema';
import { zodResolver } from '@hookform/resolvers/zod';
import { Box, Button, Paper, Typography } from '@mui/material';
import { LockOpen } from '@mui/icons-material';
import TextInput from '../../app/shared/components/TextInput';
import { Link, useLocation, useNavigate } from 'react-router';

export default function LoginForm() {
  const { loginUser } = useAccount();
  const navigate = useNavigate();
  const location = useLocation();
  const {
    control,
    handleSubmit,
    formState: { isValid, isSubmitting },
  } = useForm<LoginSchema>({
    mode: 'onTouched',
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginSchema) => {
    await loginUser.mutateAsync(data as unknown as LoginStatus, {
      onSuccess: () => {
        navigate(location.state?.from || '/activities'); // This is the 'from' that we defined in the RequireAuth class when we redirect a user if he is not logged in
      },
    });
  };

  return (
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
        <Typography variant='h4'>Sign in</Typography>
      </Box>
      <TextInput label='Email' control={control} name='email'></TextInput>
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
        Login
      </Button>
      <Typography sx={{ textAlign: 'center' }}>
        Don't have an account?
        <Typography
          component={Link}
          to='/register'
          color='primary'
          sx={{ p: 2 }}>
          Sign up
        </Typography>
      </Typography>
    </Paper>
  );
}
