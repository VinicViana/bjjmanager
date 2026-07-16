import { Injectable } from '@angular/core';
import { FirebaseApp, initializeApp } from 'firebase/app';
import {
  FirebaseStorage,
  getDownloadURL,
  getStorage,
  ref,
  uploadBytesResumable,
  UploadTaskSnapshot
} from 'firebase/storage';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MediaType } from '../models/common.model';

export interface MediaUploadProgress {
  progress: number;
  downloadUrl?: string;
}

@Injectable({ providedIn: 'root' })
export class MediaUploadService {
  private readonly app: FirebaseApp = initializeApp(environment.firebaseConfig);
  private readonly storage: FirebaseStorage = getStorage(this.app);

  uploadFile(folder: string, entityId: string, file: File): Observable<MediaUploadProgress> {
    return new Observable((subscriber) => {
      const safeName = `${Date.now()}-${file.name}`.replace(/\s+/g, '_');
      const storageRef = ref(this.storage, `${folder}/${entityId}/${safeName}`);
      const task = uploadBytesResumable(storageRef, file);

      task.on(
        'state_changed',
        (snapshot: UploadTaskSnapshot) => {
          const progress = snapshot.totalBytes > 0 ? Math.round((snapshot.bytesTransferred / snapshot.totalBytes) * 100) : 0;
          subscriber.next({ progress });
        },
        (error) => subscriber.error(error),
        () => {
          getDownloadURL(task.snapshot.ref)
            .then((downloadUrl) => {
              subscriber.next({ progress: 100, downloadUrl });
              subscriber.complete();
            })
            .catch((error) => subscriber.error(error));
        }
      );
    });
  }

  static mediaTypeFromFile(file: File): MediaType {
    return file.type.startsWith('video/') ? 'Video' : 'Image';
  }
}
