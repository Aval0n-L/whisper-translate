import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AudioCaptureService {
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: Blob[] = [];
  private isRecording: boolean = false;

  constructor(private http: HttpClient) {}

  // Запуск записи
  async startRecording(): Promise<void> {
    if (this.isRecording) {
      console.error('Запись уже идет.');
      return;
    }

    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });

    this.mediaRecorder = new MediaRecorder(stream, {
      mimeType: 'audio/webm;codecs=opus',
    });

    // Логируем состояние записи
    console.log('Начало записи аудио');
    this.isRecording = true;

    // Сохраняем аудиофрагменты по мере их доступности
    this.mediaRecorder.ondataavailable = (event) => {
      if (event.data.size > 0) {
        console.log('Добавлено в audioChunks, размер фрагмента:', event.data.size);
        this.audioChunks.push(event.data);
      }
    };

    // Начинаем запись
    this.mediaRecorder.start();
  }

  // Остановка записи и отправка данных на сервер
  stopRecording(updateResultsCallback: (response: any) => void): void {
    if (!this.isRecording || !this.mediaRecorder) {
      console.error('Запись не идет.');
      return;
    }

    this.mediaRecorder.onstop = () => {
      console.log('Запись остановлена, отправка данных на сервер.');

      // Отправляем накопленные аудиофрагменты
      this.sendAudio().subscribe({
        next: (response) => {
          console.log('Ответ сервера: ', response);
          updateResultsCallback(response);  // Обновляем интерфейс с полученными данными
        },
        error: (error) => {
          console.error('Ошибка при отправке аудио: ', error);
        }
      });

      // Очищаем фрагменты после отправки
      this.audioChunks = [];
      this.isRecording = false;
    };

    // Останавливаем запись
    this.mediaRecorder.stop();
  }

  // Отправка аудиофрагментов на сервер и получение данных
  sendAudio(): Observable<any> {
    // Проверяем, есть ли данные в audioChunks перед отправкой
    if (this.audioChunks.length === 0) {
      console.error('Нет данных для отправки.');
      return new Observable((observer) => {
        observer.error('Нет данных для отправки.');
        observer.complete();
      });
    }

    const audioBlob = new Blob(this.audioChunks, { type: 'audio/m4a' });
    const formData = new FormData();
    formData.append('audio', audioBlob, 'audio.m4a');

    const postRequest = this.http.post('https://localhost:5000/api/audio/upload', formData);

    console.log('Аудиофрагмент отправляется.');

    return postRequest;
  }
}
