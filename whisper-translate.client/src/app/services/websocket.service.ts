import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WebSocketService {
  private socket: WebSocket | null = null;
  private subject: Subject<any> = new Subject();
  private isConnected: boolean = false;

  connect(): void {
    if (!this.socket || this.socket.readyState !== WebSocket.OPEN) {
      this.socket = new WebSocket('wss://localhost:5000/ws');
      
      this.socket.onopen = () => {
        console.log('WebSocket соединение установлено');
        this.isConnected = true;
      };
      
      this.socket.onmessage = (event) => {
        const data = JSON.parse(event.data);
        
        // Send data to the stream
        this.subject.next(data); 
      };

      this.socket.onerror = (error) => {
        console.error('WebSocket error:', error);
      };

      this.socket.onclose = () => {
        console.log('WebSocket closed');
      };
    } else {
      console.log('WebSocket уже подключен');
    }
  }

  disconnect(): void {
    if (this.socket) {
      this.socket.close();
    }
  }

  sendAudio(audioBlob: Blob): void {
    if (this.socket && this.isConnected)
    {
      const reader = new FileReader();

      reader.onload = () => {
        const audioArrayBuffer = reader.result as ArrayBuffer;

        // Sending audio data
        this.socket?.send(audioArrayBuffer);
      };
      reader.onerror = (error) => {
        console.error('Error reading audio file: ', error);
      };

      reader.readAsArrayBuffer(audioBlob);
    } else {
      console.error('WebSocket is not open for audio transmission');
    }
  }

  // WebSocket the stream message
  onMessage(): Observable<any> {
    return this.subject.asObservable();
  }
}
