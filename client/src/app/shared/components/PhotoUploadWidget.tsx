import { CloudUpload } from '@mui/icons-material';
import { Box, Button, Grid, Typography } from '@mui/material';
import { useCallback, useEffect, useRef, useState } from 'react';
import { useDropzone } from 'react-dropzone';
import Cropper, { type ReactCropperElement } from 'react-cropper';
import 'cropperjs/dist/cropper.css';

type Props = {
  uploadPhoto: (file: Blob) => void;
  loading: boolean;
};

export default function PhotoUploadWidget({ uploadPhoto, loading }: Props) {
  const [files, setFiles] = useState<object & { preview: string }[]>([]);
  const cropperRef = useRef<ReactCropperElement>(null);

  useEffect(() => {
    return () => {
      // In the onDrop function we create an ObjectURL which is basically a string containing a URL representing the object given in the parameter. These object stay in memory and can cause memory leak so we need to revoke them after loading an image or exiting this component. We use the useEffect by specifying a return function which is a function that will run when the component is unmounted.
      files.forEach((file) => URL.revokeObjectURL(file.preview));
    };
  }, [files]);

  // The onDrop function is defined using the useCallback hook to ensure that it is not recreated on every render, which can help with performance. When files are dropped into the dropzone, the acceptedFiles array is processed to create a new array of file objects that include a preview URL. This preview URL is generated using the URL.createObjectURL method, which creates a temporary URL that can be used to display the image in the browser without needing to upload it first. The resulting array of file objects with their corresponding preview URLs is then stored in the component's state using the setFiles function. This allows the component to manage and display the uploaded images as needed.
  const onDrop = useCallback((acceptedFiles: File[]) => {
    setFiles(
      acceptedFiles.map((file) =>
        Object.assign(file, {
          preview: URL.createObjectURL(file as Blob),
        }),
      ),
    );
  }, []);

  const onCrop = useCallback(() => {
    const cropper = cropperRef.current?.cropper;
    cropper?.getCroppedCanvas().toBlob((blob) => {
      uploadPhoto(blob as Blob);
    });
  }, [uploadPhoto]);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <Grid container spacing={3}>
      <Grid size={4}>
        <Typography variant='overline' color='secondary'>
          Step 1 - Add photo
        </Typography>
        <Box
          {...getRootProps()}
          sx={{
            border: 'dashed 3px #eee',
            borderColor: isDragActive ? 'green' : '#eee',
            borderRadius: '5px',
            paddingTop: '30px',
            textAlign: 'center',
            height: '280px',
          }}>
          <input {...getInputProps()} />
          <CloudUpload sx={{ fontSize: 80 }} />
          <Typography variant='h5'>Drop image here</Typography>
        </Box>
      </Grid>
      <Grid size={4}>
        <Typography variant='overline' color='secondary'>
          Step 2 - Resize image
        </Typography>
        {files[0]?.preview && (
          <Cropper
            src={files[0]?.preview}
            style={{ height: 300, width: '90%' }}
            initialAspectRatio={1}
            aspectRatio={1}
            preview='.img-preview'
            guides={false}
            viewMode={1}
            background={false}
            ref={cropperRef}
          />
        )}
      </Grid>
      <Grid size={4}>
        {files[0]?.preview && (
          <>
            <Typography variant='overline' color='secondary'>
              Step 3 - Preview & upload
            </Typography>
            <div
              className='img-preview'
              style={{ width: 300, height: 300, overflow: 'hidden' }}
            />
            <Button
              sx={{ my: 1, width: 300 }}
              onClick={onCrop}
              variant='contained'
              color='secondary'
              disabled={loading}
              loading={loading}>
              Upload
            </Button>
          </>
        )}
      </Grid>
    </Grid>
  );
}
