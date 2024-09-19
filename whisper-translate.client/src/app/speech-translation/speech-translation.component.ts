import { Component, NgZone, OnInit } from '@angular/core';
import { AudioCaptureService } from '../services/audio-capture.service';
import { NgFor, NgIf } from '@angular/common';
import { WebSocketService } from '../services/websocket.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-speech-translation',
  standalone: true,
  imports: [NgFor, NgIf, FormsModule],
  templateUrl: './speech-translation.component.html',
  styleUrl: './speech-translation.component.css'
})
export class SpeechTranslationComponent implements OnInit {
  recognizedText: string[] = [];
  translatedText: string[] = [];
  responseOptions: string[] = [];
  recording = false;
  
  transferMethod: 'http' | 'websocket' = 'http'; // Default HTTP

  constructor(
    private audioService: AudioCaptureService, 
    private webSocketService: WebSocketService,
    private ngZone: NgZone) {}

  ngOnInit(): void {
    this.loadSessionData();
  }

  startRecording(): void {
    this.recording = true;
    this.audioService.startRecording(this.transferMethod, (response: any) => {
      // Updating results for HTTP method
      if (this.transferMethod === 'http') {
        this.ngZone.run(() => {
          this.updateResults(response);
          this.saveSessionData();
        });
      }
    });

    if (this.transferMethod === 'websocket') {
      this.webSocketService.connect();

      this.webSocketService.onMessage().subscribe((data: any) => {
        // Updating results for WebSocket method
        this.ngZone.run(() => {
          this.updateResults(data);
          this.saveSessionData();
        });
      });
    }

    console.log('Start recording');
  }

  stopRecording(): void {
    this.recording = false;
    this.audioService.stopRecording(this.transferMethod);
    if (this.transferMethod === 'websocket') {
      this.webSocketService.disconnect();
    }
    console.log('Stop recording');
  }

  // Update result from the server side
  updateResults(response: any): void {
    this.recognizedText.unshift(response.recognizedText);
    this.translatedText.unshift(response.translatedText);
    this.responseOptions.unshift(...response.responseOptions);
  }

  clearResults(): void {
    this.recognizedText = [];
    this.translatedText = [];
    this.responseOptions = [];
  }

  // Save data in sessionStorage
  saveSessionData(): void {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      sessionStorage.setItem('recognizedText', JSON.stringify(this.recognizedText));
      sessionStorage.setItem('translatedText', JSON.stringify(this.translatedText));
      sessionStorage.setItem('responseOptions', JSON.stringify(this.responseOptions));
    }
  }

  // Load data from sessionStorage
  loadSessionData(): void {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      const savedRecognizedText = sessionStorage.getItem('recognizedText');
      const savedTranslatedText = sessionStorage.getItem('translatedText');
      const savedResponseOptions = sessionStorage.getItem('responseOptions');

      if (savedRecognizedText) {
        this.recognizedText = JSON.parse(savedRecognizedText);
      }
      if (savedTranslatedText) {
        this.translatedText = JSON.parse(savedTranslatedText);
      }
      if (savedResponseOptions) {
        this.responseOptions = JSON.parse(savedResponseOptions);
      }
    }
  }
}
