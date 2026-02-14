import { useParams } from 'react-router';
import { useProfile } from '../../lib/hooks/useProfile';
import {
  Box,
  Button,
  Divider,
  ImageList,
  ImageListItem,
  Typography,
} from '@mui/material';
import { useState } from 'react';
import PhotoUploadWidget from '../../app/shared/components/PhotoUploadWidget';
import StarButton from '../../app/shared/components/StarButton';
import DeleteButton from '../../app/shared/components/DeleteButon';

export default function ProfilePhotos() {
  const { id } = useParams();
  const {
    photos,
    loadingPhotos,
    isCurrentUser,
    uploadPhoto,
    profile,
    setMainPhoto,
    deletePhoto,
  } = useProfile(id);
  const [editMode, setEditMode] = useState(false);

  const handlePhotoUpload = (file: Blob) => {
    uploadPhoto.mutate(file, {
      onSuccess: () => {
        setEditMode(false);
      },
    });
  };

  if (loadingPhotos) {
    return <Typography>Loading photos...</Typography>;
  }
  if (!photos || photos.length === 0) {
    return <Typography>No photos found for this user</Typography>;
  }
  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
        <Typography variant='h5'>Photos</Typography>
        {isCurrentUser && (
          <Button onClick={() => setEditMode(!editMode)}>
            {editMode ? 'Cancel' : 'Add photo'}
          </Button>
        )}
      </Box>
      <Divider sx={{ my: 2 }} />
      {editMode ? (
        <PhotoUploadWidget
          uploadPhoto={handlePhotoUpload}
          loading={uploadPhoto.isPending}
        />
      ) : (
        <>
          {photos.length === 0 ? (
            <Typography>No photos added yet</Typography>
          ) : (
            <Box sx={{ width: 500, height: 450, overflowY: 'scroll' }}>
              <ImageList variant='masonry' cols={3} gap={8}>
                {photos.map((photo, index) => (
                  <ImageListItem key={photo.url}>
                    <img
                      // For using Cloudinary features
                      srcSet={`${photo.url.replace('/upload/', '/upload/w_164,h_164,c_fill,f_auto,dpr_2,g_face/')}`}
                      src={`${photo.url.replace('/upload/', '/upload/w_164,h_164,c_fill,f_auto,g_face/')}`}
                      // srcSet={`${photo.url}?w=248&fit=crop&auto=format&dpr=2 2x`}
                      // src={`${photo.url}?w=248&fit=crop&auto=format`}
                      alt={`user profile image ${index}`}
                      loading='lazy'
                    />
                    {isCurrentUser && (
                      <div>
                        <Box
                          sx={{ position: 'absolute', top: 0, left: 0 }}
                          onClick={() => setMainPhoto.mutate(photo)}>
                          <StarButton
                            selected={photo.url === profile?.imageUrl}
                          />
                        </Box>
                        {profile?.imageUrl !== photo.url && (
                          <Box
                            sx={{ position: 'absolute', top: 0, right: 0 }}
                            onClick={() => deletePhoto.mutate(photo.id)}>
                            <DeleteButton />
                          </Box>
                        )}
                      </div>
                    )}
                  </ImageListItem>
                ))}
              </ImageList>
            </Box>
          )}
        </>
      )}
    </Box>
  );
}
