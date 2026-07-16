export type MediaType = 'Image' | 'Video';

export interface MediaInput {
  fileName: string;
  fileUrl: string;
  mediaType: MediaType;
}

export interface MediaItem extends MediaInput {
  id: string;
}
