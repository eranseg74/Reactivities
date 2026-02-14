import { useForm } from 'react-hook-form';
import {
  editProfileSchema,
  type EditProfileSchema,
} from '../../lib/schemas/editProfileSchema';
import { zodResolver } from '@hookform/resolvers/zod';
import { Box, Button } from '@mui/material';
import TextInput from '../../app/shared/components/TextInput';
import { useProfile } from '../../lib/hooks/useProfile';
import { useParams } from 'react-router';

type Props = {
  setEditMode: (mode: boolean) => void;
};

export default function ProfileEditForm({ setEditMode }: Props) {
  const { control, handleSubmit } = useForm<EditProfileSchema>({
    mode: 'onTouched',
    resolver: zodResolver(editProfileSchema),
  });
  const { id } = useParams();
  const { editProfile } = useProfile(id);

  const onSubmit = async (data: EditProfileSchema) => {
    editProfile.mutate(data, {
      onSuccess: () => setEditMode(false),
    });
  };

  return (
    <Box
      component='form'
      onSubmit={handleSubmit(onSubmit)}
      display='flex'
      flexDirection='column'
      gap={3}>
      <TextInput label='Display Name' control={control} name='displayName' />
      <TextInput label='Bio' multiline control={control} rows={5} name='bio' />
      <Button type='submit' variant='contained' fullWidth>
        Update profile
      </Button>
    </Box>
  );
}
