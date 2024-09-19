import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { WebSocketService } from './websocket.service';

@Injectable({
  providedIn: 'root',
})
export class AudioCaptureService {
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: Blob[] = [];
  private isRecording: boolean = false;

  constructor(
    private http: HttpClient,
    private webSocketService: WebSocketService
  ) {}

  async startRecording(
    transferMethod: 'http' | 'websocket',
    httpCallback?: (response: any) => void): Promise<void> {
      if (this.isRecording) {
        console.error('Recording is already in progress.');
        return;
      }

      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

      this.mediaRecorder = new MediaRecorder(stream, {
        mimeType: 'audio/webm;codecs=opus',
      });

      this.isRecording = true;
      this.audioChunks = [];

      this.mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          this.audioChunks.push(event.data);

          if (transferMethod === 'websocket') {
            const audioBlob = new Blob([event.data], { type: 'audio/webm' });
            this.webSocketService.sendAudio(audioBlob);
          }
        }
      };

      this.mediaRecorder.onstop = () => {
        if (transferMethod === 'http' && httpCallback) {
          this.sendAudio().subscribe({
            next: (response) => {
              httpCallback(response);
            },
            error: (error) => {
              console.error('Error sending audio: ', error);
            },
          });
        }
        this.isRecording = false;
      };

      // Split audio into chunks every 10 seconds
      this.mediaRecorder.start(10000); 
  }

  stopRecording(transferMethod: 'http' | 'websocket'): void {
    if (this.mediaRecorder && this.isRecording) {
      this.mediaRecorder.stop();
    }
  }

  sendAudio(): Observable<any> {
    if (this.audioChunks.length === 0) {
      console.error('There is no data to send.');
      return new Observable((observer) => {
        observer.error('There is no data to send.');
        observer.complete();
      });
    }

    const audioBlob = new Blob(this.audioChunks, { type: 'audio/webm' });
    const formData = new FormData();
    formData.append('audio', audioBlob, 'audio.webm');

    const postRequest = this.http.post('https://localhost:5000/api/audio/upload', formData);

    // Cleaning up audio fragments after sending
    this.audioChunks = []; 

    return postRequest;
  }
}
